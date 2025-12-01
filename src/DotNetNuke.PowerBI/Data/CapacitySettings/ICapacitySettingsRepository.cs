using DotNetNuke.PowerBI.Data.CapacitySettings.Models;
using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Data.CapacitySettings
{
    public interface ICapacitySettingsRepository
    {
        Models.CapacitySettings GetCapacityById(int capacityId, int portalId);
        Models.CapacitySettings GetCapacityByName(string capacityName, int portalId);
        List<Models.CapacitySettings> GetCapacities(int portalId);
        List<Models.CapacitySettings> GetAllCapacities();
        List<Models.CapacitySettings> GetEnabledCapacities(int portalId);
        bool AddCapacity(Models.CapacitySettings capacity, int portalId, int? userId);
        bool UpdateCapacity(Models.CapacitySettings capacity, int portalId);
        bool DeleteCapacity(int capacityId, int portalId);
    }
}
