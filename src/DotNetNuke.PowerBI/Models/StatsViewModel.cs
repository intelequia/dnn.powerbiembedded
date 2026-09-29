using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetNuke.PowerBI.Models
{
    [Serializable]
    [DataContract]
    public class StatsViewModel
    {
        public StatsViewModel()
        {
            Trend = new List<StatsTrendPoint>();
            MostViewed = new List<StatsMostViewedItem>();
        }

        [DataMember]
        public int PublishedReports { get; set; }

        [DataMember]
        public int PublishedReportsThisMonth { get; set; }

        [DataMember]
        public int ActiveUsers24h { get; set; }

        [DataMember]
        public int ActiveUsersYesterday { get; set; }

        [DataMember]
        public int DatasetsTotal { get; set; }

        [DataMember]
        public int DatasetsAutoRefreshed24h { get; set; }

        [DataMember]
        public DateTime? LastRefreshUtc { get; set; }

        [DataMember]
        public string LastRefreshHealth { get; set; }

        [DataMember]
        public string LastRefreshNote { get; set; }

        [DataMember]
        public bool HasApplicationInsightsConfig { get; set; }

        [DataMember]
        public string Range { get; set; }

        [DataMember]
        public List<StatsTrendPoint> Trend { get; set; }

        [DataMember]
        public List<StatsMostViewedItem> MostViewed { get; set; }

        [DataMember]
        public bool IsPartialData { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string LastRefreshClickUrl { get; set; }
    }

    [Serializable]
    [DataContract]
    public class StatsTrendPoint
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string BucketUtcIso { get; set; }

        [DataMember]
        public int Views { get; set; }
    }

    [Serializable]
    [DataContract]
    public class StatsMostViewedItem
    {
        [DataMember]
        public int TabId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int Views { get; set; }

        [DataMember]
        public List<string> Tags { get; set; }
    }
}
