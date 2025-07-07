using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Components;
using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Services.Scheduling;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Tasks
{
    public class CapacityRuleTask : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(CapacityRuleTask));

        public CapacityRuleTask(ScheduleHistoryItem item) : base()
        {
            this.ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            try
            {
                this.ScheduleHistoryItem.AddLogNote("Starting capacity rule evaluation");
                DoWorkAsync().Wait();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in capacity rule evaluation");
            }
        }

        private async Task DoWorkAsync()
        {
            var common = new Common();
            var capacityManagementService = new CapacityManagementService();
            var settings = SharedSettingsRepository.Instance.GetAllSettings();

                foreach (var setting in settings.AsParallel())
                {
                    try
                    {
                        // Skip if Azure Management API credentials are not configured
                        if (string.IsNullOrEmpty(setting.AzureManagementSubscriptionId) ||
                            string.IsNullOrEmpty(setting.AzureManagementResourceGroup) ||
                            string.IsNullOrEmpty(setting.AzureManagementCapacityName) ||
                            string.IsNullOrEmpty(setting.AzureManagementClientId) ||
                            string.IsNullOrEmpty(setting.AzureManagementClientSecret) ||
                            string.IsNullOrEmpty(setting.AzureManagementTenantId))
                        {
                            continue;
                        }

                        var rules = CapacityRulesRepository.Instance.GetRulesBySettingsId(setting.SettingsId, setting.PortalId);
                        var activeRules = rules.Where(r => r.IsEnabled && !r.IsDeleted).ToList();

                        this.ScheduleHistoryItem.AddLogNote($"Processing {activeRules.Count} active rules for settings {setting.SettingsId}");

                        foreach (var rule in activeRules)
                        {
                            try
                            {
                                if (IsRuleDue(rule))
                                {
                                    this.ScheduleHistoryItem.AddLogNote($"Executing rule: {rule.RuleName}");
                                    
                                    var success = false;
                                    if (rule.Action.Equals("Start", StringComparison.OrdinalIgnoreCase))
                                    {
                                        success = await capacityManagementService.StartCapacityAsync(
                                            setting.AzureManagementSubscriptionId,
                                            setting.AzureManagementResourceGroup,
                                            setting.AzureManagementCapacityName,
                                            setting.AzureManagementClientId,
                                            setting.AzureManagementClientSecret,
                                            setting.AzureManagementTenantId);
                                    }
                                    else if (rule.Action.Equals("Stop", StringComparison.OrdinalIgnoreCase))
                                    {
                                        success = await capacityManagementService.PauseCapacityAsync(
                                            setting.AzureManagementSubscriptionId,
                                            setting.AzureManagementResourceGroup,
                                            setting.AzureManagementCapacityName,
                                            setting.AzureManagementClientId,
                                            setting.AzureManagementClientSecret,
                                            setting.AzureManagementTenantId);
                                    }

                                    if (success)
                                    {
                                        rule.LastExecutedOn = DateTime.Now;
                                        CapacityRulesRepository.Instance.UpdateRule(rule);
                                        this.ScheduleHistoryItem.AddLogNote($"Successfully executed rule: {rule.RuleName}");
                                    }
                                    else
                                    {
                                        this.ScheduleHistoryItem.AddLogNote($"Failed to execute rule: {rule.RuleName}");
                                    }
                                }
                            }
                            catch (Exception ruleEx)
                            {
                                HandleRuleException(ruleEx, rule);
                            }
                        }
                    }
                    catch (Exception settingEx)
                    {
                        HandleException(settingEx, $"Error processing settings {setting.SettingsId}");
                    }
                }

            this.ScheduleHistoryItem.AddLogNote("Completed capacity rule evaluation");
            this.ScheduleHistoryItem.Succeeded = true;

        private bool IsRuleDue(CapacityRule rule)
        {
            try
            {
                var currentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, rule.TimeZoneId);
                var currentTime = currentDateTime.TimeOfDay;
                var currentDayOfWeek = (int)currentDateTime.DayOfWeek;

                // Check if today is included in the rule's days of week
                if (!string.IsNullOrEmpty(rule.DaysOfWeek))
                {
                    var daysOfWeek = rule.DaysOfWeek.Split(',').Select(d => int.Parse(d.Trim())).ToList();
                    if (!daysOfWeek.Contains(currentDayOfWeek))
                    {
                        return false;
                    }
                }

                // Check if it's time to execute the rule
                var timeDifference = Math.Abs((currentTime - rule.ExecutionTime).TotalMinutes);
                if (timeDifference > 5) // Allow 5 minutes tolerance
                {
                    return false;
                }

                // Check if the rule was already executed today
                if (rule.LastExecutedOn.HasValue)
                {
                    var lastExecutedDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(rule.LastExecutedOn.Value, rule.TimeZoneId).Date;
                    if (lastExecutedDate == currentDateTime.Date)
                    {
                        return false; // Already executed today
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking if rule {rule.RuleName} is due", ex);
                return false;
            }
        }

        private void HandleRuleException(Exception ex, CapacityRule rule)
        {
            Logger.Error($"Error executing rule '{rule.RuleName}': {ex.Message}", ex);
            var errorMessage = $"Error executing rule '{rule.RuleName}': {ex.Message}";
            this.ScheduleHistoryItem.AddLogNote(errorMessage);
        }

        private void HandleException(Exception ex, string context)
        {
            Logger.Error($"{context}: {ex.Message}", ex);
            this.ScheduleHistoryItem.AddLogNote($"{context}: {ex.Message}");
            this.ScheduleHistoryItem.Succeeded = false;
        }
    }
}