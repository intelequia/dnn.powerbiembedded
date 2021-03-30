using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.PowerBI.Data.Bookmarks.Models;

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
                var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, settingsGroupId);


                var user = ModuleContext.PortalSettings.UserInfo.Username;
                var userPropertySetting = (string)ModuleContext.Settings["PowerBIEmbedded_UserProperty"];
                if (userPropertySetting == "PowerBiGroup")
                {
                    var userProperty = PortalSettings.UserInfo.Profile.GetProperty("PowerBiGroup");
                    if (userProperty?.PropertyValue != null)
                    {
                        user = userProperty.PropertyValue;
                    }
                }
                else if (userPropertySetting == "Custom")
                {
                    var customProperties = (string) ModuleContext.Settings["PowerBIEmbedded_CustomUserProperty"];
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

                if (!string.IsNullOrEmpty(Request["dashboardId"]))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user,roles, Request["dashboardId"]).Result;
                }
                else if (!string.IsNullOrEmpty(Request["reportId"]))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, Request["reportId"]).Result;
                }
                else if (!string.IsNullOrEmpty(GetSetting("PowerBIEmbedded_ContentItemId")))
                {
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    var contentItemId = GetSetting("PowerBIEmbedded_ContentItemId");
                    if (contentItemId.Substring(0, 2) == "D_")
                    {
                        model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2)).Result;
                    }
                    else
                    {
                        model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2)).Result;
                    }
                }

                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!string.IsNullOrEmpty(model.Id) && !permissionsRepo.HasPermissions(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : model.Id, ModuleContext.PortalId, 1, User))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                ViewBag.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);

                ViewBag.FilterPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_FilterPaneVisible", "true"));
                ViewBag.NavPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_NavPaneVisible", "true"));
                ViewBag.OverrideVisualHeaderVisibility = bool.Parse(GetSetting("PowerBIEmbedded_OverrideVisualHeaderVisibility", "false"));
                ViewBag.OverrideFilterPaneVisibility = bool.Parse(GetSetting("PowerBIEmbedded_OverrideFilterPaneVisibility", "false"));
                ViewBag.VisualHeaderVisible = bool.Parse(GetSetting("PowerBIEmbedded_VisualHeaderVisible", "false"));
                ViewBag.PrintVisible = bool.Parse(GetSetting("PowerBIEmbedded_PrintVisible", "false"));
                ViewBag.ToolbarVisible = bool.Parse(GetSetting("PowerBIEmbedded_ToolbarVisible", "false"));
                ViewBag.FullScreenVisible = bool.Parse(GetSetting("PowerBIEmbedded_FullScreenVisible", "false"));
                ViewBag.BookmarksVisible = bool.Parse(GetSetting("PowerBIEmbedded_BookmarksVisible", "false"));
                ViewBag.Height = GetSetting("PowerBIEmbedded_Height");

                // Sets the reports page on the viewbag
                if (embedService != null)
                {
                    var reportsPage = embedService.Settings.ContentPageUrl;
                    if (reportsPage != null && !reportsPage.StartsWith("http"))
                    {
                        reportsPage = Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias) + reportsPage;
                    }
                    ViewBag.ReportsPage = reportsPage;
                }
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
    }
}