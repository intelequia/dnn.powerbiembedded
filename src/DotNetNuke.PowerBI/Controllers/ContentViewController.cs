﻿using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Net;
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
            EmbedService embedService;
            try
            {
                // Remove the objects without permissions
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!string.IsNullOrEmpty((string) Request["sid"]))
                    embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, (string) Request["sid"]);
                else
                    embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, (string)ModuleContext.Settings["PowerBIEmbedded_SettingsGroupId"]);

                if (!string.IsNullOrEmpty(Request["dashboardId"]))
                {
                    var user = ModuleContext.PortalSettings.UserInfo.Username;
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user,roles, Request["dashboardId"]).Result;
                }
                else if (!string.IsNullOrEmpty(Request["reportId"]))
                {
                    var user = ModuleContext.PortalSettings.UserInfo.Username;
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, Request["reportId"]).Result;
                }
                else if (ModuleContext.Settings.ContainsKey("PowerBIEmbedded_ContentItemId") 
                    && !string.IsNullOrEmpty((string) ModuleContext.Settings["PowerBIEmbedded_ContentItemId"]))
                {
                    var user = ModuleContext.PortalSettings.UserInfo.Username;
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    var contentItemId = (string)ModuleContext.Settings["PowerBIEmbedded_ContentItemId"];
                    if (contentItemId.Substring(0, 2) == "D_")
                    {
                        model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2)).Result;
                    }
                    else
                    {
                        model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, contentItemId.Substring(2)).Result;
                    }
                }

                if (!string.IsNullOrEmpty(model.Id) && !permissionsRepo.HasPermissions(model.Id, ModuleContext.PortalId, 1, User))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                ViewBag.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);
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
    }
}