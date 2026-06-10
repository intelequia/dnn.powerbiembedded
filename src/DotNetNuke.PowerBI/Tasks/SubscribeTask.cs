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
                var processor = new Components.SubscriptionProcessor(common, note => this.ScheduleHistoryItem.AddLogNote(note));
                var settings = SharedSettingsRepository.Instance.GetAllSettings();

                foreach (var setting in settings.AsParallel())
                {
                    var accessToken = common.GetTokenCredentials(setting).Result;
                    var subscriptions = SubscriptionsRepository.Instance.GetSubscriptionsByWorkspaceId(setting.WorkspaceId, setting.PortalId);

                    foreach (var subscription in subscriptions.AsParallel())
                    {
                        ProcessSubscription(processor, setting, accessToken, subscription);
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

        private void ProcessSubscription(Components.SubscriptionProcessor processor, PowerBISettings setting, string accessToken, Subscription subscription)
        {
            try
            {
                processor.ProcessSubscriptionAsync(setting, accessToken, subscription).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                HandleError($"Error processing the subscription '{subscription.Name}', {ex}", subscription);
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