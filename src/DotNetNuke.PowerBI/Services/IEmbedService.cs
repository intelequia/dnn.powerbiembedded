using DotNetNuke.PowerBI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DotNetNuke.PowerBI.Data.Models;

namespace DotNetNuke.PowerBI.Services
{
    public interface IEmbedService
    {
        EmbedConfig EmbedConfig { get; }
        TileEmbedConfig TileEmbedConfig { get; }
        PowerBISettings Settings { get; }

        Task<PowerBIListView> GetContentListAsync(int userId);
        Task<EmbedConfig> GetReportEmbedConfigAsync(int userId, string userName, string roles, string reportId);
        Task<EmbedConfig> GetDashboardEmbedConfigAsync(int userId, string userName, string roles, string dashboardId);
        Task<TileEmbedConfig> GetTileEmbedConfigAsync(int userId, string tileId, string dashboardId);
    }
}