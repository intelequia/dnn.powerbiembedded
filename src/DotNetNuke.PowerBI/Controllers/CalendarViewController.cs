using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
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
            var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId).RemoveUnauthorizedItems(User);
            var settingsGroupId = pbiSettings.FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId))?.SettingsGroupId;
            var embedService = new EmbedService(ModuleContext.PortalId, ModuleContext.TabModuleId, settingsGroupId);

            List<SelectListItem> lst = new List<SelectListItem>();
            var mode = Request.QueryString["mode"];
            lst.Add(new SelectListItem() { Text = "All Workspaces", Value = "0" });
            lst.Add(new SelectListItem() { Text = "Current Workspace", Value = "1" });
            ViewBag.Options = new SelectList(lst,"Value","Text", mode ?? "0");

            try
            {
                PowerBICalendarView model;

                if (string.IsNullOrEmpty(mode) || mode == "0")
                {
                    model = embedService.GetScheduleInGroup(mode).Result;
                }
                else
                {
                    model = embedService.GetScheduleInGroup(mode).Result;
                }
                

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