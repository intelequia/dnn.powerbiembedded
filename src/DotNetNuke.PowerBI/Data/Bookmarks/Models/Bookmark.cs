using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.PowerBI.Services;
using System;

namespace DotNetNuke.PowerBI.Data.Bookmarks.Models
{
    [TableName("PBI_Bookmarks")]
    //setup the primary key for table
    [PrimaryKey("Id", AutoIncrement = true)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("PortalId")]
    public class Bookmark
    {
        public Bookmark()
        {
        }
        public Bookmark(BookmarksController.BookmarkViewModel bookmarkViewModel, int userId, int portalId)
        {
            Name = bookmarkViewModel.name;
            DisplayName = bookmarkViewModel.displayName;
            State = bookmarkViewModel.state;
            ReportId = bookmarkViewModel.reportId;
            PortalId = portalId;
        }
        public int Id { get; set; }
        public int PortalId { get; set; }
        public string ReportId { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}