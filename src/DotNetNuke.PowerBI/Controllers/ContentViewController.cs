using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Services.Cache;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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
            try
            {
                // Remove the objects without permissions
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                var embedService = new EmbedService(ModuleContext.PortalId);
                if (!string.IsNullOrEmpty(Request["dashboardId"]))
                {
                    model = embedService.GetDashboardEmbedConfigAsync(ModuleContext.PortalSettings.UserId, Request["dashboardId"]).Result;
                }
                else if (!string.IsNullOrEmpty(Request["reportId"]))
                {
                    var user = ModuleContext.PortalSettings.UserInfo.Username;
                    var roles = string.Join(",", ModuleContext.PortalSettings.UserInfo.Roles);
                    model = embedService.GetReportEmbedConfigAsync(ModuleContext.PortalSettings.UserId, user, roles, Request["reportId"]).Result;
                }
                if (!string.IsNullOrEmpty(model.Id) && !permissionsRepo.HasPermissions(model.Id, ModuleContext.PortalId, 1, User))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                ViewBag.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);
                // Sets the reports page on the viewbag
                var reportsPage = embedService.Settings.ContentPageUrl;
                if (!reportsPage.StartsWith("http"))
                {
                    reportsPage = Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias) + reportsPage;
                }
                ViewBag.ReportsPage = reportsPage;

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