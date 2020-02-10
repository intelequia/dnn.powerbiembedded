using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace DotNetNuke.PowerBI.Data.Models
{
    [TableName("PBI_ObjectPermission")]
    //setup the primary key for table
    [PrimaryKey("ID", AutoIncrement = false)]
    //configure caching using PetaPoco
    [Cacheable("PBI_ObjectPermissions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    //[Scope("ModuleId")]
    public class ObjectPermission
    {
        public Guid ID { get; set; }    // This field is here just because DAL2 doesn't support a primary key with multiple columns (in this table, the primary key should be PowerBiObjectID,PermissionID)
        public string PowerBiObjectID { get; set; }
        public int PermissionID { get; set; }
        public bool AllowAccess { get; set; }
        public int PortalID { get; set; }
        public int? RoleID { get; set; }
        public int? UserID { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int LastModifiedByUserID { get; set; }
        public DateTime LastModifiedOnDate { get; set; }
        [IgnoreColumn]
        public string UserDisplayName { get; set; }
        [IgnoreColumn]
        public string RoleName { get; set; }
    }
}