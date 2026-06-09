using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Controllers;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("PBIEmbedded,DotNetNuke.PowerBI.ListView,DotNetNuke.PowerBI.ContentView,DotNetNuke.PowerBI.CalendarView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ModuleSettingsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ModuleSettingsController));

        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetContentItemsByGroup(string groupId)
        {
            try
            {
                EmbedService embedService = new EmbedService(ActiveModule.PortalID, ActiveModule.TabModuleID, groupId);

                PowerBIListView contentItems = embedService.GetContentListAsync(PortalSettings.UserId).Result;
                if (contentItems != null)
                {
                    // Remove other culture contents
                    contentItems = contentItems.RemoveOtherCultureItems();
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    contentItems
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetReportPages(string groupId, string reportId)
        {
            try
            {
                // The Content Item dropdown prefixes report ids with "R_".
                if (!string.IsNullOrEmpty(reportId) && reportId.StartsWith("R_", StringComparison.OrdinalIgnoreCase))
                {
                    reportId = reportId.Substring(2);
                }

                if (string.IsNullOrEmpty(reportId))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { pages = new object[0] });
                }

                EmbedService embedService = new EmbedService(ActiveModule.PortalID, ActiveModule.TabModuleID, groupId);
                var reportPages = embedService.GetReportPages(reportId).Result;

                var pages = reportPages?.Value?
                    .OrderBy(p => p.Order)
                    .Select(p => new { name = p.Name, displayName = p.DisplayName })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { pages });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

    }
}