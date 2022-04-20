#region Copyright

// 
// Intelequia - https://intelequia.com
// Copyright (c) 2022
// by Intelequia
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using Dnn.PersonaBar.Library;
using Dnn.PersonaBar.Library.Attributes;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Services.Models;
using DotNetNuke.Services.Cache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [MenuPermission(Scope = ServiceScope.Admin)]
    public class PBIEmbeddedController : PersonaBarApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(PBIEmbeddedController));

        /// GET: api/PBIEmbedded/GetSettings
        /// <summary>
        /// Gets the settings
        /// </summary>
        /// <returns>settings</returns>
        [HttpGet]
        public HttpResponseMessage GetWorkspaces()
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettings(PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    workspaces = settings
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        public class UpdateWorkspaceSettingsInput
        {
            public PowerBISettings workspaceSettingsDetail { get; set; }
        }
        /// POST: api/PBIEmbedded/UpdateWorkspace
        /// <summary>
        /// Updates the settings
        /// </summary>
        /// <returns>settings</returns>
        [HttpPost]
        public HttpResponseMessage UpdateWorkspace(UpdateWorkspaceSettingsInput parameters)
        {
            try
            {
                parameters.workspaceSettingsDetail.SettingsGroupId = parameters.workspaceSettingsDetail.WorkspaceId;
                if (parameters.workspaceSettingsDetail.SettingsId == 0)
                {
                    SharedSettingsRepository.Instance.AddSettings(parameters.workspaceSettingsDetail, PortalId, UserInfo.UserID);
                }
                else
                {
                    SharedSettingsRepository.Instance.UpdateSettings(parameters.workspaceSettingsDetail, PortalId);
                }
                CachingProvider.Instance().Clear("Prefix", $"PBI_{PortalId}_");

                // Verify settings
                var settings = SharedSettingsRepository.Instance.GetSettings(PortalId);
                var setting = settings.FirstOrDefault(x => x.WorkspaceId == parameters.workspaceSettingsDetail.WorkspaceId);
                return GetPowerBiObjectList(setting.SettingsId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public class DeleteWorkspaceSettingsInput
        {
            public int SettingsId { get; set; }
        }
        /// POST: api/PBIEmbedded/UpdateWorkspace
        /// <summary>
        /// Deletes the settings
        /// </summary>
        /// <returns>settings</returns>
        [HttpPost]
        public HttpResponseMessage DeleteWorkspace(DeleteWorkspaceSettingsInput parameters)
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettingsById(parameters.SettingsId, PortalId);
                if (settings == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Settings not found");
                }
                SharedSettingsRepository.Instance.DeleteSetting(parameters.SettingsId, PortalId);
                CachingProvider.Instance().Clear("Prefix", $"PBI_{PortalId}_");
                return Request.CreateResponse(HttpStatusCode.OK, new {
                    Success=true
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetPowerBiObjectList(int settingsId)
        {
            try
            {
                var result = new List<GetPowerBiObjectListResponse>();
                var embedService = new EmbedService(PortalSettings.PortalId, -1, settingsId);
                var model = embedService.GetContentListAsync(PortalSettings.UserInfo.UserID).Result;

                if (model == null)
                {
                    throw new ConfigurationErrorsException(embedService.EmbedConfig.ErrorMessage);
                }

                // Add workspace permissions
                var workspaceSecurity = ObjectPermissionsRepository.Instance.GetObjectPermissions(embedService.Settings.SettingsGroupId, PortalSettings.PortalId);
                result.Add(new GetPowerBiObjectListResponse
                {
                    Id = embedService.Settings.SettingsGroupId,
                    Name = embedService.Settings.SettingsGroupName,
                    PowerBiType = GetPowerBiObjectListResponse.ObjectType.Workspace,
                    Permissions = GetPowerBiObjectListResponse.DataToPermissions(workspaceSecurity)
                });

                // Add report permissions
                foreach (var report in model.Reports)
                {
                    var reportSecurity = ObjectPermissionsRepository.Instance.GetObjectPermissions(report.Id.ToString(), PortalSettings.PortalId);

                    result.Add(new GetPowerBiObjectListResponse
                    {
                        Id = report.Id.ToString(),
                        Name = report.Name,
                        PowerBiType = GetPowerBiObjectListResponse.ObjectType.Report,
                        Permissions = GetPowerBiObjectListResponse.DataToPermissions(reportSecurity)
                    });
                }

                // Add dashboard permissions
                foreach (var dashboard in model.Dashboards)
                {
                    var reportSecurity = ObjectPermissionsRepository.Instance.GetObjectPermissions(dashboard.Id.ToString(), PortalSettings.PortalId);
                    result.Add(new GetPowerBiObjectListResponse
                    {
                        Id = dashboard.Id.ToString(),
                        Name = dashboard.DisplayName,
                        PowerBiType = GetPowerBiObjectListResponse.ObjectType.Dashboard,
                        Permissions = GetPowerBiObjectListResponse.DataToPermissions(reportSecurity)
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    InheritPermissions = embedService.Settings.InheritPermissions,
                    PowerBiObjects = result
                });
            }
            catch (ConfigurationErrorsException cex)
            {
                Logger.Error(cex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Success = false,
                    Message = cex.Message,
                    Error = cex
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = Components.Common.LocalizeGlobalString("ErrorGeneric"),
                    Error = ex
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePowerBiObjectsPermissions(SavePowerBiObjectsPermissionsInput input)
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettingsById(input.settingsId, PortalId);
                if (settings.InheritPermissions != input.inheritPermissions)
                {
                    settings.InheritPermissions = input.inheritPermissions;
                    SharedSettingsRepository.Instance.UpdateSettings(settings, PortalId);
                }
                foreach (var pbiObject in input.powerBiObjects)
                {
                    ObjectPermissionsRepository.Instance.DeleteObjectPermissions(pbiObject.Id, PortalSettings.PortalId);
                    foreach (var permission in pbiObject.Permissions.RolePermissions)
                    {
                        var p = permission.Permissions.FirstOrDefault();
                        if (p != null)
                        {
                            ObjectPermissionsRepository.Instance.CreateObjectPermission(pbiObject.Id
                                                                                        , p.PermissionId
                                                                                        , p.AllowAccess
                                                                                        , PortalSettings.PortalId
                                                                                        , permission.RoleId
                                                                                        , null);
                        }
                    }
                    foreach (var permission in pbiObject.Permissions.UserPermissions)
                    {
                        var p = permission.Permissions.FirstOrDefault();
                        if (p != null)
                        {
                            ObjectPermissionsRepository.Instance.CreateObjectPermission(pbiObject.Id
                                                                                        , p.PermissionId
                                                                                        , p.AllowAccess
                                                                                        , PortalSettings.PortalId
                                                                                        , null
                                                                                        , permission.UserId);
                        }
                    }
                }

                CachingProvider.Instance().Clear("Prefix", $"PBI_{PortalId}_");

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = Components.Common.LocalizeGlobalString("ErrorGeneric"),
                    Error = ex
                });
            }
        }


    }
}