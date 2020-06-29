using Dnn.PersonaBar.Library.DTO;
using Newtonsoft.Json;

namespace DotNetNuke.PowerBI.Services.Models
{
    [JsonObject]
    public class PBIPermissions : Permissions
    {
        public PBIPermissions(bool needDefinitions) : base(needDefinitions)
        {
            //var adminsRole = RoleController.Instance.GetRoleByName(portalId, "Administrators");
            //var allUsersRole = RoleController.Instance.GetRoleByName(portalId, "All Users");
            //var registeredUsersRole = RoleController.Instance.GetRoleByName(portalId, "Registered Users");
            //this.EnsureRole(adminsRole, true, true);
            //this.EnsureRole(allUsersRole, false, true);
            //this.EnsureRole(registeredUsersRole, false, true);
        }

        [JsonProperty("objectId")]
        public int ObjectId { get; set; }

        protected override void LoadPermissionDefinitions()
        {
            PermissionDefinitions.Add(new Permission()
            {
                PermissionId = 1,
                PermissionName = "View",
                FullControl = false,
                View = true
            });
        }
    }
}