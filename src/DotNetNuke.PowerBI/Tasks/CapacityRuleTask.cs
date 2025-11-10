using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.PowerBI.Services.Models;
using DotNetNuke.Services.Scheduling;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

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
            var capacityManagementService = new CapacityManagementService();
            var settings = SharedSettingsRepository.Instance.GetAllSettings();

            foreach (var setting in settings.AsParallel())
            {
                try
                {
                    // Skip if Azure Management API basic configuration is not present
                    if (string.IsNullOrEmpty(setting.AzureManagementSubscriptionId) ||
                        string.IsNullOrEmpty(setting.AzureManagementResourceGroup) ||
                        string.IsNullOrEmpty(setting.AzureManagementCapacityName))
                    {
                        continue;
                    }

                    // Validate authentication configuration based on AuthenticationType
                    if (!ValidateAuthenticationConfiguration(setting))
                    {
                        this.ScheduleHistoryItem.AddLogNote($"Skipping settings {setting.SettingsId}: Invalid authentication configuration for {setting.AuthenticationType}");
                        continue;
                    }

                    var rules = CapacityRulesRepository.Instance.GetRulesBySettingsId(setting.SettingsId, setting.PortalId);
                    var activeRules = rules.Where(r => r.IsEnabled && !r.IsDeleted).ToList();

                    if (activeRules.Count == 0)
                    {
                        continue; // No active rules, skip logging
                    }

                    this.ScheduleHistoryItem.AddLogNote($"Processing {activeRules.Count} active rules for settings {setting.SettingsId} ({setting.SettingsGroupName}) using {setting.AuthenticationType}");

                    foreach (var rule in activeRules)
                    {
                        try
                        {
                            if (IsRuleDue(rule) == false)
                            {
                                this.ScheduleHistoryItem.AddLogNote($"Skipping rule: {rule.RuleName}, not due for execution");
                                continue;
                            }

                            this.ScheduleHistoryItem.AddLogNote($"Executing rule: {rule.RuleName} (Action: {rule.Action})");

                            AzureCapacity capacity = await capacityManagementService.GetCapacityStatusAsync(setting);
                            if (capacity == null)
                                continue;

                            var success = false;
                            if (rule.Action.Equals("Start", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsServiceActive(capacity.State))
                                    this.ScheduleHistoryItem.AddLogNote($"Service is already running, for rule: {rule.RuleName}");
                                else
                                    success = await capacityManagementService.StartCapacityAsync(setting);
                            }
                            else if (rule.Action.Equals("Stop", StringComparison.OrdinalIgnoreCase)
                                || rule.Action.Equals("Pause", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsServiceStopped(capacity.State))
                                    this.ScheduleHistoryItem.AddLogNote($"Service is already stopped, for rule: {rule.RuleName}");
                                else
                                    success = await capacityManagementService.PauseCapacityAsync(setting);
                            }
                            else
                            {
                                this.ScheduleHistoryItem.AddLogNote($"Unknown action '{rule.Action}' for rule: {rule.RuleName}");
                                continue;
                            }

                            if (success)
                            {
                                rule.LastExecutedOn = DateTime.UtcNow;
                                CapacityRulesRepository.Instance.UpdateRule(rule);
                                this.ScheduleHistoryItem.AddLogNote($"Successfully executed rule: {rule.RuleName}");
                            }
                            else
                            {
                                this.ScheduleHistoryItem.AddLogNote($"Failed to execute rule: {rule.RuleName}");
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
                    HandleException(settingEx, $"Error processing settings {setting.SettingsId} ({setting.SettingsGroupName})");
                }
            }

            this.ScheduleHistoryItem.AddLogNote("Completed capacity rule evaluation");
            this.ScheduleHistoryItem.Succeeded = true;
        }

        /// <summary>
        /// Validates authentication configuration based on the AuthenticationType
        /// </summary>
        private bool ValidateAuthenticationConfiguration(Data.Models.PowerBISettings setting)
        {
            if (setting.AuthenticationType.Equals("MasterUser", StringComparison.OrdinalIgnoreCase))
            {
                // For MasterUser, we need Username and Password
                if (string.IsNullOrEmpty(setting.Username) || string.IsNullOrEmpty(setting.Password))
                {
                    return false;
                }
                // ClientId can come from ApplicationId or AzureManagementClientId
                if (string.IsNullOrEmpty(setting.ApplicationId) && string.IsNullOrEmpty(setting.ServicePrincipalApplicationId))
                {
                    return false;
                }
            }
            else // ServicePrincipal
            {
                // For ServicePrincipal, we need ClientId, ClientSecret and TenantId
                if (string.IsNullOrEmpty(setting.ServicePrincipalApplicationId) ||
                    string.IsNullOrEmpty(setting.ServicePrincipalApplicationSecret) ||
                    string.IsNullOrEmpty(setting.ServicePrincipalTenant))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a rule should be executed based on its schedule
        /// </summary>
        private bool IsRuleDue(CapacityRule rule)
        {
            try
            {
                DateTime currentDateTime = IanaToWindowsDateTime(rule.TimeZoneId);
                TimeSpan currentTime = currentDateTime.TimeOfDay;
                int currentDayOfWeek = (int)currentDateTime.DayOfWeek;

                // Check if today is included in the rule's days of week
                if (!string.IsNullOrEmpty(rule.DaysOfWeek))
                {
                    var daysOfWeek = rule.DaysOfWeek.Split(',').Select(d => int.Parse(d.Trim())).ToList();
                    if (!daysOfWeek.Contains(currentDayOfWeek))
                    {
                        return false;
                    }
                }

                // Check if it's time to execute the rule (with 5 minutes tolerance)
                double timeDifference = Math.Abs((currentTime - rule.ExecutionTime).TotalMinutes);
                if (timeDifference > 5)
                {
                    return false;
                }

                // Check if the rule was already executed recently (within the last hour to prevent duplicates)
                if (rule.LastExecutedOn.HasValue)
                {
                    var timeSinceLastExecution = DateTime.UtcNow - rule.LastExecutedOn.Value;
                    if (timeSinceLastExecution.TotalMinutes < 60)
                    {
                        return false; // Already executed within the last hour
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

        private DateTime IanaToWindowsDateTime(string ianaTimeZone)
        {
            string windowsZone = TZConvert.IanaToWindows(ianaTimeZone);
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(windowsZone);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }

        private bool IsServiceActive(CapacityState currentCapacityState)
        {
            return currentCapacityState == CapacityState.Active;
        }

        private bool IsServiceStopped(CapacityState currentCapacityState)
        {
            return currentCapacityState == CapacityState.NotActivated || currentCapacityState == CapacityState.Suspended;
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