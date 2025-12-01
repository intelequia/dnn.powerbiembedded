using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace DotNetNuke.PowerBI.Data.CapacitySettings.Models
{
    [TableName("PBI_CapacitySettings")]
    [PrimaryKey("CapacityId", AutoIncrement = true)]
    [Cacheable("PBI_CapacitySettings", CacheItemPriority.Default, 20)]
    [Scope("PortalId")]
    public class CapacitySettings
    {
        public const int DefaultAzureManagementPollingInterval = 60;

        public int CapacityId { get; set; }
        public int PortalId { get; set; }
        public string CapacityName { get; set; }
        public string CapacityDisplayName { get; set; }
        public string Description { get; set; }
        
        // Service Principal authentication settings (only authentication method supported)
        public string ServicePrincipalApplicationId { get; set; }
        public string ServicePrincipalApplicationSecret { get; set; }
        public string ServicePrincipalTenant { get; set; }
        
        // Azure Management API settings
        public string AzureManagementSubscriptionId { get; set; }
        public string AzureManagementResourceGroup { get; set; }
        public string AzureManagementCapacityName { get; set; }
        public int AzureManagementPollingInterval { get; set; }
        
        // Additional settings
        public string DisabledCapacityMessage { get; set; }
        public bool IsEnabled { get; set; }
        
        // Audit fields
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public CapacitySettings()
        {
            AzureManagementPollingInterval = DefaultAzureManagementPollingInterval;
            IsEnabled = true;
            IsDeleted = false;
        }
    }
}
