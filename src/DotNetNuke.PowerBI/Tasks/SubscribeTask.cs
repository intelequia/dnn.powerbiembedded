﻿using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
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
                        PortalSettings portalSettings = new PortalSettings(subscription.PortalId);

                        if (subscription.Enabled)
                        {
                            if(DateTime.Now > subscription.StartDate && DateTime.Now < subscription.EndDate)
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
                                    if (subscription.RepeatPeriod.Equals("Daily") && totalDays >= 1)
                                    {
                                        if (!SendSubscriptionsEmails(setting, tokenCredentials, subscription, portalSettings))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (subscription.RepeatPeriod.Equals("Weekly") && totalDays >= 7)
                                    {
                                        if (!SendSubscriptionsEmails(setting, tokenCredentials, subscription, portalSettings))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (subscription.RepeatPeriod.Equals("Monthly") && totalDays >= 30)
                                    {
                                        if (!SendSubscriptionsEmails(setting, tokenCredentials, subscription, portalSettings))
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.ScheduleHistoryItem.AddLogNote("Done");
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.AddLogNote(ex.Message);
                this.ScheduleHistoryItem.Succeeded = false;
                this.Errored(ref ex);
            }
        }
        private bool SendSubscriptionsEmails(PowerBISettings setting, TokenCredentials tokenCredentials, Subscription subscription, PortalSettings portalSettings)
        {
            DotNetNuke.PowerBI.Components.Common common = new DotNetNuke.PowerBI.Components.Common();

            Attachment attachment = common.ExportPowerBIReport(Guid.Parse(subscription.ReportId), tokenCredentials, setting, subscription.ReportPages, portalSettings.DefaultLanguage.ToLower()).Result;
            // I want to check if the PDFReport is null or not, if it is null then I want to return false and not send the email
            if (attachment == null)
            {
                return false;
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
                return false;
            }
            string bccEmails = string.Join(",", recipients);
            
            try
            {
                Mail.SendMail(fromAddress, fromAddress, String.Empty, bccEmails, String.Empty, MailPriority.Normal, subject, MailFormat.Html, Encoding.UTF8, body, attachments, String.Empty, String.Empty, String.Empty, String.Empty, true);
            } catch
            {
                return false;
            }
            // subscription.LastProcessedOn = DateTime.Now;
            SubscriptionsRepository.Instance.EditSubscription(subscription);

            return true;
        }
    }
}