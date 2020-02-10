using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin.Models
{
    public class SavePowerBiObjectsPermissionsInput
    {
        public class PowerBiObject
        {
            public string Id;
            public string Name;
            public List<Permission> Permissions;
        }
        public class Permission
        {
            public string Id;
            public int PermissionId;
            public bool AllowAccess;
            public string PbiObjectId;
            public int? UserId;
            public int? RoleId;
        }

        public List<PowerBiObject> powerBiObjects;
    }
}