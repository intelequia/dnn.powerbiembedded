using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.FavoriteReports;
using DotNetNuke.PowerBI.Data.FavoriteReports.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ListView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class FavoriteReportsController : DnnApiController
    {
        public class FavoriteReportViewModel
        {
            public string reportId { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage GetFavoriteReports()
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var favorites = FavoriteReportsRepository.Instance.GetFavoriteReports(userId, portalId);
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    Data = favorites
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e.Message
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage IsReportFavorite(string reportId)
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var isFavorite = FavoriteReportsRepository.Instance.IsReportFavorite(reportId, userId, portalId);
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    Data = isFavorite
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e.Message
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage AddFavoriteReport(FavoriteReportViewModel favoriteReportViewModel)
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var favoriteReport = new FavoriteReport(favoriteReportViewModel.reportId, userId, portalId);
                var id = FavoriteReportsRepository.Instance.AddFavoriteReport(favoriteReport);
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = true,
                    Data = id
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e.Message
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage RemoveFavoriteReport(FavoriteReportViewModel favoriteReportViewModel)
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var removed = FavoriteReportsRepository.Instance.RemoveFavoriteReport(favoriteReportViewModel.reportId, userId, portalId);
                
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = removed
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e.Message
                });
            }
        }
    }
}