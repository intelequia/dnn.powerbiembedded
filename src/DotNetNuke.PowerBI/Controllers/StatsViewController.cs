using DotNetNuke.Framework;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    public class StatsViewController : DnnController
    {
        [DnnHandleError]
        public ActionResult Index()
        {
            ServicesFramework.Instance.RequestAjaxScriptSupport();
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            ViewBag.DefaultRange = GetSetting("PowerBIEmbedded_Stats_DefaultRange", "14d");
            ViewBag.ShowPublishedReports = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowPublishedReports", "True"));
            ViewBag.ShowActiveUsers = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowActiveUsers", "True"));
            ViewBag.ShowDatasets = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowDatasets", "True"));
            ViewBag.ShowLastRefresh = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowLastRefresh", "True"));
            ViewBag.ShowReportViews = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowReportViews", "True"));
            ViewBag.ShowMostViewed = bool.Parse(GetSetting("PowerBIEmbedded_Stats_ShowMostViewed", "True"));
            return View();
        }

        private string GetSetting(string key, string defaultValue)
        {
            if (ModuleContext.Settings.ContainsKey(key))
            {
                return (string)ModuleContext.Settings[key];
            }

            return defaultValue;
        }
    }
}
