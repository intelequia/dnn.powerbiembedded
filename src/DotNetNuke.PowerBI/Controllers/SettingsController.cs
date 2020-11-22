using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        public class SettingsModel
        {
            public SettingsModel()
            {
                FilterPaneVisible = true;
                NavPaneVisible = true;
                OverrideVisualHeaderVisibility = false;
                VisualHeaderVisible = false;
                IsContentView = false;
                ToolbarVisible = false;
                PrintVisible = false;
                Height = "";
            }
            public string SettingsGroupId { get; set; }
            public string ContentItemId { get; set; }
            public bool IsContentView { get; set; }
            public bool FilterPaneVisible { get; set; }
            public bool NavPaneVisible { get; set; }
            public bool OverrideVisualHeaderVisibility { get; set; }
            public bool VisualHeaderVisible { get; set; }
            public bool ToolbarVisible { get; set; }
            public bool PrintVisible { get; set; }
            public string Height { get; set; }
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
                    SettingsGroupId = GetSetting("PowerBIEmbedded_SettingsGroupId"),
                    ContentItemId = GetSetting("PowerBIEmbedded_ContentItemId"),
                    IsContentView = ModuleContext.Configuration.ModuleDefinition.DefinitionName == "PowerBI Embedded Content View",
                    FilterPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_FilterPaneVisible", "True")),
                    NavPaneVisible = bool.Parse(GetSetting("PowerBIEmbedded_NavPaneVisible", "True")),
                    Height = GetSetting("PowerBIEmbedded_Height"),
                    OverrideVisualHeaderVisibility = bool.Parse(GetSetting("PowerBIEmbedded_OverrideVisualHeaderVisibility", "False")),
                    VisualHeaderVisible = bool.Parse(GetSetting("PowerBIEmbedded_VisualHeaderVisible", "False")),
                    ToolbarVisible = bool.Parse(GetSetting("PowerBIEmbedded_ToolbarVisible", "False")),
                    PrintVisible = bool.Parse(GetSetting("PowerBIEmbedded_PrintVisible", "False"))
                };

                if (model.IsContentView)
                {
                    var settingsGroupId = model.SettingsGroupId;
                    if (string.IsNullOrEmpty(settingsGroupId))
                    {
                        var defaultPbiSettingsGroupId = model.SettingsGroupId;
                        var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId).RemoveUnauthorizedItems(User);
                        if (!string.IsNullOrEmpty(defaultPbiSettingsGroupId) && pbiSettings.Any(x => x.SettingsGroupId == defaultPbiSettingsGroupId))
                        {
                            settingsGroupId = defaultPbiSettingsGroupId;
                        }
                        else
                        {
                            settingsGroupId = pbiSettings.FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId)).SettingsGroupId;
                        }
                    }

                    var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, settingsGroupId);
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
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_FilterPaneVisible", settings.FilterPaneVisible.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_NavPaneVisible", settings.NavPaneVisible.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_Height", settings.Height);
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_OverrideVisualHeaderVisibility", settings.OverrideVisualHeaderVisibility.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_VisualHeaderVisible", settings.VisualHeaderVisible.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_ToolbarVisible", settings.ToolbarVisible.ToString());
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, "PowerBIEmbedded_PrintVisible", settings.PrintVisible.ToString());

                return RedirectToDefaultRoute();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View();
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