using Microsoft.PowerBI.Api.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class PowerBIListView
    {
        public PowerBIListView()
        {
            Reports = new List<Report>();
            Dashboards = new List<Dashboard>();
        }
        [DataMember]
        public List<Report> Reports { get; set; }
        [DataMember]
        public List<Dashboard> Dashboards { get; set; }
    }
}