using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DotNetNuke.PowerBI.Data.FavoriteReports.Models;

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
            Workspaces = new List<Workspace>();
            FavoriteReports = new List<Report>();
        }
        [DataMember]
        public string WorkspaceId { get; set; }
        [DataMember]
        public List<Report> Reports { get; set; }
        [DataMember]
        public List<Dashboard> Dashboards { get; set; }
        [DataMember]
        public List<Workspace> Workspaces { get; set; }
        [DataMember]
        public List<Report> FavoriteReports { get; set; }
    }
}