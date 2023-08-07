using Dnn.PersonaBar.Library.Helper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Services.Models
{
    public class GetPowerBiObjectListResponse
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(GetPowerBiObjectListResponse));
        public enum ObjectType
        {
            Report = 0,
            Dashboard = 1,
            Workspace = -1
        }
        public enum PermissionType
        {
            View = 1,
            Edit = 2
        }

        public string Id;
        public string Name;
        public ObjectType PowerBiType;
        public Dnn.PersonaBar.Library.DTO.Permissions Permissions;

        public static Dnn.PersonaBar.Library.DTO.Permissions DataToPermissions(IQueryable<Data.Models.ObjectPermission> objPermissions)
        {
            var result = new PBIPermissions(true);
            var permissions = new List<Dnn.PersonaBar.Library.DTO.Permission>();

            foreach (var permission in objPermissions)
            {
                if (permission.RoleID.HasValue)
                {
                    RoleInfo role;
                    if (permission.RoleID.Value == -1)
                    {
                        role = new RoleInfo
                        {
                            RoleID = -1,
                            RoleName = "All Users",
                            IsSystemRole = true
                        };
                    }
                    else
                    {
                        role = RoleController.Instance.GetRoleById(PortalSettings.Current.PortalId, permission.RoleID.Value);
                    }
                    result.EnsureRole(role);

                    var rolePermission = result.RolePermissions.FirstOrDefault(x => x.RoleId == permission.RoleID.Value);
                    if (rolePermission.Permissions == null)
                    {
                        rolePermission.Permissions = new List<Dnn.PersonaBar.Library.DTO.Permission>();
                    }
                    if (permission.PermissionID == 1)
                    {
                        rolePermission.Permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission()
                        {
                            AllowAccess = permission.AllowAccess,
                            FullControl = false,
                            PermissionId = permission.PermissionID,
                            PermissionName = "View",
                            View = true,
                        });
                    }
                    if (permission.PermissionID == 2)
                    {
                        rolePermission.Permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission()
                        {
                            AllowAccess = permission.AllowAccess,
                            FullControl = false,
                            PermissionId = permission.PermissionID,
                            PermissionName = "Edit",
                            View = false,
                        });
                    }


                }
                else
                {
                    var user = UserController.Instance.GetUserById(PortalSettings.Current.PortalId, permission.UserID.Value);
                    if (user == null)
                    {
                        Logger.Warn($"Detected misconfigured permission for user {permission.UserID.Value} and portal {PortalSettings.Current.PortalId}. Ensure the user has logged in at least one time on the portal (missing record in UserPortals table).");
                        continue;
                    }

                    ModulePermissionInfo permissionBase = new ModulePermissionInfo
                    {
                        PermissionID = permission.PermissionID,
                        PermissionName = "View",
                        AllowAccess = permission.AllowAccess,
                        UserID = permission.UserID.Value,
                        DisplayName = user.DisplayName,
                        Username = user.Username,
                    };

                    if (permission.PermissionID == 1)
                    {
                        //permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission
                        //{
                        //    AllowAccess = permission.AllowAccess,
                        //    FullControl = false,
                        //    PermissionId = permission.PermissionID,
                        //    PermissionName = "View",
                        //    View = true,
                        //});
                    }
                    if (permission.PermissionID == 2)
                    {
                        //permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission
                        //{
                        //    AllowAccess = permission.AllowAccess,
                        //    FullControl = false,
                        //    PermissionId = permission.PermissionID,
                        //    PermissionName = "Edit",
                        //    View = false,
                        //});
                        permissionBase = new ModulePermissionInfo
                        {
                            PermissionID = permission.PermissionID,
                            PermissionName = "Edit",
                            AllowAccess = permission.AllowAccess,
                            UserID = permission.UserID.Value,
                            DisplayName = user.DisplayName,
                            Username = user.Username,
                            
                        };
                    }

                    result.AddUserPermission(permissionBase);
                    //result.UserPermissions.Add(new Dnn.PersonaBar.Library.DTO.UserPermission()
                    //{
                    //    UserId = user.UserID,
                    //    DisplayName = user.DisplayName,
                    //    Permissions = permissions
                    //});
                }
            }

            return result;
        }
    }
}