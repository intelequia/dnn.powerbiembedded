using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.PowerBI.Controllers
{
    public class ListViewController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ListViewController));
        private const string LanguageRegularExpression = "(\\([a-zA-Z]{2}-[a-zA-z]{2}\\))";
        // GET: ListView
        [DnnHandleError]
        public ActionResult Index()
        {
            try
            {
                var embedService = new EmbedService(ModuleContext.PortalId,ModuleContext.TabModuleId);
                var model = embedService.GetContentListAsync(ModuleContext.PortalSettings.UserId).Result;

                // Remove other culture contents
                if (model != null)
                {
                    model.Reports.RemoveAll(x =>
                        Regex.Match(x.Name, LanguageRegularExpression, RegexOptions.IgnoreCase).Success
                        && !x.Name.ToLowerInvariant()
                            .Contains(
                                $"({System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()})"));
                    model.Dashboards.RemoveAll(x =>
                        Regex.Match(x.DisplayName, LanguageRegularExpression, RegexOptions.IgnoreCase).Success
                        && !x.DisplayName.ToLowerInvariant()
                            .Contains(
                                $"({System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()})"));
                    model.Reports.ForEach(r =>
                        r.Name = Regex.Replace(r.Name, LanguageRegularExpression, "", RegexOptions.IgnoreCase));
                    model.Dashboards.ForEach(r =>
                        r.DisplayName = Regex.Replace(r.DisplayName, LanguageRegularExpression, "",
                            RegexOptions.IgnoreCase));

                    // Remove the objects without permissions
                    var permissionsRepo = ObjectPermissionsRepository.Instance;
                    model.Reports.RemoveAll(x =>
                        !permissionsRepo.HasPermissions(x.Id, ModuleContext.PortalId, 1, User));
                    model.Dashboards.RemoveAll(x =>
                        !permissionsRepo.HasPermissions(x.Id, ModuleContext.PortalId, 1, User));

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
                    ViewBag.SettingsId = tabModuleSettings["PowerBIEmbedded_SettingsId"];

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