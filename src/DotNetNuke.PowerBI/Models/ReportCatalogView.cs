using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetNuke.PowerBI.Models
{
    /// <summary>
    /// View model for the Report Catalog module. The catalog is built from the DNN pages
    /// (tabs) the current user is allowed to view and that are tagged with the "Report"
    /// term (case-insensitive). The remaining tags of each page become its categories.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReportCatalogView
    {
        public ReportCatalogView()
        {
            Items = new List<ReportCatalogItem>();
            Categories = new List<ReportCatalogCategory>();
        }

        /// <summary>Catalog entries the user is allowed to see, ordered alphabetically by title.</summary>
        [DataMember]
        public List<ReportCatalogItem> Items { get; set; }

        /// <summary>
        /// Distinct categories (non-"Report" tags) ordered alphabetically ascending. The color
        /// of each category is assigned from a fixed palette based on its ordinal position in
        /// this sorted list.
        /// </summary>
        [DataMember]
        public List<ReportCatalogCategory> Categories { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ReportCatalogCategory
    {
        [DataMember]
        public string Name { get; set; }

        /// <summary>Hex color assigned to the category (e.g. "#4263eb").</summary>
        [DataMember]
        public string Color { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ReportCatalogItem
    {
        public ReportCatalogItem()
        {
            Categories = new List<string>();
        }

        [DataMember]
        public int TabId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        /// <summary>Navigation URL of the page the entry points to.</summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>Categories the page belongs to, ordered alphabetically ascending.</summary>
        [DataMember]
        public List<string> Categories { get; set; }

        /// <summary>
        /// The alphabetically-first category, used to color the icon and the leading badge.
        /// Null when the page has no category other than "Report".
        /// </summary>
        [DataMember]
        public string PrimaryCategory { get; set; }

        /// <summary>Hex color derived from <see cref="PrimaryCategory"/> (or a neutral fallback).</summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>Resolved URL of the page icon, or null when the page has no icon.</summary>
        [DataMember]
        public string IconUrl { get; set; }

        /// <summary>True when <see cref="IconUrl"/> points to an SVG that can be recolored.</summary>
        [DataMember]
        public bool IconIsSvg { get; set; }

        /// <summary>Placeholder view counter (currently a random value).</summary>
        [DataMember]
        public int Views { get; set; }
    }
}
