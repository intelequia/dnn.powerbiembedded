using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    public class MostViewedController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MostViewedController));
        // GET: ListView
        [DnnHandleError]
        public ActionResult Index()
        {
            try
            {
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
                        settingsGroupId = pbiSettings.OrderBy(x => x.SettingsGroupName).FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId))?.SettingsGroupId;
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
                    ViewBag.SettingsGroupId = settingsGroupId;

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