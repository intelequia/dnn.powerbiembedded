using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Components;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.Services.Scheduling;
using Microsoft.PowerBI.Api;
using System;
using System.Linq;

namespace DotNetNuke.PowerBI.Tasks
{
    public class DatasetRefreshRetryTask : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DatasetRefreshRetryTask));

        public DatasetRefreshRetryTask(ScheduleHistoryItem item)
        {
            ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            var common = new Common();
            foreach (var setting in SharedSettingsRepository.Instance.GetAllSettings()
                .Where(s => s.RefreshMaxRetries > 0 && s.RefreshRetryDelayMinutes > 0 && !string.IsNullOrEmpty(s.WorkspaceId))
                .GroupBy(s => new { s.PortalId, WorkspaceId = s.WorkspaceId.ToLowerInvariant() })
                .Select(g => g.First()))
            {
                try
                {
                    var workspaceId = Guid.Parse(setting.WorkspaceId);
                    var token = common.GetTokenCredentials(setting).GetAwaiter().GetResult();
                    using (var client = new PowerBIClient(token, new Uri(setting.ApiUrl)))
                    {
                        var datasets = client.Datasets.GetDatasetsInGroupAsync(workspaceId).GetAwaiter().GetResult().Value.Value;
                        foreach (var dataset in datasets.Where(d => d.IsRefreshable == true))
                        {
                            try
                            {
                                // Power BI refuses another refresh while one is running. Re-read the
                                // latest history before sending, including after any previous task run.
                                var history = client.Datasets.GetRefreshHistoryInGroupAsync(workspaceId, dataset.Id, 100)
                                    .GetAwaiter().GetResult().Value.Value;
                                var decision = DatasetRefreshRetry.Evaluate(history, setting.RefreshMaxRetries,
                                    setting.RefreshRetryDelayMinutes, DateTimeOffset.UtcNow);
                                if (decision.Status != "Due")
                                    continue;

                                client.Datasets.RefreshDatasetInGroupAsync(workspaceId, dataset.Id).GetAwaiter().GetResult();
                                ScheduleHistoryItem.AddLogNote($"Retry {decision.Attempts + 1} started for dataset {dataset.Name} in {setting.WorkspaceId}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn($"Unable to retry dataset {dataset.Id} in {setting.WorkspaceId}", ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Unable to check refreshes in workspace {setting.WorkspaceId}", ex);
                }
            }
            ScheduleHistoryItem.Succeeded = true;
        }
    }
}
