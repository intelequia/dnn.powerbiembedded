using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin
{
    [SupportedModules("DotNetNuke.PowerBI.ListView,DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class GroupSettingsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(GroupSettingsController));

        [HttpPost]
        [Web.Api.ValidateAntiForgeryToken]
        public HttpResponseMessage AddOrEditSettings(PowerBISettings powerBiSettings)
        {
            if (powerBiSettings.SettingsId < 0)
            {
                powerBiSettings.PortalId = Components.Common.CurrentPortalSettings.PortalId;
                powerBiSettings.SettingsGroupId = powerBiSettings.WorkspaceId;
                var success =
                    SharedSettingsRepository.Instance.SaveSettings(powerBiSettings, powerBiSettings.PortalId, null);
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = success,
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                Success = false,
            });
        }

        [HttpGet]
        [Web.Api.ValidateAntiForgeryToken]
        public HttpResponseMessage GetSettingsGroups()
        {
            var groupSettings = SharedSettingsRepository.Instance.GetSettings(Components.Common.CurrentPortalSettings.PortalId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = true,
                Data = groupSettings
            });
        }

        [HttpGet]
        [Web.Api.ValidateAntiForgeryToken]
        public HttpResponseMessage SaveModuleSettings(int settingsId, int tabModuleId)
        {

            var settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId,
                Components.Common.CurrentPortalSettings.PortalId);

            var module = ModuleController.Instance;

            DeleteModuleSettings(module, tabModuleId);

            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsId", settings.SettingsId.ToString());
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsGroupId", settings.SettingsGroupId);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsGroupName", settings.SettingsGroupName);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_Username", settings.Username);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_Password", settings.Password);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_ApplicationId", settings.ApplicationId);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_WorkspaceId", settings.WorkspaceId);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_ContentPageUrl", settings.ContentPageUrl);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_AuthorizationType", settings.AuthenticationType);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalApplicationId", settings.ServicePrincipalApplicationId);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalApplicationSecret", settings.ServicePrincipalApplicationSecret);
            module.UpdateTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalTenant", settings.ServicePrincipalTenant);


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = true,
            });
        }

        private void DeleteModuleSettings(IModuleController module, int tabModuleId)
        {
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsId");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsGroupId");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_SettingsGroupName");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_Username");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_Password");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_ApplicationId");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_WorkspaceId");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_ContentPageUrl");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_AuthorizationType");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalApplicationId");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalApplicationSecret");
            module.DeleteTabModuleSetting(tabModuleId, "PowerBIEmbedded_ServicePrincipalTenant");

            var cacheKey = string.Format(DataCache.SingleTabModuleCacheKey, tabModuleId);
            DotNetNuke.Common.Utilities.DataCache.RemoveCache(cacheKey);

        }

        [HttpGet]
        [Web.Api.ValidateAntiForgeryToken]
        public HttpResponseMessage GetModuleSettings(int tabModuleId)
        {

            var module = ModuleController.Instance.GetTabModule(tabModuleId);

            var settingsId = module.TabModuleSettings["PowerBIEmbedded_SettingsId"];

            if (settingsId != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    ModuleSettings = settingsId
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = false,
            });
        }

        [HttpPost]
        [Web.Api.ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteSettings(DeleteSettings deleteSettings)
        {
            var success =
                SharedSettingsRepository.Instance.DeleteSetting(deleteSettings.settingsId, Components.Common.CurrentPortalSettings.PortalId);
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = success,
                });
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                Success = false,
            });
        }

        [HttpPost]
        public HttpResponseMessage HelloWorld()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = true,
            });
        }

        [HttpGet]
        public HttpResponseMessage HelloWorld2()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = true,
            });
        }
    }

    public class DeleteSettings
    {
        public int settingsId;
    }

    public class SaveModuleSettings
    {
        public int settingsId;
        public int moduleId;
        public int tabModuleId;
    }
}