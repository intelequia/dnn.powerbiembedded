using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using DotNetNuke.Web.Api;
using System.Web.Http;
using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.Bookmarks;
using DotNetNuke.Security;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class BookmarksController : DnnApiController
    {
        public class BookmarkViewModel
        {
            public string displayName { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string reportId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage SaveBookmark(BookmarkViewModel bookmarkViewModel)
        {
            try
            {
                var b = bookmarkViewModel;
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var id = BookmarksRepository.Instance.SaveBookmark(new Bookmark(b, userId, portalId));
                if (id > -1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                        Data = id
                    });
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Success = false
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBookmarks(string reportId)
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var bookmarks = BookmarksRepository.Instance.GetBookmarksByUser(portalId, reportId, userId);
                if (bookmarks != null && bookmarks.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                        Data = bookmarks
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }
        
        [HttpPost]
        public HttpResponseMessage DeleteBookmark(Bookmark bookmark)
        {
            try
            {
                var deleted = BookmarksRepository.Instance.DeleteBookmark(bookmark.Id);
                if (deleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                    });
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Success = false,
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }
    }
}