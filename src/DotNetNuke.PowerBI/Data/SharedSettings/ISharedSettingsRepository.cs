﻿using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.Models;

namespace DotNetNuke.PowerBI.Data.SharedSettings
{
    public interface ISharedSettingsRepository
    {
        PowerBISettings GetSettingsById(int settingId, int portalId);
        PowerBISettings GetSettingsByGroupId(string settingGroupId, int portalId);
        List<PowerBISettings> GetSettings(int portalId);
        bool AddSettings(PowerBISettings setting ,int portalId, int? userId);
        bool UpdateSettings(PowerBISettings settings, int portalId);
        bool DeleteSetting(int settingId, int portalId);
    }
}