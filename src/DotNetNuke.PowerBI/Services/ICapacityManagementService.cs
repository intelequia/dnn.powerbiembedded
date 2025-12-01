using DotNetNuke.PowerBI.Data.CapacitySettings.Models;
using DotNetNuke.PowerBI.Services.Models;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Services
{
    public interface ICapacityManagementService
    {
        Task<AzureCapacity> GetCapacityStatusAsync(CapacitySettings settings);
        Task<bool> StartCapacityAsync(CapacitySettings settings);
        Task<bool> PauseCapacityAsync(CapacitySettings settings);
        Task<bool> IsCapacityRunningAsync(CapacitySettings settings);
    }
}