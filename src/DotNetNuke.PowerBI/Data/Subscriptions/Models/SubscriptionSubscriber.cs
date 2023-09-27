using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Data.Subscriptions.Models
{
    [TableName("PBI_SubscriptionSubscribers")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class SubscriptionSubscriber
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}