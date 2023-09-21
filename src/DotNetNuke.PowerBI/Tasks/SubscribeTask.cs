using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Components;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Data.Subscriptions;
using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Scheduling;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using MailPriority = DotNetNuke.Services.Mail.MailPriority;
using Subscription = DotNetNuke.PowerBI.Data.Subscriptions.Models.Subscription;
using UserInfo = DotNetNuke.Entities.Users.UserInfo;

namespace DotNetNuke.PowerBI.Tasks
{
    public class SubscribeTask : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SubscribeTask));

        public SubscribeTask(ScheduleHistoryItem item) : base()
        {
            this.ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            try
            {
                DotNetNuke.PowerBI.Components.Common common = new DotNetNuke.PowerBI.Components.Common();
                List<PowerBISettings> settings = SharedSettingsRepository.Instance.GetAllSettings();
                foreach (PowerBISettings setting in settings)
                {
                    TokenCredentials tokenCredentials = common.GetTokenCredentials(setting).Result;
                    List<Subscription> subscriptions = SubscriptionsRepository.Instance.GetSubscriptionsByWorkspaceId(setting.WorkspaceId, setting.PortalId);
                    foreach (Subscription subscription in subscriptions)
                    {
                        try
                        {
                            PortalSettings portalSettings = new PortalSettings(subscription.PortalId);

                        if (subscription.Enabled)
                        {
                            if(DateTime.Now.Date >= subscription.StartDate.Date && DateTime.Now.Date <= subscription.EndDate.Date)
                            {
                                DateTime currentDate = DateTime.Now;
                                TimeSpan timeSinceLastProcessed = new TimeSpan();
                                double totalDays = 30;
                                if (subscription.LastProcessedOn.HasValue)
                                {
                                    timeSinceLastProcessed = currentDate - subscription.LastProcessedOn.Value;
                                    totalDays = timeSinceLastProcessed.TotalDays;
                                }


                                    TimeSpan repeatTime = subscription.RepeatTime;
                                    DateTime currentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, subscription.TimeZone);
                                    DateTime repeatDateTime = currentDateTime.Date + repeatTime;

                                    if (currentDateTime >= repeatDateTime)
                                    {
                                        if ((subscription.RepeatPeriod.Equals("Daily") && totalDays >= 1)
                                            || (subscription.RepeatPeriod.Equals("Weekly") && totalDays >= 7)
                                                || (subscription.RepeatPeriod.Equals("Monthly") && totalDays >= 30))
                                        {
                                            string result = SendSubscriptionsEmails(setting, tokenCredentials, subscription, portalSettings);
                                            if (!string.IsNullOrEmpty(result))
                                            {
                                                Logger.Error($"Error: {result}");
                                                this.ScheduleHistoryItem.AddLogNote(result);
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error processing the subscription '{subscription.Name}'", ex);
                            this.ScheduleHistoryItem.AddLogNote(ex.Message);
                            subscription.LastProcessedOn = DateTime.Now;
                            SubscriptionsRepository.Instance.EditSubscription(subscription);
                        }
                    }
                }
                this.ScheduleHistoryItem.AddLogNote("Done");
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error processing the subscribe Task", ex);
                this.ScheduleHistoryItem.AddLogNote(ex.Message);
                this.ScheduleHistoryItem.Succeeded = false;
                this.Errored(ref ex);
            }
        }
        private string SendSubscriptionsEmails(PowerBISettings setting, TokenCredentials tokenCredentials, Subscription subscription, PortalSettings portalSettings)
        {
            DotNetNuke.PowerBI.Components.Common common = new DotNetNuke.PowerBI.Components.Common();
            Attachment attachment = null;
            try
            {
                attachment = common.ExportPowerBIReport(Guid.Parse(subscription.ReportId), tokenCredentials, setting, subscription.ReportPages, portalSettings.DefaultLanguage.ToLower()).Result;
            } catch (Exception ex)
            {
                return $"Error processing '{subscription.Name}': {ex.InnerException.Message}\n";
            }

            if (attachment == null)
            {
                return "There was an error processing the export.";
            }
            List<Attachment> attachments = new List<Attachment>
                                        {
                                            attachment
                                        };
            string fromAddress = HostController.Instance.GetString("HostEmail");
            string body = subscription.Message;
            string subject = subscription.EmailSubject;
            List<string> recipients = new List<string>();

            List<SubscriptionSubscriber> subscriptionSubscribers = SubscriptionsSubscribersRepository.Instance.GetSubscribersBySubscription(subscription.Id);
            IEnumerable<int> userIds = subscriptionSubscribers
            .Where(subscriber => subscriber.UserId.HasValue)
            .Select(subscriber => subscriber.UserId.Value);

            foreach (SubscriptionSubscriber subscriptionSubscriber in subscriptionSubscribers)
            {
                if (subscriptionSubscriber.UserId != null)
                {
                    recipients.Add(UserController.GetUserById(portalSettings.PortalId, (int)subscriptionSubscriber.UserId).Email);
                }
                else
                {
                    
                    RoleController roleController = new RoleController();
                    UserController userController = new UserController();
                    RoleInfo roleInfo = roleController.GetRoleById(portalSettings.PortalId, (int)subscriptionSubscriber.RoleId);
                    List<UserInfo> userInfo = roleController.GetUsersByRole(portalSettings.PortalId, roleInfo.RoleName).ToList();
                    foreach (UserInfo user in userInfo)
                    {
                        if (userIds.Contains(user.UserID))
                        {
                            continue;
                        }
                        recipients.Add(user.Email);
                    }
                }
            }
            if (recipients.Count == 0)
            {
                return $"No recipients found for subscription '{subscription.Name}'";
            }
            string bccEmails = string.Join(",", recipients);
            
            try
            {
                Mail.SendMail(fromAddress, fromAddress, String.Empty, bccEmails, String.Empty, MailPriority.Normal, subject, MailFormat.Html, Encoding.UTF8, body, attachments, String.Empty, String.Empty, String.Empty, String.Empty, true);
            } catch (Exception ex)
            {
                return $"Error processing '{subscription.Name}': {ex.Message}";
            }
            subscription.LastProcessedOn = DateTime.Now;
            SubscriptionsRepository.Instance.EditSubscription(subscription);

            return string.Empty;
        }
    }
}