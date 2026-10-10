using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Controllers;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class EmbedTokenController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EmbedTokenController));

        public class RenewTokenRequest
        {
            public string SettingsGroupId { get; set; }
            public string Id { get; set; }
            public string ContentType { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<HttpResponseMessage> Renew(RenewTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SettingsGroupId) ||
                !Guid.TryParse(request.Id, out var itemId) ||
                (request.ContentType != "report" && request.ContentType != "dashboard"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(request.SettingsGroupId, UserInfo))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                var service = new EmbedService(PortalSettings.PortalId, ActiveModule.TabModuleID, request.SettingsGroupId);
                if (service.Settings == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                var permissionKey = service.Settings.InheritPermissions ? service.Settings.SettingsGroupId : itemId.ToString();
                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(permissionKey, UserInfo))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                var module = ModuleController.Instance.GetTabModule(ActiveModule.TabModuleID);
                var settings = new System.Collections.Hashtable(module.ModuleSettings);
                foreach (System.Collections.DictionaryEntry setting in module.TabModuleSettings)
                {
                    settings[setting.Key] = setting.Value;
                }
                var username = ContentViewController.ResolveRlsUsername(UserInfo, key => settings[key] as string ?? "");
                var roles = string.Join(",", UserInfo.Roles);
                var canEdit = PowerBIListViewExtensions.UserHasPermissionsToWorkspace(permissionKey, UserInfo, 2);
                var model = request.ContentType == "dashboard"
                    ? await service.GetDashboardEmbedConfigAsync(UserInfo.UserID, username, roles, itemId.ToString(), canEdit, true).ConfigureAwait(false)
                    : await service.GetReportEmbedConfigAsync(UserInfo.UserID, username, roles, itemId.ToString(), canEdit, true).ConfigureAwait(false);

                if (model.EmbedToken == null || !string.IsNullOrEmpty(model.ErrorMessage))
                {
                    Logger.Error($"Embed token renewal failed: {model.ErrorMessage}");
                    return Request.CreateResponse(HttpStatusCode.BadGateway);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK, new
                {
                    token = model.EmbedToken.Token,
                    expiration = model.EmbedToken.Expiration.ToUniversalTime().ToString("o")
                });
                response.Headers.CacheControl = new CacheControlHeaderValue { NoStore = true };
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("Embed token renewal failed", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}