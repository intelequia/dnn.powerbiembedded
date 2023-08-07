using Dnn.PersonaBar.Library.Model;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Components;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Extensibility;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    public class ContentViewController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ContentViewController));

        // GET: ListView
        [DnnHandleError]
        public ActionResult Index()
        {
            var model = new EmbedConfig();
            bool hasEditPermission = HasEditPermission();

            try
            {
                // Remove the objects without permissions
                var settingsGroupId = Request.QueryString["sid"];
                if (string.IsNullOrEmpty(settingsGroupId))
                {
                    var defaultPbiSettingsGroupId = (string)ModuleContext.Settings["PowerBIEmbedded_SettingsGroupId"];
                    var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId).RemoveUnauthorizedItems(User);
                    if (!string.IsNullOrEmpty(defaultPbiSettingsGroupId) && pbiSettings.Any(x => x.SettingsGroupId == defaultPbiSettingsGroupId))
                    {
                        settingsGroupId = defaultPbiSettingsGroupId;
                    }
                    else
                    {
                        settingsGroupId = pbiSettings.FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId))?.SettingsGroupId;
                    }
                }
                else
                {
                    if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId, User))
                    {
                        Logger.Error($"User {User.Username} doesn't have permissions for settings group {settingsGroupId}");
                        settingsGroupId = null;
                    }
                }
                var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, settingsGroupId);


                var user = ModuleContext.PortalSettings.UserInfo.Username;
                var userPropertySetting = (string)ModuleContext.Settings["PowerBIEmbedded_UserProperty"];
                if (userPropertySetting?.ToLowerInvariant() == "email")
                {
                    user = ModuleContext.PortalSettings.UserInfo.Email;
                }
                else if (userPropertySetting == "PowerBiGroup")
                {
                    var userProperty = PortalSettings.UserInfo.Profile.GetProperty("PowerBiGroup");
                    if (userProperty?.PropertyValue != null)
                    {
                        user = userProperty.PropertyValue;
                    }
                }
                else if (userPropertySetting == "Custom" || userPropertySetting == "Custom User Profile Property")
                {
                    var customProperties = (string)ModuleContext.Settings["PowerBIEmbedded_CustomUserProperty"];
                    var matches = Regex.Matches(customProperties, @"\[PROFILE:(?<PROPERTY>[A-z]*)]");

                    foreach (Match match in matches)
                    {
                        var userProperty = PortalSettings.UserInfo.Profile.GetProperty(match.Groups["PROPERTY"].Value);
                        if (userProperty?.PropertyValue != null)
                        {
                            customProperties = customProperties.Replace(match.Value, userProperty.PropertyValue);
                        }
                    }

                    user = customProperties;
                }
                else if (userPropertySetting == "Custom Extension Library")
                {
                    var customExtensionLibrary = (string)ModuleContext.Settings["PowerBIEmbedded_CustomExtensionLibrary"];
                    if (!string.IsNullOrEmpty(customExtensionLibrary))
                    {
                        try
                        {
                            var type = Type.GetType(customExtensionLibrary, true);
                            if (type.GetInterfaces().Contains(typeof(IRlsCustomExtension)))
                            {
                                IRlsCustomExtension extensionInstance = (IRlsCustomExtension)Activator.CreateInstance(type);
                                user = extensionInstance.GetRlsValue(System.Web.HttpContext.Current);
                            }
                            else
                            {
                                throw new Exception($"Library '{customExtensionLibrary}' does not implement IRlsCustomExtension");
                            }
                        }
                        catch (Exception cex)
                        {
                            Logger.Error($"Error instancing custom extension library '{customExtensionLibrary}'", cex);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Request["dashboardId"]))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, Request["dashboardId"], hasEditPermission).Result;
                }
                else if (!string.IsNullOrEmpty(Request["reportId"]))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, Request["reportId"], hasEditPermission).Result;
                }
                else if (!string.IsNullOrEmpty(GetSetting("PowerBIEmbedded_ContentItemId")))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    var contentItemId = GetSetting("PowerBIEmbedded_ContentItemId");
                    if (contentItemId.Substring(0, 2) == "D_")
                    {
                        model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2), hasEditPermission).Result;
                    }
                    else
                    {
                        model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2), hasEditPermission).Result;
                    }
                }

                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!string.IsNullOrEmpty(model.Id) && !PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : model.Id, User))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }




                ViewBag.CanEdit = hasEditPermission;
                ViewBag.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);

                ViewBag.FilterPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_FilterPaneVisible", "true"));
                ViewBag.NavPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_NavPaneVisible", "true"));
                ViewBag.OverrideVisualHeaderVisibility = bool.Parse(GetSetting("PowerBIEmbedded_OverrideVisualHeaderVisibility", "false"));
                ViewBag.OverrideFilterPaneVisibility = bool.Parse(GetSetting("PowerBIEmbedded_OverrideFilterPaneVisibility", "false"));
                ViewBag.VisualHeaderVisible = bool.Parse(GetSetting("PowerBIEmbedded_VisualHeaderVisible", "false"));
                ViewBag.PrintVisible = bool.Parse(GetSetting("PowerBIEmbedded_PrintVisible", "false"));
                ViewBag.ToolbarVisible = bool.Parse(GetSetting("PowerBIEmbedded_ToolbarVisible", "false"))
                    && model.ContentType == "report" && model.ReportType == "PowerBIReport";
                ViewBag.FullScreenVisible = bool.Parse(GetSetting("PowerBIEmbedded_FullScreenVisible", "false"));
                ViewBag.HideVisualizationData = bool.Parse(GetSetting("PowerBIEmbedded_HideVisualizationData", "false"));
                ViewBag.BookmarksVisible = bool.Parse(GetSetting("PowerBIEmbedded_BookmarksVisible", "false"));
                ViewBag.ApplicationInsightsEnabled = bool.Parse(GetSetting("PowerBIEmbedded_ApplicationInsightsEnabled", "false"));
                ViewBag.Height = GetSetting("PowerBIEmbedded_Height");
                ViewBag.PageName = GetSetting("PowerBIEmbedded_PageName");
                ViewBag.BackgroundImageUrl = GetSetting("PowerBIEmbedded_BackgroundImageUrl", "");



                // Sets the reports page on the viewbag
                if (embedService != null)
                {
                    var reportsPage = embedService.Settings.ContentPageUrl;
                    if (reportsPage != null && !reportsPage.StartsWith("http"))
                    {
                        reportsPage = Common.Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias) + reportsPage;
                    }
                    ViewBag.ReportsPage = reportsPage;
                    ViewBag.DisabledCapacityMessage = embedService.Settings.DisabledCapacityMessage;
                }

                var currentLocale = LocaleController.Instance.GetLocale(ModuleContext.PortalId, CultureInfo.CurrentCulture.Name);

                var context = new
                {
                    ModuleContext.PortalId,
                    ModuleContext.TabId,
                    ModuleContext.ModuleId,
                    CurrentLocale = new Culture()
                    {
                        LanguageId = currentLocale.LanguageId,
                        Code = currentLocale.Code,
                        Name = currentLocale.NativeName.Split('(')[0].Trim()
                    },
                    ViewBag.OverrideVisualHeaderVisibility,
                    ViewBag.OverrideFilterPaneVisibility,
                    ViewBag.ApplicationInsightsEnabled,
                    ViewBag.NavPaneVisible,
                    ViewBag.VisualHeaderVisible,
                    ViewBag.Locale,
                    ViewBag.FilterPaneVisible,
                    ViewBag.HideVisualizationData,
                    ViewBag.ReportsPage,
                    ViewBag.Height,
                    ViewBag.PageName,
                    ViewBag.BackgroundImageUrl,
                    ViewBag.CanEdit,
                    model.ContentType,
                    Token = model.EmbedToken?.Token,
                    model.EmbedUrl,
                    model.Id
                };
                ServicesFramework.Instance.RequestAjaxScriptSupport();
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
                DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                ClientAPI.RegisterClientVariable(DnnPage, $"ViewContext_{ModuleContext.ModuleId}",
                    JsonConvert.SerializeObject(context, Formatting.None), true);


                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                model.ErrorMessage = LocalizeString("Error");
                return View(model);
            }
        }

        private string GetSetting(string key, string defaultValue = "")
        {
            return ModuleContext.Settings.ContainsKey(key) ?
                (string)ModuleContext.Settings[key]
                : defaultValue;
        }

        private bool HasEditPermission()
        {
            PortalSettings portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            int portalId = portalSettings.PortalId;
            Entities.Users.UserInfo user = portalSettings.UserInfo;
            bool hasEditPermission = false;

            List<ObjectPermission> objectPermissions = ObjectPermissionsRepository.Instance.GetObjectPermissionsByPortal(portalId).ToList();
            IList<UserRoleInfo> userRoles = RoleController.Instance.GetUserRoles(user, true);
            bool hasRoleWithIdZero = RoleController.Instance.GetUserRoles(user, true).Any(role => role.RoleID == 0);

            // Check if any object permission has PermissionID equal to 2 and matches the RoleID of any user role
            //if (hasRoleWithIdZero || ObjectPermissionsRepository.Instance
            //    .GetObjectPermissionsByPortal(portalId)
            //.Any(permission =>
            //        (userRoles.Any(role => role.RoleID == permission.RoleID) || objectPermissions.Any(databaseUserId => databaseUserId.UserID == user.UserID))
            //        && permission.PermissionID == 2
            //        && permission.AllowAccess))
            //{
            //    hasEditPermission = true;

            //}

            if (hasRoleWithIdZero)
            {
                hasEditPermission = true;
            }
            else
            {
                var list = ObjectPermissionsRepository.Instance.GetObjectPermissionsByPortal(portalId).ToList();
                foreach ( var permission in list ) 
                { 
                    foreach ( var userRole in userRoles)
                    {
                        if (permission.RoleID == -1)
                        {
                            if (permission.PermissionID == 2 && permission.AllowAccess)
                            {
                                hasEditPermission = true;
                            }
                        }
                        else
                        {
                            if (userRole.RoleID == permission.RoleID)
                            {
                                if (permission.PermissionID == 2 && permission.AllowAccess)
                                {
                                    hasEditPermission = true;
                                }
                            }
                            else
                            {
                                if (userRole.UserID == permission.UserID)
                                {
                                    if (permission.PermissionID == 2 && permission.AllowAccess)
                                    {
                                        hasEditPermission = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return hasEditPermission;
        }
    }
}