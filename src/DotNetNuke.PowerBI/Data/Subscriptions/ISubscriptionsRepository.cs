using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Data.Subscriptions
{
    public interface ISubscriptionsRepository
    {
        int AddSubscription(Subscription subscription);
        bool EditSubscription(Subscription subscription);
        bool DeleteSubscription(int subscriptionId);
        Subscription GetSubscriptionById(int subscriptionId);
        List<Subscription> GetSubscriptionsByReportId(string reportId, int currentPortalId);
        List<Subscription> GetSubscriptionsByWorkspaceId(string workspaceId, int currentPortalId);


    }
}
