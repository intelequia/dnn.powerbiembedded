using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ExportsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ExportsController));

        [HttpGet]
        [HttpHead]
        public async Task<HttpResponseMessage> Download(string sid, string rid)
        {
            var model = new EmbedConfig();
            try
            {
                // Remove the objects without permissions
                
                if (!Guid.TryParse(sid, out Guid settingsGroupId) 
                    || !Guid.TryParse(rid, out Guid reportId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sid and Rid are required");
                }
                
                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId.ToString(), UserInfo))
                {
                    Logger.Error($"User {UserInfo.Username} doesn't have permissions for settings group {settingsGroupId}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                var embedService = new EmbedService(this.PortalSettings.PortalId, this.ActiveModule.TabModuleID, settingsGroupId.ToString());
                bool hasDownloadPermission = HasPermission(embedService.Settings, reportId.ToString(), 3);
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!hasDownloadPermission || (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : reportId.ToString(), UserInfo)))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                var contents = await embedService.GetContentListAsync(PortalSettings.UserId);
                var report = contents.Reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }

                Stream reportStream = await embedService.DownloadReportAsync(Guid.Parse(embedService.Settings.WorkspaceId), reportId);
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(reportStream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pbix");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{report.Name}.pbix"
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private bool HasPermission(PowerBISettings settings, string reportId, int permissionId)
        {
            bool hasInheritPermissions = settings.InheritPermissions;
            string comparison = hasInheritPermissions ? settings.SettingsGroupId : reportId;
            UserInfo user = PortalSettings.UserInfo;
            return PowerBIListViewExtensions.UserHasPermissionsToWorkspace(comparison, user, permissionId);
        }
    }
}