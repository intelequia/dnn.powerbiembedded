﻿using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using System.Collections.Generic;


namespace DotNetNuke.PowerBI.Data.Bookmarks
{
    public interface IBookmarksRepository
    {
        List<Bookmark> GetBookmarks(string reportId, int currentPortalId);
        List<Bookmark> GetBookmarksByUser(int portalId, string reportId, int userId);
        int SaveBookmark(Bookmark bookmark);
        bool DeleteBookmark(int bookmarkId);
    }
}