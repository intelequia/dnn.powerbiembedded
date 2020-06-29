using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        public class SettingsModel
        {
            public string SettingsGroupId { get; set; }
            public string ContentItemId { get; set; }
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
                    SettingsGroupId = (string) tabModule.TabModuleSettings["PowerBIEmbedded_SettingsGroupId"],
                    ContentItemId = (string)tabModule.TabModuleSettings["PowerBIEmbedded_ContentItemId"]
                };

                if (ModuleContext.Configuration.ModuleDefinition.DefinitionName == "PowerBI Embedded Content View")
                {
                    var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, model.SettingsGroupId);
                    var contentItems = embedService.GetContentListAsync(ModuleContext.PortalSettings.UserId).Result;
                    if (contentItems != null)
                    {
                        // Remove other culture contents
                        contentItems = contentItems.RemoveOtherCultureItems();
                        ViewBag.ContentItems = contentItems;
                    }
                }
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
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_SettingsGroupId", settings.SettingsGroupId);
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_ContentItemId", settings.ContentItemId);

                return RedirectToDefaultRoute();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View();
            }
        }

    }
}