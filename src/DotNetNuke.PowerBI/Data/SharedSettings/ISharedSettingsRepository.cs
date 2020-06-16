using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.Models;

namespace DotNetNuke.PowerBI.Data.SharedSettings
{
    public interface ISharedSettingsRepository
    {
        PowerBISettings GetSettingsById(int settingId, int portalId);
        List<PowerBISettings> GetSettings(int portalId);
        bool SaveSettings(PowerBISettings setting ,int portalId, int? userId);
        bool UpdateSettings(PowerBISettings settings, int portalId);
        bool DeleteSetting(int settingId, int portalId);
    }
}