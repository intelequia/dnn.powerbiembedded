using DotNetNuke.Entities.Portals;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Services.Cache;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using DotNetNuke.Entities.Modules;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;

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
            var settings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId);
            //settings de la base de datos <== voy por aqui

            return View(settings);
        }

        //[HttpPost]
        //[ValidateInput(false)]
        //[DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        //public ActionResult Index(PowerBISettings settings)
        //{

        //    //TODO CREO QUE AQUI SE GUARDA
        //    var portal = PortalController.Instance;

        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_Username", settings.Username, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_Password", settings.Password, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ApplicationId", settings.ApplicationId, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_WorkspaceId", settings.WorkspaceId, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ContentPageUrl", settings.ContentPageUrl, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_AuthorizationType", settings.AuthenticationType, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ServicePrincipalApplicationId", settings.ServicePrincipalApplicationId, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ServicePrincipalApplicationSecret", settings.ServicePrincipalApplicationSecret, true, null, false);
        //    portal.UpdatePortalSetting(ModuleContext.PortalId, "PowerBIEmbedded_ServicePrincipalTenant", settings.ServicePrincipalTenant, true, null, false);

        //    CachingProvider.Instance().Clear("Prefix", $"PBI_{ModuleContext.PortalId}_{PortalSettings.UserId}_");
        //    //var modules = new ModuleController();
        //    //modules.UpdateModuleSetting(ModuleContext.ModuleId, "PowerBIEmbedded_ContentPageUrl", txtContentPageUrl.Text);
        //    //ModuleContext.Configuration.ModuleSettings["DNNModule1_Setting1"] = settings.Setting1.ToString();
        //    //ModuleContext.Configuration.ModuleSettings["DNNModule1_Setting2"] = settings.Setting2.ToUniversalTime().ToString("u");

        //    return RedirectToDefaultRoute();
        //}

    }
}