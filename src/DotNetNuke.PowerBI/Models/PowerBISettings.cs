using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.WebControls;
using System;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    public class PowerBISettings
    {
        public const string DefaultAuthorityUrl = "https://login.microsoftonline.com/common";
        public const string DefaultResourceUrl = "https://analysis.windows.net/powerbi/api";
        public const string DefaultApiUrl = "https://api.powerbi.com";
        public const string DefaultEmbedUrl = "https://app.powerbi.com";
        public const string DefaultAuthenticationType = "MasterUser";

        public int PortalId { get; set; }
        public string AuthenticationType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
        public string Tenant { get; set; }
        public string ApiUrl { get; set; }
        public string EmbedUrl { get; set; }
        public string WorkspaceId { get; set; }
        public string ContentPageUrl { get; set; }

        public PowerBISettings()
        {
            PortalId = -1;
            AuthorityUrl = DefaultAuthorityUrl;
            ApiUrl = DefaultApiUrl;
            ResourceUrl = DefaultResourceUrl;
            EmbedUrl = DefaultEmbedUrl;
            AuthenticationType = DefaultAuthenticationType;
        }

        public PowerBISettings(int portalId)
        {
            PortalId = portalId;
            AuthorityUrl = DefaultAuthorityUrl;
            ApiUrl = DefaultApiUrl;
            ResourceUrl = DefaultResourceUrl;
            EmbedUrl = DefaultEmbedUrl;
            AuthenticationType = DefaultAuthenticationType;
        }

        public static PowerBISettings GetPortalPowerBISettings(int portalId)
        {
            var portalSettings = PortalController.Instance.GetPortalSettings(portalId);
            var settings = new PowerBISettings(portalId);
            if (portalSettings.ContainsKey("PowerBIEmbedded_Username"))
                settings.Username = portalSettings["PowerBIEmbedded_Username"];
            if (portalSettings.ContainsKey("PowerBIEmbedded_Password"))
                settings.Password = portalSettings["PowerBIEmbedded_Password"];
            if (portalSettings.ContainsKey("PowerBIEmbedded_ApplicationId"))
                settings.ApplicationId = portalSettings["PowerBIEmbedded_ApplicationId"];
            if (portalSettings.ContainsKey("PowerBIEmbedded_WorkspaceId"))
                settings.WorkspaceId = portalSettings["PowerBIEmbedded_WorkspaceId"];
            if (portalSettings.ContainsKey("PowerBIEmbedded_ContentPageUrl"))
                settings.ContentPageUrl = portalSettings["PowerBIEmbedded_ContentPageUrl"];


            return settings;
        }
    }
}