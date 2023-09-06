using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Data.Subscriptions.Models
{
    [TableName("PBI_Subscriptions")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Scope("PortalId")]
    public class Subscription
    {
        public int Id { get; set; }
        public int PortalId { get; set; }
        public string ReportId { get; set; }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RepeatPeriod { get; set; }
        public TimeSpan RepeatTime { get; set; }
        public string TimeZone { get; set; }
        public string EmailSubject { get; set; }
        public string Message { get; set; }
        public string ReportPages { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastProcessedOn { get; set; }

        [IgnoreColumn]
        public string Users { get; set; }

        [IgnoreColumn]
        public string Roles { get; set; }
    }
}
