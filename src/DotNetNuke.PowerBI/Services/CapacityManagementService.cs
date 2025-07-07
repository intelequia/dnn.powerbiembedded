using DotNetNuke.Instrumentation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Services
{
    public class CapacityManagementService : ICapacityManagementService
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(CapacityManagementService));
        private const string AzureManagementApiUrl = "https://management.azure.com";
        private const string AzureManagementResource = "https://management.core.windows.net/";

        private async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string tenantId)
        {
            var authorityUrl = $"https://login.microsoftonline.com/{tenantId}";
            var authenticationContext = new AuthenticationContext(authorityUrl);
            var credential = new ClientCredential(clientId, clientSecret);
            
            try
            {
                var authenticationResult = await authenticationContext.AcquireTokenAsync(AzureManagementResource, credential);
                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.Error("Error getting access token for Azure Management API", ex);
                throw;
            }
        }

        public async Task<Capacity> GetCapacityStatusAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync(clientId, clientSecret, tenantId);
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    
                    var url = $"{AzureManagementApiUrl}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{capacityName}?api-version=2017-10-01";
                    var response = await httpClient.GetAsync(url);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var azureCapacity = JsonConvert.DeserializeObject<AzureCapacityResponse>(content);
                        
                        // Convert Azure capacity response to PowerBI Capacity model
                        return new Capacity
                        {
                            Id = Guid.Parse(azureCapacity.Id.Split('/')[^1]),
                            DisplayName = azureCapacity.Name,
                            State = ConvertAzureStateToCapacityState(azureCapacity.Properties.State),
                            Region = azureCapacity.Location,
                            Sku = azureCapacity.Sku.Name
                        };
                    }
                    else
                    {
                        Logger.Error($"Error getting capacity status. Status: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error getting capacity status", ex);
                return null;
            }
        }

        public async Task<bool> StartCapacityAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync(clientId, clientSecret, tenantId);
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    
                    var url = $"{AzureManagementApiUrl}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{capacityName}/resume?api-version=2017-10-01";
                    var response = await httpClient.PostAsync(url, null);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Info($"Successfully started capacity {capacityName}");
                        return true;
                    }
                    else
                    {
                        Logger.Error($"Error starting capacity. Status: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error starting capacity", ex);
                return false;
            }
        }

        public async Task<bool> PauseCapacityAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync(clientId, clientSecret, tenantId);
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    
                    var url = $"{AzureManagementApiUrl}/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{capacityName}/suspend?api-version=2017-10-01";
                    var response = await httpClient.PostAsync(url, null);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Info($"Successfully paused capacity {capacityName}");
                        return true;
                    }
                    else
                    {
                        Logger.Error($"Error pausing capacity. Status: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error pausing capacity", ex);
                return false;
            }
        }

        public async Task<bool> IsCapacityRunningAsync(string subscriptionId, string resourceGroup, string capacityName, string clientId, string clientSecret, string tenantId)
        {
            var capacity = await GetCapacityStatusAsync(subscriptionId, resourceGroup, capacityName, clientId, clientSecret, tenantId);
            return capacity != null && capacity.State == CapacityState.Active;
        }

        private CapacityState ConvertAzureStateToCapacityState(string azureState)
        {
            switch (azureState?.ToLower())
            {
                case "succeeded":
                case "online":
                    return CapacityState.Active;
                case "paused":
                case "suspended":
                    return CapacityState.Paused;
                case "provisioning":
                case "updating":
                    return CapacityState.UpdatingSku;
                default:
                    return CapacityState.Unknown;
            }
        }
    }

    // Helper classes for Azure API responses
    public class AzureCapacityResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public AzureCapacityProperties Properties { get; set; }
        public AzureCapacitySku Sku { get; set; }
    }

    public class AzureCapacityProperties
    {
        public string State { get; set; }
        public string ProvisioningState { get; set; }
    }

    public class AzureCapacitySku
    {
        public string Name { get; set; }
        public string Tier { get; set; }
        public int Capacity { get; set; }
    }
}