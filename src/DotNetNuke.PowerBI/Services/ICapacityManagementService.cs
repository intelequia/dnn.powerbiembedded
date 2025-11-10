using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Services.Models;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Services
{
    public interface ICapacityManagementService
    {
        Task<AzureCapacity> GetCapacityStatusAsync(PowerBISettings settings);
        Task<bool> StartCapacityAsync(PowerBISettings settings);
        Task<bool> PauseCapacityAsync(PowerBISettings settings);
        Task<bool> IsCapacityRunningAsync(PowerBISettings settings);
    }
}