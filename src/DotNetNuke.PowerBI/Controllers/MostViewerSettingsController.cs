using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class MostViewerSettingsController : DnnController
    {
        public class SettingsModel
        {
            public SettingsModel()
            {
                ApplicationInsightsAppId = "";
                ApplicationInsightsSecret = "";
            }
            public string ApplicationInsightsAppId { get; set; }
            public string ApplicationInsightsSecret { get; set; }
        }
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SettingsController));
        // GET: Settings
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId);
                ViewBag.Settings = settings;

                var tabModule = ModuleController.Instance.GetTabModule(this.ModuleContext.TabModuleId);
                var model = new SettingsModel
                {
                    ApplicationInsightsAppId = GetSetting("PowerBIEmbedded_ApplicationInsightsAppId", ""),
                    ApplicationInsightsSecret = GetSecureSetting("PowerBIEmbedded_ApplicationInsightsSecret", ""),
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View();
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Index(SettingsModel settings)
        {
            try
            {
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_ApplicationInsightsAppId", settings.ApplicationInsightsAppId.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_ApplicationInsightsSecret", settings.ApplicationInsightsSecret.ToString());
                return RedirectToDefaultRoute();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View();
            }
        }

        private string GetSecureSetting(string key, string defaultValue = "")
        {
            return ModuleContext.Settings.ContainsKey(key) ?
                (string)ModuleContext.Settings[key]
                : defaultValue;
        }

        private string GetSetting(string key, string defaultValue = "")
        {
            return ModuleContext.Settings.ContainsKey(key) ?
                (string)ModuleContext.Settings[key]
                : defaultValue;
        }

    }
}