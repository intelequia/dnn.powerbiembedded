using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Data.Bookmarks
{
    public class BookmarksRepository : ServiceLocator<IBookmarksRepository, BookmarksRepository>, IBookmarksRepository
    {
        public bool DeleteBookmark(int bookmarkId)
        {
            Requires.NotNegative("portalId", bookmarkId);
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                var bookmark = repo.GetById(bookmarkId);
                repo.Delete(bookmark);
                return true;
            }
        }

        public List<Bookmark> GetBookmarks(string reportId, int currentPortalId)
        {
            Requires.NotNull(reportId);
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                var bookmarks = repo.Get(currentPortalId).Where(bookmark => bookmark.ReportId == reportId).ToList();
                return bookmarks;
            }
        }

        public List<Bookmark> GetBookmarksByUser(int portalId, string reportId, int userId)
        {
            Requires.NotNegative("portalId", portalId);
            Requires.NotNull(reportId);
            Requires.NotNull(userId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                var bookmarks = repo.Get(portalId).Where(bookmark => bookmark.CreatedBy == userId && bookmark.ReportId == reportId).ToList();
                return bookmarks;
            }
        }

        public int SaveBookmark(Bookmark bookmark)
        {
            Requires.NotNull(bookmark);

            bookmark.CreatedOn = DateTime.Now;
            bookmark.CreatedBy = Components.Common.CurrentUser.UserID;

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                repo.Insert(bookmark);

                return bookmark.Id;
            }
        }

        protected override Func<IBookmarksRepository> GetFactory()
        {
            return () => new BookmarksRepository();
        }
    }
}