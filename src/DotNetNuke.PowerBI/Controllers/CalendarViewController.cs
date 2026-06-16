using DotNetNuke.Framework;
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

        // GET: CalendarView
        // Renders only the module shell (workspace selector + calendar/history
        // containers). The calendar events and refresh history are loaded from the
        // client via the CalendarController WebAPI, so switching workspaces and paging
        // the history no longer triggers full page postbacks.
        [DnnHandleError]
        public ActionResult Index()
        {
            try
            {
                var pbiSettings = SharedSettingsRepository.Instance.GetSettings(ModuleContext.PortalId);
                var lst = pbiSettings
                    .Select(s => new SelectListItem { Text = s.SettingsGroupName, Value = s.WorkspaceId })
                    .ToList();

                var cid = Request.QueryString["cid"];
                lst.Add(new SelectListItem { Text = "All Workspaces", Value = "-1" });
                ViewBag.Options = new SelectList(lst, "Value", "Text", cid ?? "-1");

                // Enable the DNN Services Framework so the view can call the WebAPI
                // CalendarController via AJAX (jQuery $.ServicesFramework + anti-forgery).
                ServicesFramework.Instance.RequestAjaxScriptSupport();
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                return View(new PowerBICalendarView());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                var model = new PowerBICalendarView
                {
                    ErrorMessage = LocalizeString("Error")
                };
                return View(model);
            }
        }
    }
}
