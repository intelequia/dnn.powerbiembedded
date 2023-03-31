using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using DotNetNuke.Web.Api;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.PowerBI.Models;
using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("PBIEmbedded,DotNetNuke.PowerBI.ListView,DotNetNuke.PowerBI.ContentView,DotNetNuke.PowerBI.CalendarView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ModuleSettingsController : DnnApiController
    {
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

    }
}