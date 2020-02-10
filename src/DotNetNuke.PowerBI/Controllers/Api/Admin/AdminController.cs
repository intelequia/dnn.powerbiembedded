using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Controllers.Api.Admin.Models;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin
{
    [SupportedModules("DotNetNuke.PowerBI.ListView,DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class AdminController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(AdminController));

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Hello()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Success = true,
                Message = "Hello world!"
            });
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetPowerBiObjectList()
        {
            try
            {
                var result = new List<GetPowerBiObjectListResponse>();
                
                var embedService = new EmbedService(PortalSettings.PortalId);
                
                var model = embedService.GetContentListAsync(PortalSettings.UserInfo.UserID).Result;

                foreach (var report in model.Reports)
                {
                    var reportSecurity = ObjectPermissionsRepository.Instance.GetObjectPermissionsExtended(report.Id, PortalSettings.PortalId);
                    result.Add(new GetPowerBiObjectListResponse
                    {
                        Id = report.Id,
                        Name = report.Name,
                        PowerBiType = GetPowerBiObjectListResponse.ObjectType.Report,
                        Permissions = GetPowerBiObjectListResponse.Permission.DataToPermissions(reportSecurity)
                    });
                }

                foreach (var dashboard in model.Dashboards)
                {
                    var reportSecurity = ObjectPermissionsRepository.Instance.GetObjectPermissionsExtended(dashboard.Id, PortalSettings.PortalId);
                    result.Add(new GetPowerBiObjectListResponse
                    {
                        Id = dashboard.Id,
                        Name = dashboard.DisplayName,
                        PowerBiType = GetPowerBiObjectListResponse.ObjectType.Dashboard,
                        Permissions = GetPowerBiObjectListResponse.Permission.DataToPermissions(reportSecurity)
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    PowebBiObjects = result
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

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetPowerBiPermissions()
        {
            try
            {
                var result = ObjectPermissionsRepository.Instance.GetObjectPermissionsByPortalExtended(PortalSettings.PortalId);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    PowebBiPermissions = result
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

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetPortalUsersAndGroups()
        {
            try
            {
                var result = new GetPortalUsersResponse();
                // Users
                foreach (UserInfo objUser in UserController.GetUsers(PortalSettings.PortalId))
                {
                    result.users.Add(new GetPortalUsersResponse.User
                    {
                        UserId = objUser.UserID,
                        DisplayName = objUser.DisplayName,
                        Username = objUser.Username
                    });
                }

                // Roles
                foreach (RoleInfo objRole in RoleController.Instance.GetRoles(PortalSettings.PortalId))
                {
                    result.roles.Add(new GetPortalUsersResponse.Role
                    {
                        RoleId = objRole.RoleID,
                        RoleName = objRole.RoleName
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    Data = result
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
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SavePowerBiObjectsPermissions(SavePowerBiObjectsPermissionsInput input)
        {
            try
            {
                ObjectPermissionsRepository.Instance.DeleteObjectPermissions(PortalSettings.PortalId);
                foreach (var obj in input.powerBiObjects)
                {
                    foreach (var permission in obj.Permissions)
                    {
                        ObjectPermissionsRepository.Instance.CreateObjectPermission(permission.PbiObjectId
                                                                                  , permission.PermissionId
                                                                                  , permission.AllowAccess
                                                                                  , PortalSettings.PortalId
                                                                                  , permission.RoleId
                                                                                  , permission.UserId);
                    }
                }

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