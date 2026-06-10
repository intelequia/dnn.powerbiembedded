using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.Subscriptions;
using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using MailPriority = DotNetNuke.Services.Mail.MailPriority;
using Subscription = DotNetNuke.PowerBI.Data.Subscriptions.Models.Subscription;
using UserInfo = DotNetNuke.Entities.Users.UserInfo;

namespace DotNetNuke.PowerBI.Components
{
    /// <summary>
    /// Encapsulates the logic to render and send a subscription report by email.
    /// Used both by the scheduled <c>SubscribeTask</c> and by the on-demand "Run now" action.
    /// </summary>
    public class SubscriptionProcessor
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SubscriptionProcessor));
        private readonly Common _common;
        private readonly Action<string> _logNote;

        public SubscriptionProcessor(Common common = null, Action<string> logNote = null)
        {
            _common = common ?? new Common();
            _logNote = logNote;
        }

        private void Note(string note)
        {
            _logNote?.Invoke(note);
        }

        /// <summary>
        /// Processes a single subscription.
        /// </summary>
        /// <param name="setting">Power BI settings for the subscription workspace.</param>
        /// <param name="accessToken">A valid Power BI access token.</param>
        /// <param name="subscription">The subscription to process.</param>
        /// <param name="force">
        /// When <c>true</c> the subscription is sent immediately ignoring the enabled flag and the
        /// schedule, and the <c>LastProcessedOn</c> timestamp is not updated (used by "Run now").
        /// </param>
        /// <param name="subscribers">
        /// Optional explicit list of subscribers. When <c>null</c> the subscribers are loaded from
        /// the repository using the subscription id. Used by "Run now" to test the current selection.
        /// </param>
        public async Task ProcessSubscriptionAsync(PowerBISettings setting, string accessToken, Subscription subscription, bool force = false, IList<SubscriptionSubscriber> subscribers = null)
        {
            var portalSettings = new PortalSettings(subscription.PortalId);

            if (!force && (!subscription.Enabled || !IsSubscriptionDue(subscription)))
            {
                return;
            }

            var htmlBody = CreateEmailBody(subscription);
            var subject = subscription.EmailSubject;

            var subscriptionSubscribers = subscribers ?? SubscriptionsSubscribersRepository.Instance.GetSubscribersBySubscription(subscription.Id);
            var userIds = subscriptionSubscribers
                .Where(subscriber => subscriber.UserId.HasValue)
                .Select(subscriber => subscriber.UserId.Value);

            foreach (var subscriptionSubscriber in subscriptionSubscribers)
            {
                if (subscriptionSubscriber.UserId != null)
                {
                    var userInfo = UserController.GetUserById(portalSettings.PortalId, (int)subscriptionSubscriber.UserId);
                    await SendEmailAsync(setting, accessToken, subscription, userInfo, subject, htmlBody, portalSettings);
                }
                else
                {
                    await ProcessRoleSubscribersAsync(setting, accessToken, subscription, portalSettings, subscriptionSubscriber, userIds, subject, htmlBody);
                }
            }

            Note($"Processed '{subscription.Name}'");

            if (!force)
            {
                subscription.LastProcessedOn = DateTime.Now;
                SubscriptionsRepository.Instance.EditSubscription(subscription);
            }
        }

        public bool IsSubscriptionDue(Subscription subscription)
        {
            var currentDate = DateTime.Now;
            var timeSinceLastProcessed = currentDate - (subscription.LastProcessedOn ?? currentDate);
            const string daily = "Daily";
            const string weekly = "Weekly";
            const string monthly = "Monthly";

            // If LastProcessedOn is null, treat it as if it's been a long time since the last processing
            if (subscription.LastProcessedOn == null)
            {
                return true;
            }

            var totalDays = (int)timeSinceLastProcessed.TotalDays;
            var currentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, subscription.TimeZone);
            var repeatDateTime = currentDateTime.Date + subscription.RepeatTime;

            return (currentDateTime >= repeatDateTime) &&
                ((subscription.RepeatPeriod.Equals(daily) && totalDays >= 1) ||
                 (subscription.RepeatPeriod.Equals(weekly) && totalDays >= 7) ||
                 (subscription.RepeatPeriod.Equals(monthly) && totalDays >= 30));
        }

        private string CreateEmailBody(Subscription subscription)
        {
            const string subscriptionName = "[[SubscriptionName]]";
            const string emailBody = "[[EmailBody]]";
            const string reportDate = "[[ReportDate]]";
            var templatePath = HostingEnvironment.MapPath("~\\DesktopModules\\MVC\\PowerBiEmbedded\\Views\\emailtemplate.cshtml");
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PowerBI.Export.EmailTemplatePath"]))
                templatePath = ConfigurationManager.AppSettings["PowerBI.Export.EmailTemplatePath"];

            var htmlBody = File.ReadAllText(templatePath);
            htmlBody = htmlBody.Replace(subscriptionName, subscription.Name);
            htmlBody = htmlBody.Replace(emailBody, subscription.Message);
            htmlBody = htmlBody.Replace(reportDate, DateTime.UtcNow.Date.ToShortDateString());

            return htmlBody;
        }

        private async Task SendEmailAsync(PowerBISettings setting, string accessToken, Subscription subscription, UserInfo userInfo, string subject, string htmlBody, PortalSettings portalSettings)
        {
            var username = _common.GetUsernameProperty(subscription.ModuleId, userInfo);
            var roles = RoleController.Instance.GetUserRoles(UserController.Instance.GetUserByDisplayname(subscription.PortalId, userInfo.DisplayName), true);
            var roleList = roles.Select(role => role.RoleName).ToList();
            var rolesString = string.Join(",", roleList);
            const string reportName = "[[ReportName]]";

            var attachment = await _common.ExportPowerBIReport(Guid.Parse(subscription.ReportId), accessToken, setting, subscription.ReportPages, rolesString, username, portalSettings.DefaultLanguage.ToLower());

            if (attachment == null)
            {
                throw new ApplicationException($"There was an error processing the export for subscription '{subscription.Name}'.");
            }

            var attachments = new List<Attachment> { attachment };
            htmlBody = htmlBody.Replace(reportName, attachment.Name);

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

        private async Task ProcessRoleSubscribersAsync(
            PowerBISettings setting,
            string accessToken,
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

            foreach (var user in userInfo)
            {
                if (userIds.Contains(user.UserID))
                {
                    continue;
                }

                if (Mail.IsValidEmailAddress(user.Email, subscription.PortalId))
                {
                    await SendEmailAsync(setting, accessToken, subscription, user, subject, htmlBody, portalSettings);
                }
            }
        }
    }
}
