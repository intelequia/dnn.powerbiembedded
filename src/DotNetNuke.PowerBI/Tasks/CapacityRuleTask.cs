using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.PowerBI.Data.CapacitySettings;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.PowerBI.Services.Models;
using DotNetNuke.Services.Scheduling;
using Microsoft.PowerBI.Api.Models;
using System;
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
            var capacities = CapacitySettingsRepository.Instance.GetAllCapacities();

            foreach (var capacity in capacities.AsParallel())
            {
                try
                {
                    // Skip if capacity is disabled or deleted
                    if (!capacity.IsEnabled || capacity.IsDeleted)
                    {
                        continue;
                    }

                    var rules = CapacityRulesRepository.Instance.GetRulesByCapacityId(capacity.CapacityId, capacity.PortalId);
                    var activeRules = rules.Where(r => r.IsEnabled && !r.IsDeleted).ToList();

                    if (activeRules.Count == 0)
                    {
                        continue; // No active rules, skip logging
                    }

                    this.ScheduleHistoryItem.AddLogNote($"Processing {activeRules.Count} active rules for capacity {capacity.CapacityId} ({capacity.CapacityDisplayName})");

                    foreach (var rule in activeRules)
                    {
                        try
                        {
                            if (IsRuleDue(rule) == false)
                            {
                                continue; // Not due for execution
                            }

                            this.ScheduleHistoryItem.AddLogNote($"Executing rule: {rule.RuleName} (Action: {rule.Action})");

                            var azureCapacity = await capacityManagementService.GetCapacityStatusAsync(capacity);
                            if (azureCapacity == null)
                                continue;

                            var success = false;
                            if (rule.Action.Equals("Start", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsServiceActive(azureCapacity.State))
                                    this.ScheduleHistoryItem.AddLogNote($"Service is already running, for rule: {rule.RuleName}");
                                else
                                    success = await capacityManagementService.StartCapacityAsync(capacity);
                            }
                            else if (rule.Action.Equals("Stop", StringComparison.OrdinalIgnoreCase)
                                || rule.Action.Equals("Pause", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsServiceStopped(azureCapacity.State))
                                    this.ScheduleHistoryItem.AddLogNote($"Service is already stopped, for rule: {rule.RuleName}");
                                else
                                    success = await capacityManagementService.PauseCapacityAsync(capacity);
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
                catch (Exception capacityEx)
                {
                    HandleException(capacityEx, $"Error processing capacity {capacity.CapacityId} ({capacity.CapacityDisplayName})");
                }
            }

            this.ScheduleHistoryItem.AddLogNote("Completed capacity rule evaluation");
            this.ScheduleHistoryItem.Succeeded = true;
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

        private bool IsServiceActive(string state)
        {
            return state.Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsServiceStopped(string state)
        {
            return state.Equals("paused", StringComparison.OrdinalIgnoreCase) || 
                   state.Equals("suspended", StringComparison.OrdinalIgnoreCase);
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