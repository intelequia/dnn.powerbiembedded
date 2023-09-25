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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Web;
using System.Web.Hosting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
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
                var common = new Components.Common();
                var settings = SharedSettingsRepository.Instance.GetAllSettings();

                foreach (var setting in settings.AsParallel())
                {
                    var tokenCredentials = common.GetTokenCredentials(setting).Result;
                    var subscriptions = SubscriptionsRepository.Instance.GetSubscriptionsByWorkspaceId(setting.WorkspaceId, setting.PortalId);

                    foreach (var subscription in subscriptions.AsParallel())
                    {
                        ProcessSubscription(common, setting, tokenCredentials, subscription);
                    }
                }

                this.ScheduleHistoryItem.AddLogNote("Done");
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void ProcessSubscription(Components.Common common, PowerBISettings setting, TokenCredentials tokenCredentials, Subscription subscription)
        {
            try
            {
                var portalSettings = new PortalSettings(subscription.PortalId);

                if (subscription.Enabled && IsSubscriptionDue(subscription))
                {
                    var htmlBody = CreateEmailBody(subscription);
                    var subject = subscription.EmailSubject;

                    var subscriptionSubscribers = SubscriptionsSubscribersRepository.Instance.GetSubscribersBySubscription(subscription.Id);
                    var userIds = subscriptionSubscribers
                        .Where(subscriber => subscriber.UserId.HasValue)
                        .Select(subscriber => subscriber.UserId.Value);

                    foreach (var subscriptionSubscriber in subscriptionSubscribers.AsParallel())
                    {
                        if (subscriptionSubscriber.UserId != null)
                        {
                            var userInfo = UserController.GetUserById(portalSettings.PortalId, (int)subscriptionSubscriber.UserId);
                            SendEmail(common, setting, tokenCredentials, subscription, userInfo, subject, htmlBody, portalSettings);
                        }
                        else
                        {
                            ProcessRoleSubscribers(common, setting, tokenCredentials, subscription, portalSettings, subscriptionSubscriber, userIds, subject, htmlBody);
                        }
                    }
                    this.ScheduleHistoryItem.AddLogNote($"Processed '{subscription.Name}'");
                    subscription.LastProcessedOn = DateTime.Now;
                    SubscriptionsRepository.Instance.EditSubscription(subscription);
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error processing the subscription '{subscription.Name}', {ex}", subscription);
            }
        }

        private bool IsSubscriptionDue(Subscription subscription)
        {
            var currentDate = DateTime.Now;
            var timeSinceLastProcessed = currentDate - (subscription.LastProcessedOn ?? currentDate);

            // If LastProcessedOn is null, treat it as if it's been a long time since the last processing
            if (subscription.LastProcessedOn == null)
            {
                return true;
            }

            var totalDays = (int)timeSinceLastProcessed.TotalDays;
            var currentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, subscription.TimeZone);
            var repeatDateTime = currentDateTime.Date + subscription.RepeatTime;

            return (currentDateTime >= repeatDateTime) &&
                ((subscription.RepeatPeriod.Equals("Daily") && totalDays >= 1) ||
                 (subscription.RepeatPeriod.Equals("Weekly") && totalDays >= 7) ||
                 (subscription.RepeatPeriod.Equals("Monthly") && totalDays >= 30));
        }


        private string CreateEmailBody(Subscription subscription)
        {
            var templatePath = HostingEnvironment.MapPath("~\\DesktopModules\\MVC\\PowerBiEmbedded\\Views\\emailtemplate.cshtml");
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PowerBI.Export.EmailTemplatePath"]))
                templatePath = ConfigurationManager.AppSettings["PowerBI.Export.EmailTemplatePath"];

            var htmlBody = File.ReadAllText(templatePath);
            htmlBody = htmlBody.Replace("[[SubscriptionName]]", subscription.Name);
            htmlBody = htmlBody.Replace("[[EmailBody]]", subscription.Message);
            htmlBody = htmlBody.Replace("[[ReportDate]]", DateTime.UtcNow.Date.ToShortDateString());

            return htmlBody;
        }

        private void SendEmail(Components.Common common, PowerBISettings setting, TokenCredentials tokenCredentials, Subscription subscription, UserInfo userInfo, string subject, string htmlBody, PortalSettings portalSettings)
        {
            var username = common.GetUsernameProperty(subscription.ModuleId, userInfo);
            var roles = RoleController.Instance.GetUserRoles(UserController.Instance.GetUserByDisplayname(subscription.PortalId, userInfo.DisplayName), true);
            var roleList = roles.Select(role => role.RoleName).ToList();
            var rolesString = string.Join(",", roleList);
            Attachment attachment = null;

            try
            {
                attachment = common.ExportPowerBIReport(Guid.Parse(subscription.ReportId), tokenCredentials, setting, subscription.ReportPages, rolesString, username, portalSettings.DefaultLanguage.ToLower()).Result;
            }
            catch (Exception ex)
            {
                HandleException(ex, subscription);
            }

            if (attachment == null)
            {
                HandleError("There was an error processing the export.", subscription);
            }

            var attachments = new List<Attachment> { attachment };
            htmlBody = htmlBody.Replace("[[ReportName]]", attachment.Name);

            try
            {
                Mail.SendMail(
                    HostController.Instance.GetString("HostEmail"),
                    userInfo.Email,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    MailPriority.Normal,
                    subject,
                    MailFormat.Html,
                    Encoding.UTF8,
                    htmlBody,
                    attachments,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    true);
            }
            catch (Exception ex)
            {
                HandleException(ex, subscription);
            }
        }

        private void ProcessRoleSubscribers(
            Components.Common common,
            PowerBISettings setting,
            TokenCredentials tokenCredentials,
            Subscription subscription,
            PortalSettings portalSettings,
            SubscriptionSubscriber subscriptionSubscriber,
            IEnumerable<int> userIds,
            string subject,
            string htmlBody)
        {
            var roleController = new RoleController();
            var roleInfo = roleController.GetRoleById(portalSettings.PortalId, (int)subscriptionSubscriber.RoleId);
            var userInfo = roleController.GetUsersByRole(portalSettings.PortalId, roleInfo.RoleName).ToList();

            foreach (var user in userInfo.AsParallel())
            {
                if (userIds.Contains(user.UserID))
                {
                    continue;
                }

                if (Mail.IsValidEmailAddress(user.Email, subscription.PortalId))
                {
                    SendEmail(common, setting, tokenCredentials, subscription, user, subject, htmlBody, portalSettings);
                }
            }
        }

        private void HandleException(Exception ex, Subscription subscription = null)
        {
            Logger.Error($"Error: {ex.InnerException.Message}", ex);
            var errorMessage = subscription != null ? $"Error processing '{subscription.Name}': {ex.InnerException.Message}\n" : ex.InnerException.Message;
            this.ScheduleHistoryItem.AddLogNote(errorMessage);
        }

        private void HandleError(string errorMessage, Subscription subscription)
        {
            Logger.Error(errorMessage);
            this.ScheduleHistoryItem.AddLogNote(errorMessage);
            subscription.LastProcessedOn = DateTime.Now;
            SubscriptionsRepository.Instance.EditSubscription(subscription);
        }
    }
}