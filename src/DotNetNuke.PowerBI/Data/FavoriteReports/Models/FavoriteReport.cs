using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace DotNetNuke.PowerBI.Data.FavoriteReports.Models
{
    [TableName("PBI_FavoriteReports")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Scope("PortalId")]
    public class FavoriteReport
    {
        public FavoriteReport()
        {
        }

        public FavoriteReport(string reportId, int userId, int portalId)
        {
            ReportId = reportId;
            UserId = userId;
            PortalId = portalId;
            CreatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public string ReportId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}