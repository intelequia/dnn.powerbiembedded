using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Bookmarks;
using DotNetNuke.PowerBI.Data.Bookmarks.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class BookmarksController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(BookmarksController));

        public class BookmarkViewModel
        {
            public string displayName { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string reportId { get; set; }
        }

        public class ReportStateViewModel
        {
            public string reportId { get; set; }
            public string state { get; set; }
        }

        private bool CanRememberReportState()
        {
            var settings = ModuleController.Instance.GetTabModule(ActiveModule.TabModuleID).TabModuleSettings;
            var enabled = settings["PowerBIEmbedded_RememberReportState"] as string ?? "true";
            return UserController.Instance.GetCurrentUserInfo().UserID > 0 &&
                string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        public HttpResponseMessage GetReportState(string reportId)
        {
            if (!CanRememberReportState())
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            if (string.IsNullOrEmpty(reportId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var bookmark = BookmarksRepository.Instance.GetReportState(PortalSettings.PortalId, reportId, UserController.Instance.GetCurrentUserInfo().UserID);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, State = bookmark?.State });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveReportState(ReportStateViewModel state)
        {
            if (!CanRememberReportState())
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            if (state == null || string.IsNullOrEmpty(state.reportId) || string.IsNullOrEmpty(state.state))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                BookmarksRepository.Instance.SaveReportState(PortalSettings.PortalId, state.reportId, UserController.Instance.GetCurrentUserInfo().UserID, state.state);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
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
                var deleted = BookmarksRepository.Instance.DeleteBookmark(bookmark.Id, PortalSettings.PortalId, PortalSettings.UserId);
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