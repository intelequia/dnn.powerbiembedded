using System.Collections.Generic;
using System.Linq;
using Dnn.PersonaBar.Library.Helper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.PowerBI.Services.Models
{
    public class GetPowerBiObjectListResponse
    {
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
                    rolePermission.Permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission()
                    {
                        AllowAccess = permission.AllowAccess,
                        FullControl = false,
                        PermissionId = permission.PermissionID,
                        PermissionName = "View",
                        View = permission.AllowAccess
                    });
                }
                else
                {
                    var user = UserController.Instance.GetUserById(PortalSettings.Current.PortalId, permission.UserID.Value);
                    var permissions = new List<Dnn.PersonaBar.Library.DTO.Permission>();
                    permissions.Add(new Dnn.PersonaBar.Library.DTO.Permission()
                    {
                        AllowAccess = permission.AllowAccess,
                        FullControl = false,
                        PermissionId = permission.PermissionID,
                        PermissionName = "View",
                        View = permission.AllowAccess
                    });
                    result.UserPermissions.Add(new Dnn.PersonaBar.Library.DTO.UserPermission()
                    {
                        UserId = user.UserID,
                        DisplayName = user.DisplayName,
                        Permissions = permissions
                    });
                }
            }

            return result;
        }
    }
}