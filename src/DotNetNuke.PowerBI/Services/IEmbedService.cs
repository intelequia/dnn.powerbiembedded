using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Models;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Services
{
    public interface IEmbedService
    {
        EmbedConfig EmbedConfig { get; }
        TileEmbedConfig TileEmbedConfig { get; }
        PowerBISettings Settings { get; }

        Task<PowerBIListView> GetContentListAsync(int userId);
        Task<EmbedConfig> GetReportEmbedConfigAsync(int userId, string userName, string roles, string reportId, bool hasEditPermission);
        Task<EmbedConfig> GetDashboardEmbedConfigAsync(int userId, string userName, string roles, string dashboardId, bool hasEditPermission);
        Task<TileEmbedConfig> GetTileEmbedConfigAsync(int userId, string tileId, string dashboardId, bool hasEditPermission);
    }
}