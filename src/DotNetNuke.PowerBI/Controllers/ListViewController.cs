using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    public class ListViewController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ListViewController));
        // GET: ListView
        [DnnHandleError]
        public ActionResult Index()
        {
            try
            {
                var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId);
                var model = embedService.GetContentListAsync(ModuleContext.PortalSettings.UserId).Result;                
                if (model != null)
                {
                    // Remove other culture contents
                    model = model.RemoveOtherCultureItems();

                    // Remove the objects without permissions
                    model = model.RemoveUnauthorizedItems(User);

                    // Sets the reports page on the viewbag
                    var reportsPage = embedService.Settings.ContentPageUrl;
                    if (!reportsPage.StartsWith("http"))
                    {
                        reportsPage = Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias) + reportsPage;
                    }

                    ViewBag.ReportsPage = reportsPage;

                    //Get SettingsId
                    var tabModuleSettings = ModuleController.Instance.GetTabModule(ModuleContext.TabModuleId)
                        .TabModuleSettings;
                    if (tabModuleSettings.ContainsKey("PowerBIEmbedded_SettingsGroupId"))
                        ViewBag.SettingsGroupId = tabModuleSettings["PowerBIEmbedded_SettingsGroupId"];
                    else
                        ViewBag.SettingsGroupId = "";

                    return View(model);
                }

                return View();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View();
            }
        }
    }
}