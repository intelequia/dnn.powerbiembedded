using System;
using System.Runtime.Serialization;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class Workspace
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int SettingsId { get; set; }
        [DataMember]
        public bool InheritPermissions { get; set; }
    }
}