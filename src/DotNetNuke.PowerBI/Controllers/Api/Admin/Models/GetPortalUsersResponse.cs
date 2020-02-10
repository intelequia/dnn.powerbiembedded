using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.PowerBI.Controllers.Api.Admin.Models
{
    public class GetPortalUsersResponse
    {
        public class User
        {
            public int UserId;
            public string DisplayName;
            public string Username;
        }
        public class Role
        {
            public int RoleId;
            public string RoleName;
        }
        public List<User> users;
        public List<Role> roles;
        public GetPortalUsersResponse()
        {
            this.users = new List<User>();
            this.roles = new List<Role>();
        }
    }
}