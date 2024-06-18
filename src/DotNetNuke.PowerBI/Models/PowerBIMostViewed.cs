using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class PowerBIMostViewed
    {
        public PowerBIMostViewed() {
            Reports = new List<ReportWithWorkspace>();
        }
        [DataMember]
        public List<ReportWithWorkspace> Reports { get; set; }
    }
}