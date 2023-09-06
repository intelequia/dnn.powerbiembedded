using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Data.Subscriptions
{
    public interface ISubscriptionsSubscribersRepository
    {
        bool AddSubscriptionSubscriber(SubscriptionSubscriber subscriptionSubscriber);
        bool EditSubscriptionSubscriber(SubscriptionSubscriber subscriptionSubscriber);
        bool DeleteSubscriptionSubscriber(int subscriptionSubscriberId);
        List<SubscriptionSubscriber> GetSubscribersBySubscription(int subscriptionId);
        SubscriptionSubscriber GetSubscriptionSubscriberById(int subscriptionSubscriberId);
    }
}
