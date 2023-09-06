using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using DotNetNuke.PowerBI.Data.Bookmarks;
using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Data.Subscriptions
{
    public class SubscriptionsRepository : ServiceLocator<ISubscriptionsRepository, SubscriptionsRepository>, ISubscriptionsRepository
    {
        public Subscription GetSubscriptionById(int subscriptionId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                var subscription = repo.GetById(subscriptionId);
                return subscription;
            }
        }
        public bool DeleteSubscription(int subscriptionId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                var subscription = repo.GetById(subscriptionId);
                repo.Delete(subscription);
                return true;
            }
        }

        public List<Subscription> GetSubscriptionsByReportId(string reportId, int portalId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                var subscriptions = repo.Get(portalId).Where(subscription => subscription.ReportId == reportId).ToList();
                return subscriptions;
            }
        }

        public List<Subscription> GetSubscriptionsByWorkspaceId(string workspaceId, int portalId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                var subscriptions = repo.Get(portalId).Where(subscription => subscription.GroupId == workspaceId).ToList();
                return subscriptions;
            }
        }

        public int AddSubscription(Subscription subscription)
        {
            Requires.NotNull(subscription);

            subscription.CreatedOn = DateTime.Now;
            subscription.CreatedBy = Components.Common.CurrentUser.UserID;

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                repo.Insert(subscription);

                return subscription.Id;
            }
        }

        public bool EditSubscription(Subscription subscription)
        {
            Requires.NotNull(subscription);

            subscription.CreatedOn = DateTime.Now;

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Subscription>();
                repo.Update(subscription);

                return true;
            }
        }

        protected override Func<ISubscriptionsRepository> GetFactory()
        {
            return () => new SubscriptionsRepository();

        }
    }
}