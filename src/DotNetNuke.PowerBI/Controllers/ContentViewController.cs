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
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using static DotNetNuke.PowerBI.Services.SubscriptionController;

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
                var contentItemId = GetSetting("PowerBIEmbedded_ContentItemId");
                string itemId = contentItemId.Length > 2 ? contentItemId.Substring(2) : ""; 

                bool hasEditPermission = HasPermission(embedService.Settings, Request["reportId"] ?? itemId, 2);
                bool hasDownloadPermission = HasPermission(embedService.Settings, Request["reportId"] ?? itemId, 3);


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
                    
                    if (contentItemId.Substring(0, 2) == "D_")
                    {
                        model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, itemId, hasEditPermission).Result;
                    }
                    else
                    {
                        model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, itemId, hasEditPermission).Result;
                    }
                }

                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!string.IsNullOrEmpty(model.Id) && !PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : model.Id, User))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }


                Pages reportPages = embedService.GetReportPages(model.Id).Result;
                ViewBag.ReportPages = reportPages;
                ViewBag.CanEdit = hasEditPermission && bool.Parse(GetSetting("PowerBIEmbedded_EditVisible", "false"));
                ViewBag.CanDownload = hasDownloadPermission && bool.Parse(GetSetting("PowerBIEmbedded_DownloadVisible", "false"));
                ViewBag.CanExport = hasDownloadPermission && bool.Parse(GetSetting("PowerBIEmbedded_ExportVisible", "false"));
                ViewBag.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);

                ViewBag.TimeZones = TimeZoneInfo.GetSystemTimeZones();
                ViewBag.PreferredTimeZone = User.Profile.PreferredTimeZone;
                ViewBag.ShowSubscription = bool.Parse(GetSetting("PowerBIEmbedded_ShowSubscriptions", "false"));
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
                ViewBag.RefreshVisible = bool.Parse(GetSetting("PowerBIEmbedded_RefreshVisible", "true"));
                

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
                    ViewBag.CanDownload,
                    ViewBag.CanExport,
                    ViewBag.TimeZones,
                    ViewBag.PreferredTimeZone,
                    ViewBag.ReportPages,
                    model.ContentType,
                    Token = model.EmbedToken?.Token,
                    model.EmbedUrl,
                    model.Id,
                    embedService.Settings.WorkspaceId,
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

        private bool HasPermission(PowerBISettings settings, string reportId, int permissionId)
        {
            bool hasInheritPermissions = settings.InheritPermissions;
            string comparison = hasInheritPermissions ? settings.SettingsGroupId : reportId;
            PortalSettings portalSettings = ModuleContext.PortalSettings;
            UserInfo user = portalSettings.UserInfo;

            return PowerBIListViewExtensions.UserHasPermissionsToWorkspace(comparison, user, permissionId);
        }
    }
}