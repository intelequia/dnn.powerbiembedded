using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class PowerBICalendarView
    {


        public PowerBICalendarView()
        {
            RefreshSchedules = new List<CalendarItem>();
            History = new List<RefreshedDataset>();
            Workspaces = new List<string>();
        }
        [DataMember]
        public List<string> Workspaces { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public List<CalendarItem> RefreshSchedules { get; set; }
        [DataMember]
        public List<RefreshedDataset> History { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));


    }

    public class CalendarItem
    {
        public string id { get; set; }
        public string resourceId { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string description { get; set; }
    }

    public class Schedule
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class RefreshedDataset : Refresh
    {
        public string Dataset { get; set; }
        public string WorkSpaceName { get; set; }
        public string CapacityName { get; set; }
    }
}