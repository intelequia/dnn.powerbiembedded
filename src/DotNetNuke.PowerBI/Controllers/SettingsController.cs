using DotNetNuke.Entities.Portals;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Services.Cache;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        // GET: Settings
        [HttpGet]
        public ActionResult Index()
        {
            var settings = PowerBISettings.GetPortalPowerBISettings(ModuleContext.PortalId);
            ViewBag.AuthTypes = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = LocalizeString("MasterUser"),
                    Value = "MasterUser",
                    Selected = settings.AuthenticationType == "MasterUser"
                },
                new SelectListItem()
                {
                    Text = LocalizeString("ServicePrincipal"),
                    Value = "ServicePrincipal",
                    Selected = settings.AuthenticationType == "ServicePrincipal"
                },
            };
            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Index(PowerBISettings settings)
        {
            var portal = PortalController.Instance;
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_Username", settings.Username, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_Password", settings.Password, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ApplicationId", settings.ApplicationId, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_WorkspaceId", settings.WorkspaceId, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ContentPageUrl", settings.ContentPageUrl, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_AuthorizationType", settings.AuthenticationType, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ApplicationSecret", settings.ApplicationSecret, true, null, false);
            portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_Tenant", settings.Tenant, true, null, false);

            CachingProvider.Instance().Clear("Prefix", $"PBI_{ModuleContext.PortalId}_{PortalSettings.UserId}_");

            //var modules = new ModuleController();
            //modules.UpdateModuleSetting(ModuleContext.ModuleId, "PowerBIEmbedded_ContentPageUrl", txtContentPageUrl.Text);
            //ModuleContext.Configuration.ModuleSettings["DNNModule1_Setting1"] = settings.Setting1.ToString();
            //ModuleContext.Configuration.ModuleSettings["DNNModule1_Setting2"] = settings.Setting2.ToUniversalTime().ToString("u");

            return RedirectToDefaultRoute();
        }


    }
}