using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using System.Collections.Generic;


namespace DotNetNuke.PowerBI.Data.Bookmarks
{
    public interface IBookmarksRepository
    {
        List<Bookmark> GetBookmarks(string reportId, int currentPortalId);
        List<Bookmark> GetBookmarksByUser(int portalId, string reportId, int userId);
        Bookmark GetReportState(int portalId, string reportId, int userId);
        void SaveReportState(int portalId, string reportId, int userId, string state);
        int SaveBookmark(Bookmark bookmark);
        bool DeleteBookmark(int bookmarkId, int portalId, int userId);
        Bookmark GetBookmarkBySubscription(int portalId, int subscriptionId);
        int SaveOrUpdateSubscriptionBookmark(Bookmark bookmark);
        bool DeleteBySubscription(int subscriptionId);
    }
}