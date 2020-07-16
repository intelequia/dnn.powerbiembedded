using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.PowerBI.Data.Models
{

    [TableName("PBI_Settings")]
    //setup the primary key for table
    [PrimaryKey("SettingsId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("PBI_Settings", CacheItemPriority.Default, 20)]
    [Scope("PortalId")]
    public class PowerBISettings
    {
        public const string DefaultAuthorityUrl = "https://login.microsoftonline.com/common";
        public const string DefaultResourceUrl = "https://analysis.windows.net/powerbi/api";
        public const string DefaultApiUrl = "https://api.powerbi.com";
        public const string DefaultEmbedUrl = "https://app.powerbi.com";
        public const string DefaultAuthenticationType = "MasterUser";

        public int SettingsId { get; set; }
        public string SettingsGroupId { get; set; } //WorkspaceId 
        public string SettingsGroupName { get; set; } //Nombre del workshop -->
        public int PortalId { get; set; }
        public string AuthenticationType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ApplicationId { get; set; }
        public string ServicePrincipalApplicationId { get; set; }
        public string ServicePrincipalApplicationSecret { get; set; }
        public string ServicePrincipalTenant { get; set; }
        public string ApiUrl { get; set; }
        public string EmbedUrl { get; set; }
        public string WorkspaceId { get; set; }
        public string ContentPageUrl { get; set; }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool InheritPermissions { get; set; }


        public PowerBISettings()
        {
            PortalId = -1;
            AuthorityUrl = DefaultAuthorityUrl;
            ApiUrl = DefaultApiUrl;
            ResourceUrl = DefaultResourceUrl;
            EmbedUrl = DefaultEmbedUrl;
            AuthenticationType = DefaultAuthenticationType;
        }

        private PowerBISettings(int portalId)
        {
            PortalId = portalId;
            AuthorityUrl = DefaultAuthorityUrl;
            ApiUrl = DefaultApiUrl;
            ResourceUrl = DefaultResourceUrl;
            EmbedUrl = DefaultEmbedUrl;
            AuthenticationType = DefaultAuthenticationType;
        }

        public static PowerBISettings GetPortalPowerBISettings(int portalId, int tabModuleId)
        {
            var tabModuleSettings = ModuleController.Instance.GetTabModule(tabModuleId).TabModuleSettings;
            var settings = new PowerBISettings(portalId);
            if (tabModuleSettings.ContainsKey("PowerBIEmbedded_SettingsId"))
            {
                settings.SettingsId = Convert.ToInt32(tabModuleSettings["PowerBIEmbedded_SettingsId"]);
                var sharedSettings = SharedSettings.SharedSettingsRepository.Instance.GetSettingsById(settings.SettingsId, portalId);
                settings.Username = sharedSettings.Username;
                settings.Password = sharedSettings.Password;
                settings.ApplicationId = sharedSettings.ApplicationId;
                settings.WorkspaceId = sharedSettings.WorkspaceId;
                settings.ContentPageUrl = sharedSettings.ContentPageUrl;
                settings.AuthenticationType = sharedSettings.AuthenticationType;
                settings.ServicePrincipalTenant = sharedSettings.ServicePrincipalTenant;
                settings.ServicePrincipalApplicationId = sharedSettings.ServicePrincipalApplicationId;
                settings.ServicePrincipalApplicationSecret = sharedSettings.ServicePrincipalApplicationSecret;
                settings.SettingsGroupId = sharedSettings.SettingsGroupId;
                settings.SettingsGroupName = sharedSettings.SettingsGroupName;
                settings.InheritPermissions = sharedSettings.InheritPermissions;
            }
            return settings;
        }
    }
}