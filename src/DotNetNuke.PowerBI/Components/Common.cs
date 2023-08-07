using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using System.Globalization;

namespace DotNetNuke.PowerBI.Components
{
    public class Common
    {

        #region Portal Info
        /// <summary>
        /// Current portal Id.
        /// </summary>
        //public static int PortalId => PortalSettings.Current != null ? PortalSettings.Current.PortalId : 0;

        /// <summary>
        /// Current portal Email.
        /// </summary>
        public static string PortalEmail => PortalSettings.Current != null ? PortalSettings.Current.Email : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static string PortalAlias => PortalSettings.Current != null ? PortalSettings.Current.PortalAlias.HTTPAlias : "";

        /// <summary>
        /// Current portal alias url with protocol
        /// </summary>
        public static string PortalAliasUrl => PortalSettings.Current != null ? CurrentPortalSettings.SSLEnabled ? $"https://{PortalSettings.Current.PortalAlias.HTTPAlias}" : $"http://{PortalSettings.Current.PortalAlias.HTTPAlias}" : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static string PortalLogo => PortalSettings.Current != null ? PortalSettings.Current.LogoFile : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static PortalSettings CurrentPortalSettings => PortalSettings.Current ?? null;

        #endregion

        #region User Info

        public static UserInfo CurrentUser => UserController.Instance.GetCurrentUserInfo();

        public static bool IsSuperUser()
        {
            // Usuario no logueado
            if (CurrentUser.UserID == -1)
                return false;

            return CurrentUser.IsSuperUser;
        }

        #endregion

        #region Global string localization

        public static string LocalizeGlobalString(string key)
        {
            string defaultFileName = "GlobalResources.resx";
            string filePath = "~/DesktopModules/MVC/PowerBIEmbedded/App_GlobalResources/";
            var portalId = PortalSettings.Current != null ? PortalSettings.Current.PortalId : 0;
            string resourceFileName = Localization.GetResourceFileName(defaultFileName, CultureInfo.CurrentCulture.Name, string.Empty, portalId);
            string keyValue = Localization.GetString(key, filePath + resourceFileName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                return keyValue;
            }
            keyValue = Localization.GetString(key, filePath + defaultFileName);
            return keyValue;
        }

        #endregion

    }

}