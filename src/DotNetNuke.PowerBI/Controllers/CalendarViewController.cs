using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace DotNetNuke.PowerBI.Controllers
{
    public class CalendarViewController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(CalendarViewController));

        // GET: ListView
        [DnnHandleError]
        public ActionResult Index()
        {


            List<SelectListItem> lst = new List<SelectListItem>();

            var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId);
            foreach (var s in pbiSettings)
            {
                lst.Add(new SelectListItem { Text = s.SettingsGroupName, Value = s.WorkspaceId });
            }

            var cid = Request.QueryString["cid"];
            lst.Add(new SelectListItem() { Text = "All Workspaces", Value = "-1" });
            ViewBag.Options = new SelectList(lst, "Value", "Text", cid ?? "-1");


            var settingsGroupId = Request.QueryString["cid"];
            if (string.IsNullOrEmpty(settingsGroupId) || settingsGroupId == "-1")
            {
                var defaultPbiSettingsGroupId = (string)ModuleContext.Settings["PowerBIEmbedded_SettingsGroupId"];
                //var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId).RemoveUnauthorizedItems(User);
                if (!string.IsNullOrEmpty(defaultPbiSettingsGroupId) && pbiSettings.Any(x => x.SettingsGroupId == defaultPbiSettingsGroupId))
                {
                    settingsGroupId = defaultPbiSettingsGroupId;
                }
                else
                {
                    settingsGroupId = pbiSettings.FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId))?.SettingsGroupId;
                }
            }
            var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, settingsGroupId);

            try
            {
                PowerBICalendarView model;

                if (string.IsNullOrEmpty(cid) || cid == "-1")
                {
                    model = embedService.GetScheduleInGroup("-1").Result;
                }
                else
                {
                    model = embedService.GetScheduleInGroup(cid).Result;
                }



                //pagination
                model.CurrentPage = Convert.ToInt32(Request.QueryString["page"]) != 0 ? Convert.ToInt32(Request.QueryString["page"]) : 0;

                model.Count = model.History.Count;
                var skip = (model.CurrentPage - 1) * model.PageSize;
                var take = model.PageSize;

                var pagedHistory = model.History.Skip(skip).Take(take).ToList();

                ViewBag.History = pagedHistory;
                if (model != null)
                {
                    return View(model);
                }
                return View();
            }
            catch (Exception ex)
            {
                var model = new PowerBICalendarView();
                Logger.Error(ex);
                model.ErrorMessage = LocalizeString("Error");
                return View(model);
            }
        }
    }
}