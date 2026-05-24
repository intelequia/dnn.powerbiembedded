using DotNetNuke.Instrumentation;
using Azure;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Services.Cache;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetNuke.PowerBI.Services
{
    public class EmbedService : IEmbedService
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EmbedService));
        private EmbedConfig embedConfig;
        private PowerBISettings powerBISettings;
        private TileEmbedConfig tileEmbedConfig;
        private string accessToken;


        public EmbedConfig EmbedConfig
        {
            get { return embedConfig; }
        }

        public TileEmbedConfig TileEmbedConfig
        {
            get { return tileEmbedConfig; }
        }
        public PowerBISettings Settings
        {
            get { return powerBISettings; }
        }


        public EmbedService(int portalId, int tabModuleId)
        {
            accessToken = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
        }

        public EmbedService(int portalId, int tabModuleId, int settingsId)
        {
            accessToken = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            if (settingsId == 0)
            {
                powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
            }
            else
            {
                powerBISettings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, portalId);
            }
        }

        public EmbedService(int portalId, int tabModuleId, string settingsGroupId)
        {
            accessToken = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            if (string.IsNullOrEmpty(settingsGroupId))
            {
                powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
            }
            else
            {
                powerBISettings = SharedSettingsRepository.Instance.GetSettingsByGroupId(settingsGroupId, portalId);
            }
        }

        public async Task<PowerBIListView> GetContentListAsync(int userId)
        {
            var model = (PowerBIListView)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_PowerBIListView");
            if (model != null)
                return model;

            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                return null;
            }
            model = new PowerBIListView()
            {
                WorkspaceId = Settings.WorkspaceId
            };

            var pbiSettings = SharedSettingsRepository.Instance.GetSettings(Settings.PortalId);
            foreach (var s in pbiSettings)
            {
                model.Workspaces.Add(new Workspace()
                {
                    Id = s.SettingsGroupId,
                    Name = s.SettingsGroupName,
                    SettingsId = s.SettingsId,
                    InheritPermissions = s.InheritPermissions
                });
            }
            if (model.Workspaces.Count > 0)
            {
                model.Workspaces = model.Workspaces.OrderBy(x => x.Name).ToList();
            }
            
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                var dashboards = client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).GetAwaiter().GetResult().Value;
                model.Dashboards.AddRange(dashboards.Value?.OrderBy(x => x.DisplayName));

                // Get a list of reports.
                var reports = client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).GetAwaiter().GetResult().Value;
                var cleanedReports = CleanUsageReports(reports.Value.ToList());
                model.Reports.AddRange(cleanedReports?.OrderBy(x => x.Name));
            }
            CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_PowerBIListView", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            return model;
        }

        private List<Report> CleanUsageReports(List<Report> reports)
        {
            if (reports.Any(r => r.Name.Contains("Usage Metrics Report")))
            {
                var i = reports.RemoveAll(r => r.Name.Contains("Usage Metrics Report"));
            }
            return reports;
        }

        public async Task<PowerBICalendarView> GetScheduleInGroup(string mode)
        {

            var model = (PowerBICalendarView)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{Thread.CurrentThread.CurrentUICulture.Name}_CalendarDataSet_{mode}");
            if (model != null)
                return model;

            model = new PowerBICalendarView();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                return null;
            }

            var random = new Random();
            var colours = new List<string>();

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                if (mode == "-1")
                {
                    //Mode = 0 schedule for all workspaces
                    var pbiSettings = SharedSettingsRepository.Instance.GetSettings(Settings.PortalId);
                    foreach (var s in pbiSettings)
                    {
                        model.Workspaces.Add(s.WorkspaceId);
                    }
                }
                else
                {
                    //Schedule for currect workspace
                    model.Workspaces = new List<string> { Settings.WorkspaceId };
                }

                var groups = client.Groups.GetGroupsAsync().GetAwaiter().GetResult().Value.Value;
                var capacities = client.Capacities.GetCapacitiesAsync().GetAwaiter().GetResult().Value.Value;


                foreach (var workspace in model.Workspaces)
                {
                    IList<Dataset> datasets;
                    //Get Schedule datasets
                    try
                    {
                        datasets = client.Datasets.GetDatasetsInGroupAsync(Guid.Parse(workspace)).GetAwaiter().GetResult().Value.Value.ToList();
                        datasets = CleanUsageDatasets(datasets.ToList());
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Error getting datasets in workspace {workspace}", ex);
                        continue;
                    }


                    var group = groups.FirstOrDefault(g => g.Id.ToString() == workspace.ToLowerInvariant());
                    var capacity = capacities.FirstOrDefault(c => c.Id == group.CapacityId);


                    for (var x = 0; x < datasets.Count; x++)
                    {
                        var dataset = datasets[x];
                        var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
                        colours.Add(color);

                        try
                        {
                            //Get refreshes history
                            var history = client.Datasets.GetRefreshHistoryInGroupAsync(Guid.Parse(Settings.WorkspaceId), dataset.Id, 100).GetAwaiter().GetResult().Value.Value.ToList();

                            foreach (var refresh in history)
                            {
                                model.History.Add(new RefreshedDataset
                                {
                                    Dataset = dataset.Name,
                                    WorkSpaceName = group.Name,
                                    CapacityName = capacity.DisplayName,
                                    StartTime = refresh.StartTime,
                                    EndTime = refresh.EndTime,
                                    RefreshType = refresh.RefreshType,
                                    RequestId = refresh.RequestId,
                                    ServiceExceptionJson = refresh.ServiceExceptionJson,
                                    Status = refresh.Status
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Getting "UnsupportedMediaType"
                            Logger.Warn($"Error getting refresh history for dataset {dataset.Name}", ex);
                        }
                        try
                        {
                            //Get refresh Schedule by dataset and Workspace
                            var schedule = client.Datasets.GetRefreshScheduleInGroupAsync(Guid.Parse(Settings.WorkspaceId), dataset.Id)
                                .GetAwaiter().GetResult().Value;
                            var timeRange = new List<Schedule>();
                            string startHour = schedule.Times[0];
                            string endHour = schedule.Times[0];

                            //Grouping of consecutive hours
                            for (int i = 0; i < schedule.Times.Count; i++)
                            {
                                if (schedule.Times[i] != schedule.Times.Last())
                                {
                                    DateTime hour = DateTime.ParseExact(schedule.Times[i], "HH:mm",
                                        CultureInfo.InvariantCulture);
                                    DateTime nextHour = DateTime.ParseExact(schedule.Times[i + 1], "HH:mm",
                                        CultureInfo.InvariantCulture);
                                    if (nextHour.Hour == hour.Hour + 1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        endHour = hour.AddHours(1).ToString("HH:mm");
                                        timeRange.Add(new Schedule
                                        {
                                            start = startHour,
                                            end = endHour
                                        });
                                        startHour = schedule.Times[i + 1];
                                        endHour = schedule.Times[i + 1];
                                    }
                                }
                                else
                                {
                                    endHour = DateTime.ParseExact(schedule.Times[i], "HH:mm",
                                        CultureInfo.InvariantCulture).AddHours(1).ToString("HH:mm");
                                    timeRange.Add(new Schedule
                                    {
                                        start = startHour,
                                        end = endHour
                                    });
                                }
                            }
                            //Create calendar event by each date and hour group
                            for (var index = 0; index < schedule.Days.Count; index++)
                            {
                                var day = schedule.Days[index];
                                var dayOfWeek = (int)day;
                                for (var i = 0; i < timeRange.Count; i++)
                                {
                                    var time = timeRange[i];
                                    var id = Guid.NewGuid().ToString("N");
                                    var item = new CalendarItem
                                    {
                                        id = id,
                                        color = colours[x < datasets.Count ? x : 0],
                                        start = getCalendarDateTime(dayOfWeek, time.start),
                                        end = getCalendarDateTime(dayOfWeek, time.end),
                                        title = dataset.Name,
                                    };
                                    item.description = $"Workspace: {group.Name}; Capacity: {capacity.DisplayName}; Dataset: {dataset.Name}; Start: {time.start}";
                                    model.RefreshSchedules.Add(item);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Warn("Error getting refresh schedule by dataset and workspace", e);
                            continue;
                        }

                    }
                }
                //Order history
                model.History = model.History.OrderByDescending(dataset => dataset.StartTime).ToList();

                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{Thread.CurrentThread.CurrentUICulture.Name}_CalendarDataSet_{mode}", model, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
            }
            return model;
        }

        private List<Dataset> CleanUsageDatasets(List<Dataset> datasets)
        {
            if (datasets.Any(r => r.Name.Contains("Report Usage Metrics Model")))
            {
                var i = datasets.RemoveAll(r => r.Name.Contains("Report Usage Metrics Model"));
            }
            return datasets;
        }

        private string getCalendarDateTime(int DayOfWeek, string time)
        {
            var baseDate = new DateTime(2006, 1, 2);
            var result = baseDate.AddDays(DayOfWeek);
            result = result.AddHours(int.Parse(time.Substring(0, time.IndexOf(":"))));
            result = result.AddMinutes(int.Parse(time.Substring(time.IndexOf(":") + 1)));

            return result.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private async Task<Capacity> GetCapacityAsync(PowerBIClient client, Guid capacityId)
        {
            if (capacityId == null)
                return null;
            var capacities = (Capacities)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_Capacities");
            if (capacities == null)
            {
                capacities = (await client.Capacities.GetCapacitiesAsync().ConfigureAwait(false)).Value;
                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_Capacities", capacities, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
            }
            return capacities?.Value?.FirstOrDefault(c => c.Id == capacityId);
        }

        private async Task<Group> GetGroupAsync(PowerBIClient client, Guid workspaceId)
        {
            if (workspaceId == null)
                return null;
            var groups = (Groups)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_Groups");
            if (groups == null)
            {
                groups = (await client.Groups.GetGroupsAsync().ConfigureAwait(false)).Value;
                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_Groups", groups, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
            }
            return groups?.Value?.FirstOrDefault(g => g.Id == workspaceId);
        }

        private async Task<bool> ValidateWorkspaceAndCapacity(PowerBIClient client, EmbedConfig model)
        {
            var workspace = await GetGroupAsync(client, Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false);
            if (workspace == null)
            {
                model.ErrorMessage = "No workspace with the given ID was found. Please make sure you have the correct workspace ID.";
            }
            else
            {
                // Check dedicated capacity status
                if (workspace.IsOnDedicatedCapacity.GetValueOrDefault())
                {
                    var capacity = await GetCapacityAsync(client, workspace.CapacityId.GetValueOrDefault()).ConfigureAwait(false);
                    if (capacity == null
                        || (capacity != null
                        && !(capacity.State == CapacityState.Active || capacity.State == CapacityState.UpdatingSku)))
                    {
                        model.IsCapacityDisabled = true;
                        model.ErrorMessage = string.IsNullOrEmpty(Settings.DisabledCapacityMessage)
                            ? model.ErrorMessage = "The workspace is on a dedicated capacity, but the capacity is not active."
                            : Settings.DisabledCapacityMessage;
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<Stream> DownloadReportAsync(Guid workspaceId, Guid reportId)
        {
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                throw new ApplicationException("Can't download report. Authentication failed.");
            }
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                try
                {
                    return (await client.Reports.ExportReportInGroupAsync(workspaceId, reportId, DownloadType.IncludeModel).ConfigureAwait(false)).Value;
                }
                catch (RequestFailedException ex)
                {
                    if (ex.Status == (int)System.Net.HttpStatusCode.BadRequest)
                    {
                        return (await client.Reports.ExportReportInGroupAsync(workspaceId, reportId, DownloadType.LiveConnect).ConfigureAwait(false)).Value;
                    }
                    else
                    {
                        throw;
                    }
                }

            }
        }

        public async Task<Export> GetExportStatusAsync(Guid workspaceId, Guid reportId, string exportId)
        {
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                throw new ApplicationException("Can't export report. Authentication failed.");
            }
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                return (await client.Reports.GetExportToFileStatusInGroupAsync(workspaceId, reportId, exportId).ConfigureAwait(false)).Value;
            }
        }

        public async Task<Stream> GetExportedFileAsync(Guid workspaceId, Guid reportId, string exportId, string resourceFileExtension)
        {
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                throw new ApplicationException("Can't export report. Authentication failed.");
            }
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                return (await client.Reports.GetFileOfExportToFileInGroupAsync(workspaceId, reportId, exportId).ConfigureAwait(false)).Value;
            }
        }

        public async Task<Export> ExportReportAsync(Guid workspaceId, Guid reportId, FileFormat format, 
                string user,
                string roles,
                IList<string> pageNames = null, /* Get the page names from the GetPages REST API */
                string urlFilter = null)
        {
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                throw new ApplicationException("Can't export report. Authentication failed.");
            }            

            List<EffectiveIdentity> identities = null;
            if (!string.IsNullOrWhiteSpace(user))
            {
                // Create a Power BI Client object. It will be used to call Power BI APIs.
                {
                    var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                    Report report = null;
                    var model = new EmbedConfig();
                    if (await ValidateWorkspaceAndCapacity(client, model).ConfigureAwait(false))
                    {
                        // Get a list of reports for the given workspace.
                        var reports = (await client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false)).Value;
                        if (reports.Value.Count() == 0)
                        {
                            throw new ApplicationException("No reports were found in the workspace");
                        }

                        report = reports.Value.FirstOrDefault(r => r.Id.ToString().Equals(reportId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                        if (report == null)
                        {
                            throw new ApplicationException("No report with the given ID was found in the workspace. Make sure ReportId is valid.");
                        }
                    }
                    else
                    {
                        throw new ApplicationException(model.ErrorMessage);
                    }
                    // Check if the dataset has effective identity required
                    // var dataset = await client.Datasets.GetDatasetAsync(report.DatasetId).ConfigureAwait(false);
                    // The line above returns an unauthorization exception when using "Service Principal" credentials. Seems a bug in the PowerBI API.
                    var reportWorkspaceId = Guid.Parse(Settings.WorkspaceId);
                    var datasetWorkspaceId = await GetReportDatasetWorkspaceIdAsync(reportWorkspaceId, report.Id).ConfigureAwait(false)
                                             ?? reportWorkspaceId;
                    Dataset dataset = null;
                    try
                    {
                        dataset = client.Datasets.GetDatasetInGroup(datasetWorkspaceId, report.DatasetId).Value;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Couldn't find dataset '{report.DatasetId}' in workspace '{datasetWorkspaceId}'", ex);
                        dataset = client.Datasets.GetDatasetsInGroup(datasetWorkspaceId).Value.Value.FirstOrDefault(x => x.Id == report.DatasetId);
                    }

                    if (dataset != null
                        && (dataset.IsEffectiveIdentityRequired.GetValueOrDefault(false) || dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false)))
                    //&& !dataset.IsOnPremGatewayRequired.GetValueOrDefault(false))
                    {
                        var rls = new EffectiveIdentity { Username = user };
                        rls.Datasets.Add(report.DatasetId);
                        if (!string.IsNullOrWhiteSpace(roles) && dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false))
                        {
                            foreach (var role in roles.Split(','))
                            {
                                rls.Roles.Add(role);
                            }
                        }
                        identities = new List<EffectiveIdentity> { rls };
                    }
                }
            }

            var powerBIReportExportConfiguration = new PowerBIReportExportConfiguration
            {
                Settings = new ExportReportSettings
                {
                    Locale = "en-us",
                },
            };
            if (pageNames != null)
            {
                foreach (var pn in pageNames)
                {
                    powerBIReportExportConfiguration.Pages.Add(new ExportReportPage(pageName: pn));
                }
            }
            if (!string.IsNullOrEmpty(urlFilter))
            {
                var filter = new ExportFilter { Filter = urlFilter };
                powerBIReportExportConfiguration.ReportLevelFilters.Add(filter);
            }
            if (identities != null)
            {
                foreach (var identity in identities)
                {
                    powerBIReportExportConfiguration.Identities.Add(identity);
                }
            }

            var exportRequest = new ExportReportRequest(format)
            {
                PowerBIReportConfiguration = powerBIReportExportConfiguration,
            };

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                return (await client.Reports.ExportToFileInGroupAsync(workspaceId, reportId, exportRequest).ConfigureAwait(false)).Value;
            }
        }

        public async Task<EmbedConfig> GetReportEmbedConfigAsync(int userId, string username, string roles, string reportId, bool hasEditPermission)
        {
            var model = (EmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{username}_{roles}_{Thread.CurrentThread.CurrentUICulture.Name}_Report_{reportId}");
            if (model != null)
                return model;

            model = new EmbedConfig();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }


            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                if (await ValidateWorkspaceAndCapacity(client, model).ConfigureAwait(false))
                {
                    // Get a list of reports for the given workspace.
                    var reports = (await client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false)).Value;
                    if (reports.Value.Count() == 0)
                    {
                        model.ErrorMessage = "No reports were found in the workspace";
                    }

                    Report report = reports.Value.FirstOrDefault(r => r.Id.ToString().Equals(reportId, StringComparison.InvariantCultureIgnoreCase));
                    if (report == null)
                    {
                        model.ErrorMessage = "No report with the given ID was found in the workspace. Make sure ReportId is valid.";
                    }
                    else
                    {
                        model.EmbedToken = await GenerateTokenAsync(username, roles, client, report, hasEditPermission).ConfigureAwait(false);
                        model.EmbedUrl = report?.EmbedUrl;
                        model.Id = string.IsNullOrEmpty(report?.Id.ToString()) ? reportId : report?.Id.ToString();
                        model.ReportType = report?.ReportType?.ToString();
                        if (model.EmbedToken == null)
                        {
                            model.ErrorMessage = "Failed to generate embed token.";
                        }
                    }
                }
                model.ContentType = "report";
                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{username}_{roles}_{Thread.CurrentThread.CurrentUICulture.Name}_Report_{reportId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;
        }

        private async Task<EmbedToken> GenerateTokenAsync(string username, string roles, PowerBIClient client, Report report, bool hasEditPermission)
        {

            try
            {
                var reportWorkspaceId = Guid.Parse(Settings.WorkspaceId);

                // If the report uses a shared dataset that lives in a different workspace,
                // the dataset must be looked up in its own workspace. The SDK Report model in
                // Microsoft.PowerBI.Api 5.x does not surface DatasetWorkspaceId, so fetch it via REST.
                var datasetWorkspaceId = await GetReportDatasetWorkspaceIdAsync(reportWorkspaceId, report.Id).ConfigureAwait(false)
                                         ?? reportWorkspaceId;

                // Discover chained datasets (composite models / DirectQuery to Power BI dataset).
                // If the report's primary dataset depends on other Power BI datasets in different
                // workspaces, all of them must be declared in the embed token.
                var chainedDatasets = await GetChainedDatasetsAsync(client, datasetWorkspaceId, report.DatasetId).ConfigureAwait(false);

                bool isCrossWorkspaceDataset = datasetWorkspaceId != reportWorkspaceId;
                bool useV2 = isCrossWorkspaceDataset || chainedDatasets.Count > 0;

                EffectiveIdentity rls = null;
                string permission = hasEditPermission ? "edit" : "view";
                if (!string.IsNullOrWhiteSpace(username))
                {
                    // Check if the dataset has effective identity required
                    // var dataset = await client.Datasets.GetDatasetAsync(report.DatasetId).ConfigureAwait(false);
                    // The line above returns an unauthorization exception when using "Service Principal" credentials. Seems a bug in the PowerBI API.
                    Dataset dataset = null;
                    try
                    {
                        dataset = client.Datasets.GetDatasetInGroup(datasetWorkspaceId, report.DatasetId).Value;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Couldn't find dataset '{report.DatasetId}' in workspace '{datasetWorkspaceId}'", ex);
                        dataset = client.Datasets.GetDatasetsInGroup(datasetWorkspaceId).Value.Value.FirstOrDefault(x => x.Id == report.DatasetId);
                    }

                    // RLS may be required either by the primary dataset or by any chained (remote) dataset.
                    bool chainedNeedsIdentity = chainedDatasets.Any(c => c.IsEffectiveIdentityRequired || c.IsEffectiveIdentityRolesRequired);
                    bool primaryNeedsIdentity = dataset != null
                        && (dataset.IsEffectiveIdentityRequired.GetValueOrDefault(false) || dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false));

                    if (primaryNeedsIdentity || chainedNeedsIdentity)
                    //&& !dataset.IsOnPremGatewayRequired.GetValueOrDefault(false))
                    {
                        rls = new EffectiveIdentity { Username = username };
                        // Only attach the identity to the datasets that actually require it.
                        // Listing datasets without RLS here makes Power BI try to resolve the
                        // identity against them via MSOLAP and fail with errors such as
                        // "Failed to open the MSOLAP connection".
                        if (primaryNeedsIdentity)
                        {
                            rls.Datasets.Add(report.DatasetId);
                        }
                        foreach (var chained in chainedDatasets)
                        {
                            if ((chained.IsEffectiveIdentityRequired || chained.IsEffectiveIdentityRolesRequired)
                                && !rls.Datasets.Contains(chained.DatasetId))
                            {
                                rls.Datasets.Add(chained.DatasetId);
                            }
                        }
                        bool rolesRequiredAnywhere = (dataset != null && dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false))
                                                     || chainedDatasets.Any(c => c.IsEffectiveIdentityRolesRequired);
                        if (!string.IsNullOrWhiteSpace(roles) && rolesRequiredAnywhere)
                        {
                            foreach (var role in roles.Split(','))
                            {
                                rls.Roles.Add(role);
                            }
                        }
                    }
                }

                EmbedToken tokenResponse;
                if (useV2)
                {
                    // Cross-workspace report/dataset or composite model with chained datasets:
                    // must use the multi-resource embed token endpoint (POST /v1.0/myorg/GenerateToken)
                    // declaring every dataset and every workspace involved.
                    var v2Request = new GenerateTokenRequestV2();
                    // Composite models / DirectQuery to Power BI datasets require XMLA read access
                    // on every dataset; without this the service returns
                    // "Cannot connect to dataset ... because XMLA permissions are off".
                    var xmlaPermissions = chainedDatasets.Count > 0
                        ? (XmlaPermissions?)XmlaPermissions.ReadOnly
                        : null;
                    v2Request.Datasets.Add(new GenerateTokenRequestV2Dataset(report.DatasetId) { XmlaPermissions = xmlaPermissions });
                    foreach (var chained in chainedDatasets)
                    {
                        if (!v2Request.Datasets.Any(d => d.Id == chained.DatasetId))
                            v2Request.Datasets.Add(new GenerateTokenRequestV2Dataset(chained.DatasetId) { XmlaPermissions = xmlaPermissions });
                    }
                    v2Request.Reports.Add(new GenerateTokenRequestV2Report(report.Id) { AllowEdit = hasEditPermission });

                    var targetWorkspaces = new HashSet<Guid> { reportWorkspaceId, datasetWorkspaceId };
                    foreach (var chained in chainedDatasets)
                    {
                        targetWorkspaces.Add(chained.WorkspaceId);
                    }
                    foreach (var ws in targetWorkspaces)
                    {
                        v2Request.TargetWorkspaces.Add(new GenerateTokenRequestV2TargetWorkspace(ws));
                    }
                    if (rls != null)
                    {
                        v2Request.Identities.Add(rls);
                    }
                    tokenResponse = (await client.EmbedToken.GenerateTokenAsync(v2Request).ConfigureAwait(false)).Value;
                }
                else
                {
                    var generateTokenRequestParameters = new GenerateTokenRequest { AccessLevel = ToAccessLevel(permission) };
                    if (rls != null)
                    {
                        generateTokenRequestParameters.Identities.Add(rls);
                    }
                    tokenResponse = (await client.Reports.GenerateTokenInGroupAsync(reportWorkspaceId, report.Id, generateTokenRequestParameters).ConfigureAwait(false)).Value;
                }
                return tokenResponse;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        private class ChainedDatasetInfo
        {
            public Guid WorkspaceId { get; set; }
            public string DatasetId { get; set; }
            public bool IsEffectiveIdentityRequired { get; set; }
            public bool IsEffectiveIdentityRolesRequired { get; set; }
        }

        /// <summary>
        /// Returns the list of Power BI datasets that the given dataset depends on via DirectQuery
        /// (composite model / "DirectQuery to Power BI dataset"). Each remote dataset and its
        /// workspace must be declared when generating the embed token; otherwise visuals fail with
        /// errors like "OnPremiseServiceException".
        /// </summary>
        private async Task<List<ChainedDatasetInfo>> GetChainedDatasetsAsync(PowerBIClient client, Guid datasetWorkspaceId, string datasetId)
        {
            var cacheKey = $"PBI_{Settings.PortalId}_{Settings.SettingsId}_ChainedDatasets_{datasetWorkspaceId}_{datasetId}";
            var cached = CachingProvider.Instance().GetItem(cacheKey) as List<ChainedDatasetInfo>;
            if (cached != null)
                return cached;

            var result = new List<ChainedDatasetInfo>();
            try
            {
                var apiUrl = (Settings.ApiUrl ?? "https://api.powerbi.com").TrimEnd('/');
                var requestUrl = $"{apiUrl}/v1.0/myorg/groups/{datasetWorkspaceId}/datasets/{datasetId}/datasources";
                JObject root;
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await http.GetAsync(requestUrl).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Warn($"GetDatasources REST call returned {response.StatusCode} for dataset {datasetId} in workspace {datasetWorkspaceId}");
                        CachingProvider.Instance().Insert(cacheKey, result, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
                        return result;
                    }
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    root = JObject.Parse(json);
                }

                var datasources = root["value"] as JArray;
                if (datasources == null)
                {
                    CachingProvider.Instance().Insert(cacheKey, result, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
                    return result;
                }

                // Lazy-resolved caches scoped to this call
                IReadOnlyList<Group> groupsList = null;
                var datasetsByWorkspace = new Dictionary<Guid, IReadOnlyList<Dataset>>();

                foreach (var ds in datasources)
                {
                    var dsType = (string)ds["datasourceType"];
                    var conn = ds["connectionDetails"];
                    if (conn == null)
                        continue;

                    var server = (string)conn["server"];
                    var database = (string)conn["database"];
                    if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
                        continue;

                    // Chained Power BI dataset connections look like:
                    //   server   = "powerbi://api.powerbi.com/v1.0/myorg/<WorkspaceName>"
                    //   database = "<DatasetName>" (sometimes a GUID)
                    if (!server.StartsWith("powerbi://", StringComparison.OrdinalIgnoreCase)
                        && !(dsType != null && dsType.Equals("AnalysisServices", StringComparison.OrdinalIgnoreCase)
                             && server.IndexOf("powerbi", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        continue;
                    }

                    var workspaceName = ExtractChainedWorkspaceName(server);
                    if (string.IsNullOrEmpty(workspaceName))
                        continue;

                    if (groupsList == null)
                    {
                        groupsList = (await client.Groups.GetGroupsAsync().ConfigureAwait(false)).Value.Value;
                    }
                    var remoteGroup = groupsList.FirstOrDefault(g => string.Equals(g.Name, workspaceName, StringComparison.OrdinalIgnoreCase));
                    if (remoteGroup == null)
                    {
                        Logger.Warn($"Chained dataset references workspace '{workspaceName}' which is not visible to the current credentials.");
                        continue;
                    }

                    if (!datasetsByWorkspace.TryGetValue(remoteGroup.Id, out var remoteDatasets))
                    {
                        remoteDatasets = (await client.Datasets.GetDatasetsInGroupAsync(remoteGroup.Id).ConfigureAwait(false)).Value.Value;
                        datasetsByWorkspace[remoteGroup.Id] = remoteDatasets;
                    }

                    Dataset remoteDataset = null;
                    if (Guid.TryParse(database, out _))
                    {
                        remoteDataset = remoteDatasets.FirstOrDefault(d => string.Equals(d.Id, database, StringComparison.OrdinalIgnoreCase));
                    }
                    if (remoteDataset == null)
                    {
                        remoteDataset = remoteDatasets.FirstOrDefault(d => string.Equals(d.Name, database, StringComparison.OrdinalIgnoreCase));
                    }
                    if (remoteDataset == null)
                    {
                        Logger.Warn($"Chained dataset '{database}' was not found in workspace '{workspaceName}' ({remoteGroup.Id}).");
                        continue;
                    }

                    if (result.Any(c => c.DatasetId == remoteDataset.Id))
                        continue;

                    result.Add(new ChainedDatasetInfo
                    {
                        WorkspaceId = remoteGroup.Id,
                        DatasetId = remoteDataset.Id,
                        IsEffectiveIdentityRequired = remoteDataset.IsEffectiveIdentityRequired.GetValueOrDefault(false),
                        IsEffectiveIdentityRolesRequired = remoteDataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false)
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error retrieving chained datasets for dataset {datasetId} in workspace {datasetWorkspaceId}", ex);
            }

            CachingProvider.Instance().Insert(cacheKey, result, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
            return result;
        }

        private static string ExtractChainedWorkspaceName(string server)
        {
            // Expected formats:
            //   powerbi://api.powerbi.com/v1.0/myorg/<WorkspaceName>
            //   powerbi://api.powerbi.com/v1.0/myorg/<WorkspaceName>/
            const string marker = "/myorg/";
            var idx = server.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return null;
            var name = server.Substring(idx + marker.Length).Trim('/');
            if (string.IsNullOrEmpty(name))
                return null;
            try
            {
                name = Uri.UnescapeDataString(name);
            }
            catch
            {
                // Leave as-is if not a valid escape sequence
            }
            return name;
        }

        private async Task<Guid?> GetReportDatasetWorkspaceIdAsync(Guid reportWorkspaceId, Guid reportId)
        {
            var cacheKey = $"PBI_{Settings.PortalId}_{Settings.SettingsId}_DatasetWorkspaceId_{reportWorkspaceId}_{reportId}";
            var cached = CachingProvider.Instance().GetItem(cacheKey);
            if (cached is Guid cachedGuid)
                return cachedGuid;
            if (cached is string cachedString && cachedString == "none")
                return null;

            try
            {
                var apiUrl = (Settings.ApiUrl ?? "https://api.powerbi.com").TrimEnd('/');
                var requestUrl = $"{apiUrl}/v1.0/myorg/groups/{reportWorkspaceId}/reports/{reportId}";
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await http.GetAsync(requestUrl).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Warn($"GetReport REST call returned {response.StatusCode} for report {reportId} in workspace {reportWorkspaceId}");
                        return null;
                    }
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var token = JObject.Parse(json);
                    var dsWsId = (string)token["datasetWorkspaceId"];
                    if (!string.IsNullOrEmpty(dsWsId) && Guid.TryParse(dsWsId, out var parsed))
                    {
                        CachingProvider.Instance().Insert(cacheKey, parsed, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
                        return parsed;
                    }
                    CachingProvider.Instance().Insert(cacheKey, "none", null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error retrieving datasetWorkspaceId for report {reportId} in workspace {reportWorkspaceId}", ex);
                return null;
            }
        }

        public async Task<Pages> GetReportPages(string reportId)
        {
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                return null;
            }

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                // Get a list of reports for the given workspace.
                var reports = (await client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false)).Value;
                if (reports.Value.Count() == 0)
                {
                    return null;
                }

                Report report = reports.Value.FirstOrDefault(r => r.Id.ToString().Equals(reportId, StringComparison.InvariantCultureIgnoreCase));
                if (report == null)
                {
                    return null;
                }

                var pages = (await client.Reports.GetPagesInGroupAsync(Guid.Parse(Settings.WorkspaceId), report.Id).ConfigureAwait(false)).Value;
                return pages;
            }
        }

        public async Task<EmbedConfig> GetDashboardEmbedConfigAsync(int userId, string username, string roles, string dashboardId, bool hasEditPermission)
        {
            string permission = hasEditPermission ? "edit" : "view";

            var model = (EmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{username}_{roles}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}");
            if (model != null)
                return model;

            model = new EmbedConfig();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                if (await ValidateWorkspaceAndCapacity(client, model).ConfigureAwait(false))
                {
                    // Get a list of reports for the given workspace.
                    var dashboards = (await client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false)).Value;
                    if (dashboards.Value.Count() == 0)
                    {
                        model.ErrorMessage = "No dashboards were found in the workspace";
                    }
                    var dashboard = dashboards.Value.FirstOrDefault(r => r.Id.ToString().Equals(dashboardId, StringComparison.InvariantCultureIgnoreCase));
                    if (dashboard == null)
                    {
                        model.ErrorMessage = "No dashboard with the given ID was found in the workspace. Make sure ReportId is valid.";
                    }
                    // Generate Embed Token for reports without effective identities.
                    GenerateTokenRequest generateTokenRequestParameters;
                    // This is how you create embed token with effective identities
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        var rls = new EffectiveIdentity { Username = username };
                        rls.Datasets.Add(dashboardId);
                        if (!string.IsNullOrWhiteSpace(roles))
                        {
                            foreach (var role in roles.Split(','))
                            {
                                rls.Roles.Add(role);
                            }
                        }
                        if (Components.Common.IsSuperUser())
                        {
                            rls.Roles.Add("SuperUsers");
                        }
                        // Generate Embed Token with effective identities.
                        generateTokenRequestParameters = new GenerateTokenRequest { AccessLevel = ToAccessLevel(permission) };
                        generateTokenRequestParameters.Identities.Add(rls);
                    }
                    else
                    {
                        // Generate Embed Token for reports without effective identities.
                        generateTokenRequestParameters = new GenerateTokenRequest { AccessLevel = ToAccessLevel(permission) };
                    }
                    EmbedToken tokenResponse;
                    try
                    {
                        tokenResponse = (await client.Dashboards.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, generateTokenRequestParameters).ConfigureAwait(false)).Value;
                    }
                    catch (RequestFailedException ex)
                    {
                        if (ex.Message.Contains("shouldn't have effective identity"))
                        {
                            // HACK: Creating embed token for accessing dataset shouldn't have effective identity"
                            // See https://community.powerbi.com/t5/Developer/quot-shouldn-t-have-effective-identity-quot-error-when-passing/m-p/437177
                            generateTokenRequestParameters = new GenerateTokenRequest { AccessLevel = ToAccessLevel(permission) };

                            tokenResponse = (await client.Dashboards.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, generateTokenRequestParameters).ConfigureAwait(false)).Value;
                        }
                        else
                            throw;
                    }
                    if (tokenResponse == null)
                    {
                        model.ErrorMessage = "Failed to generate embed token.";
                    }
                    // Generate Embed Configuration.
                    model.EmbedToken = tokenResponse;
                    model.EmbedUrl = dashboard.EmbedUrl;
                    model.Id = dashboard.Id.ToString();
                }
                model.ContentType = "dashboard";

                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{username}_{roles}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;

        }

        public async Task<TileEmbedConfig> GetTileEmbedConfigAsync(int userId, string tileId, string dashboardId, bool hasEditPermission)
        {
            var model = (TileEmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}_Tile_{tileId}");
            if (model != null)
                return model;

            model = new TileEmbedConfig();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            {
                var client = new PowerBIClient(accessToken, new Uri(Settings.ApiUrl));
                string permission = hasEditPermission ? "edit" : "view";

                // Get a list of dashboards.
                var dashboards = (await client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false)).Value;

                // Get the first report in the workspace.
                var dashboard = dashboards.Value.FirstOrDefault(r => r.Id.ToString().Equals(dashboardId, StringComparison.InvariantCultureIgnoreCase));
                if (dashboard == null)
                {
                    tileEmbedConfig.ErrorMessage = "Workspace has no dashboards.";
                    return model;
                }
                var tiles = (await client.Dashboards.GetTilesInGroupAsync(Guid.Parse(Settings.WorkspaceId), Guid.Parse(dashboardId)).ConfigureAwait(false)).Value;
                // Get the first tile in the workspace.
                var tile = tiles.Value.FirstOrDefault(x => x.Id.ToString() == tileId);
                // Generate Embed Token for a tile.
                var generateTokenRequestParameters = new GenerateTokenRequest { AccessLevel = ToAccessLevel(permission) };

                var tokenResponse = (await client.Tiles.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, tile.Id, generateTokenRequestParameters).ConfigureAwait(false)).Value;
                if (tokenResponse == null)
                {
                    tileEmbedConfig.ErrorMessage = "Failed to generate embed token.";
                    return model;
                }

                // Generate Embed Configuration.
                tileEmbedConfig = new TileEmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = tile.EmbedUrl,
                    Id = tile.Id.ToString(),
                    dashboardId = dashboard.Id.ToString(),
                    ContentType = "tile"
                };
                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}_Tile_{tileId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;
        }

        /// <summary>
        /// Check if web.config embed parameters have valid values.
        /// </summary>
        /// <returns>Null if web.config parameters are valid, otherwise returns specific error string.</returns>
        private string ValidateSettings()
        {
            // Application Id must have a value.
            if (string.IsNullOrWhiteSpace(Settings.ApplicationId))
            {
                return "ApplicationId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Application Id must be a Guid object.
            Guid result;
            if (!Guid.TryParse(Settings.ApplicationId, out result))
            {
                return "ApplicationId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill application Id in web.config.";
            }

            // Workspace Id must have a value.
            if (string.IsNullOrWhiteSpace(Settings.WorkspaceId))
            {
                return "WorkspaceId is empty. Please select a group you own and fill its Id in web.config";
            }

            // Workspace Id must be a Guid object.
            if (!Guid.TryParse(Settings.WorkspaceId, out result))
            {
                return "WorkspaceId must be a Guid object. Please select a workspace you own and fill its Id in web.config";
            }

            if (Settings.AuthenticationType.Equals("MasterUser"))
            {
                // Username must have a value.
                if (string.IsNullOrWhiteSpace(Settings.Username))
                {
                    return "Username is empty. Please fill Power BI username in web.config";
                }

                // Password must have a value.
                if (string.IsNullOrWhiteSpace(Settings.Password))
                {
                    return "Password is empty. Please fill password of Power BI username in web.config";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalApplicationId))
                {
                    return "Service Principal ApplicationId is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalApplicationSecret))
                {
                    return "ApplicationSecret is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                // Must fill tenant Id
                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalTenant))
                {
                    return "Invalid Tenant. Please fill Tenant ID in Tenant under web.config";
                }
            }
            return null;
        }

        private async Task<AuthenticationResult> DoAuthenticationAsync()
        {
            AuthenticationResult authenticationResult = null;
            if (Settings.AuthenticationType.Equals("MasterUser"))
            {
                var authenticationContext = new AuthenticationContext(Settings.AuthorityUrl);

                // Authentication using master user credentials
                var credential = new UserPasswordCredential(Settings.Username, Settings.Password);
                authenticationResult = authenticationContext.AcquireTokenAsync(Settings.ResourceUrl, Settings.ApplicationId, credential).Result;
            }
            else
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificURL = Settings.AuthorityUrl.Replace("common", Settings.ServicePrincipalTenant);
                var authenticationContext = new AuthenticationContext(tenantSpecificURL);

                // Authentication using app credentials
                var credential = new ClientCredential(Settings.ServicePrincipalApplicationId, Settings.ServicePrincipalApplicationSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(Settings.ResourceUrl, credential).Result;
            }
            await Task.CompletedTask;

            return authenticationResult;
        }

        private static TokenAccessLevel ToAccessLevel(string permission)
        {
            return string.Equals(permission, "edit", StringComparison.OrdinalIgnoreCase)
                ? TokenAccessLevel.Edit
                : TokenAccessLevel.View;
        }

        private async Task<bool> GetTokenCredentials()
        {
            // var result = new EmbedConfig { Username = username, Roles = roles };
            var error = ValidateSettings();
            if (error != null)
            {
                Logger.Error($"Error in GetTokenCredentials: {error}");
                embedConfig.ErrorMessage = error;
                return false;
            }

            accessToken = (string)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_TokenCredentials");
            if (!string.IsNullOrEmpty(accessToken))
                return true;

            // Authenticate using created credentials
            AuthenticationResult authenticationResult = null;
            try
            {
                authenticationResult = await DoAuthenticationAsync().ConfigureAwait(false);
            }
            catch (AggregateException exc)
            {
                embedConfig.ErrorMessage = exc.InnerException.Message;
                Logger.Error(embedConfig.ErrorMessage);
                return false;
            }

            if (authenticationResult == null)
            {
                embedConfig.ErrorMessage = "Authentication Failed.";
                return false;
            }

            accessToken = authenticationResult.AccessToken;
            CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_TokenCredentials", accessToken, null, authenticationResult.ExpiresOn.AddMinutes(-2).UtcDateTime, TimeSpan.Zero);
            return true;
        }

    }
}