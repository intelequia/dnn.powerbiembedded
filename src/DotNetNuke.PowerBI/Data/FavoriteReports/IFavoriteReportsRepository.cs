using DotNetNuke.PowerBI.Data.FavoriteReports.Models;
using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Data.FavoriteReports
{
    public interface IFavoriteReportsRepository
    {
        List<FavoriteReport> GetFavoriteReports(int userId, int portalId);
        bool IsReportFavorite(string reportId, int userId, int portalId);
        int AddFavoriteReport(FavoriteReport favoriteReport);
        bool RemoveFavoriteReport(string reportId, int userId, int portalId);
    }
}