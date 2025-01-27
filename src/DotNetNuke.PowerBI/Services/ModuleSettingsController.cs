using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Controllers;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
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

    }
}