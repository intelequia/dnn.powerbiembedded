using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Data.Subscriptions
{
    public class SubscriptionsSubscribersRepository : ServiceLocator<ISubscriptionsSubscribersRepository, SubscriptionsSubscribersRepository>, ISubscriptionsSubscribersRepository
    {
        public bool AddSubscriptionSubscriber(SubscriptionSubscriber subscriptionSubscriber)
        {
            Requires.NotNull(subscriptionSubscriber);

            subscriptionSubscriber.CreatedOn = DateTime.Now;
            subscriptionSubscriber.CreatedBy = Components.Common.CurrentUser.UserID;
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<SubscriptionSubscriber>();
                repo.Insert(subscriptionSubscriber);
                return true;
            }

        }

        public bool EditSubscriptionSubscriber(SubscriptionSubscriber subscriptionSubscriber)
        {
            Requires.NotNull(subscriptionSubscriber);
            subscriptionSubscriber.CreatedOn = DateTime.Now;
            subscriptionSubscriber.CreatedBy = Components.Common.CurrentUser.UserID;
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<SubscriptionSubscriber>();
                repo.Update(subscriptionSubscriber);
                return true;
            }

        }

        public bool DeleteSubscriptionSubscriber(int subscriptionSubscriberId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<SubscriptionSubscriber>();
                var subscription = repo.GetById(subscriptionSubscriberId);
                repo.Delete(subscription);
                return true;
            }
        }

        public List<SubscriptionSubscriber> GetSubscribersBySubscription(int subscriptionId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<SubscriptionSubscriber>();
                var subscriptionsSubscribers = repo.Get().Where(subscriptionSubscriber => subscriptionSubscriber.SubscriptionId == subscriptionId).ToList();
                return subscriptionsSubscribers;
            }
        }

        public SubscriptionSubscriber GetSubscriptionSubscriberById(int subscriptionSubscriberId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<SubscriptionSubscriber>();
                var subscriptionSubscriber = repo.GetById(subscriptionSubscriberId);
                return subscriptionSubscriber;
            }
        }

        protected override Func<ISubscriptionsSubscribersRepository> GetFactory()
        {
            return () => new SubscriptionsSubscribersRepository();

        }
    }
}