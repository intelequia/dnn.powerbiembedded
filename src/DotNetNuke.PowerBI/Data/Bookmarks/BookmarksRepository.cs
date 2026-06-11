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
                var bookmarks = repo.Get(portalId).Where(bookmark => bookmark.CreatedBy == userId && bookmark.ReportId == reportId && bookmark.SubscriptionId == null).ToList();
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

        public Bookmark GetBookmarkBySubscription(int portalId, int subscriptionId)
        {
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                return repo.Get(portalId).FirstOrDefault(bookmark => bookmark.SubscriptionId == subscriptionId);
            }
        }

        public int SaveOrUpdateSubscriptionBookmark(Bookmark bookmark)
        {
            Requires.NotNull(bookmark);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                var existing = repo.Get(bookmark.PortalId).FirstOrDefault(b => b.SubscriptionId == bookmark.SubscriptionId);
                if (existing != null)
                {
                    existing.DisplayName = bookmark.DisplayName;
                    existing.Name = bookmark.Name;
                    existing.State = bookmark.State;
                    existing.ReportId = bookmark.ReportId;
                    existing.CreatedOn = DateTime.Now;
                    repo.Update(existing);
                    return existing.Id;
                }

                bookmark.CreatedOn = DateTime.Now;
                bookmark.CreatedBy = Components.Common.CurrentUser.UserID;
                repo.Insert(bookmark);
                return bookmark.Id;
            }
        }

        public bool DeleteBySubscription(int subscriptionId)
        {
            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Bookmark>();
                var bookmarks = repo.Find("WHERE SubscriptionId = @0", subscriptionId).ToList();
                foreach (var bookmark in bookmarks)
                {
                    repo.Delete(bookmark);
                }
                return true;
            }
        }

        protected override Func<IBookmarksRepository> GetFactory()
        {
            return () => new BookmarksRepository();
        }
    }
}