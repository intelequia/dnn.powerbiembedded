using System.Threading.Tasks;
using Microsoft.PowerBI.Api.Models;

namespace DotNetNuke.PowerBI.Services
{
    public interface ICapacityManagementService
    {
        Task<Capacity> GetCapacityStatusAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId);
        Task<bool> StartCapacityAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId);
        Task<bool> PauseCapacityAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId);
        Task<bool> IsCapacityRunningAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId);
    }
}