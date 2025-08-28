using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.FavoriteReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Data.FavoriteReports
{
    public class FavoriteReportsRepository : ServiceLocator<IFavoriteReportsRepository, FavoriteReportsRepository>, IFavoriteReportsRepository
    {
        public List<FavoriteReport> GetFavoriteReports(int userId, int portalId)
        {
            Requires.NotNegative("userId", userId);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<FavoriteReport>();
                var favorites = repo.Get(portalId).Where(fav => fav.UserId == userId).OrderBy(fav => fav.CreatedOn).ToList();
                return favorites;
            }
        }

        public bool IsReportFavorite(string reportId, int userId, int portalId)
        {
            Requires.NotNull(reportId);
            Requires.NotNegative("userId", userId);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<FavoriteReport>();
                var favorite = repo.Get(portalId).FirstOrDefault(fav => fav.ReportId == reportId && fav.UserId == userId);
                return favorite != null;
            }
        }

        public int AddFavoriteReport(FavoriteReport favoriteReport)
        {
            Requires.NotNull(favoriteReport);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<FavoriteReport>();
                
                // Check if already exists
                var existing = repo.Get(favoriteReport.PortalId)
                    .FirstOrDefault(fav => fav.ReportId == favoriteReport.ReportId && fav.UserId == favoriteReport.UserId);
                
                if (existing != null)
                {
                    return existing.Id; // Already exists, return existing ID
                }

                repo.Insert(favoriteReport);
                return favoriteReport.Id;
            }
        }

        public bool RemoveFavoriteReport(string reportId, int userId, int portalId)
        {
            Requires.NotNull(reportId);
            Requires.NotNegative("userId", userId);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<FavoriteReport>();
                var favorite = repo.Get(portalId).FirstOrDefault(fav => fav.ReportId == reportId && fav.UserId == userId);
                
                if (favorite != null)
                {
                    repo.Delete(favorite);
                    return true;
                }
                
                return false;
            }
        }

        protected override Func<IFavoriteReportsRepository> GetFactory()
        {
            return () => new FavoriteReportsRepository();
        }
    }
}