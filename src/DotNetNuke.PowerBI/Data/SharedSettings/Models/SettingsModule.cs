using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace DotNetNuke.PowerBI.Data.Models
{

    [TableName("PBI_SettingsModule")]
    //setup the primary key for table
    [PrimaryKey("ModuleId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("PBI_SettingsModule", CacheItemPriority.Default, 20)]
    [Scope("PortalId")]
    public class SettingsModule
    {
        public int ModuleId { get; set; }
        public int SettingsId { get; set; }
        
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}