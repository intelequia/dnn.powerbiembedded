using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin.Models
{
    public class GetPowerBiPermissionsResponse
    {
        public Guid ID;
        public string PowerBiObjectID;
        public int PermissionID;
        public bool AllowAccess;
        public int PortalID;
        public int? RoleID;
        public int? UserID;
    }
}