using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Services.Models
{
    public class SavePowerBiObjectsPermissionsInput
    {
        public enum ObjectType
        {
            Report = 0,
            Dashboard = 1
        }
        public class PowerBiObject
        {
            public string Id;
            public string Name;
            public ObjectType PowerBiType;
            public PBIPermissions Permissions;
        }

        public int settingsId;
        public bool inheritPermissions;
        public List<PowerBiObject> powerBiObjects;
    }
}