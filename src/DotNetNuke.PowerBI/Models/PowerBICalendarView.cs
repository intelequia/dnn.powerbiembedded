using Microsoft.PowerBI.Api.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class PowerBICalendarView
    {
        public PowerBICalendarView()
        {
            RefreshSchedules = new List<CalendarItem>();
        }
        [DataMember]
        public List<string> Workspaces { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public List<CalendarItem> RefreshSchedules { get; set; }
        [DataMember]
        public List<SelectListItem> Options { get; set; }
        public int SelectedOption { get; set; }

    }

    public class CalendarItem
    {
        public string id { get; set; }
        public string resourceId { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public string color { get; set; }
    }

    public class Schedule
    {
        public string start { get; set; }
        public string end { get; set; }
    }
}