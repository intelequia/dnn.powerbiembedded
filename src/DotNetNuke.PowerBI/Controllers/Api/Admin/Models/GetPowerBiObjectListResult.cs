using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin.Models
{
    public class GetPowerBiObjectListResponse
    {
        public enum ObjectType
        {
            Report = 0,
            Dashboard = 1
        }
        public enum PermissionType
        {
            View = 1,
            Edit = 2
        }
        public class Permission
        {
            public bool AllowAccess;
            public Guid ID;
            public PermissionType PermissionId;
            public int PortalID;
            public int? RoleID;
            public string RoleName;
            public int? UserID;
            public string UserDisplayName;

            public static List<Permission> DataToPermissions(IQueryable<Data.Models.ObjectPermission> objPermissions)
            {
                var result = new List<Permission>();
                foreach (var permission in objPermissions)
                {
                    result.Add(new Permission
                    {
                        AllowAccess = permission.AllowAccess,
                        ID = permission.ID,
                        PermissionId = (PermissionType)permission.PermissionID,
                        PortalID = permission.PortalID,
                        RoleID = permission.RoleID,
                        RoleName = permission.RoleName,
                        UserDisplayName = permission.UserDisplayName,
                        UserID = permission.UserID
                    });
                }

                return result;
            }
        }
        public string Id;
        public string Name;
        public ObjectType PowerBiType;
        public List<Permission> Permissions;
    }

}