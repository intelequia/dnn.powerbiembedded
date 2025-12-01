using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace DotNetNuke.PowerBI.Data.CapacityRules.Models
{
    [TableName("PBI_CapacityRules")]
    [PrimaryKey("RuleId", AutoIncrement = true)]
    [Cacheable("PBI_CapacityRules", CacheItemPriority.Default, 20)]
    [Scope("PortalId")]
    public class CapacityRule
    {
        public int RuleId { get; set; }
        public int PortalId { get; set; }
        public int CapacityId { get; set; }
        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public bool IsEnabled { get; set; }
        public string RuleType { get; set; } // "TimeBasedStart", "TimeBasedStop"
        public string ScheduleExpression { get; set; } // Cron-like expression or time specification
        public TimeSpan ExecutionTime { get; set; }
        public string DaysOfWeek { get; set; } // Comma-separated days (0=Sunday, 1=Monday, etc.)
        public string Action { get; set; } // "Start", "Stop"
        public string TimeZoneId { get; set; }
        public DateTime? LastExecutedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public CapacityRule()
        {
            IsEnabled = true;
            IsDeleted = false;
            RuleType = "TimeBasedStart";
            Action = "Start";
            TimeZoneId = "UTC";
            DaysOfWeek = "1,2,3,4,5"; // Monday to Friday by default
        }
    }
}