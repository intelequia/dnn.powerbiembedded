using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.CapacitySettings.Models;
using DotNetNuke.PowerBI.Services.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Services
{
    public class CapacityManagementService : ICapacityManagementService
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(CapacityManagementService));
        private const string AzureManagementApiUrl = "https://management.azure.com";
        private const string AzureManagementResource = "https://management.core.windows.net/";

        /// <summary>
        /// Gets the access token for Azure Management API using Service Principal authentication
        /// </summary>
        private async Task<string> GetAccessTokenAsync(CapacitySettings settings)
        {
            try
            {
                // Validate Service Principal configuration
                if (string.IsNullOrEmpty(settings.ServicePrincipalApplicationId) ||
                    string.IsNullOrEmpty(settings.ServicePrincipalApplicationSecret) ||
                    string.IsNullOrEmpty(settings.ServicePrincipalTenant))
                {
                    throw new ArgumentException("Service Principal ApplicationId, ApplicationSecret and Tenant are required");
                }

                var authorityUrl = $"https://login.microsoftonline.com/{settings.ServicePrincipalTenant}";
                var authenticationContext = new AuthenticationContext(authorityUrl);
                var credential = new ClientCredential(
                    settings.ServicePrincipalApplicationId,
                    settings.ServicePrincipalApplicationSecret);

                var authenticationResult = await authenticationContext.AcquireTokenAsync(
                    AzureManagementResource,
                    credential);

                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.Error("Error getting access token for Azure Management API using Service Principal authentication", ex);
                throw;
            }
        }

        public async Task<AzureCapacity> GetCapacityStatusAsync(CapacitySettings settings)
        {
            try
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName))
                {
                    throw new ArgumentException("Azure Management API configuration is incomplete. Please configure SubscriptionId, ResourceGroup and CapacityName.");
                }

                var accessToken = await GetAccessTokenAsync(settings);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var url = $"{AzureManagementApiUrl}/subscriptions/{settings.AzureManagementSubscriptionId}/resourceGroups/{settings.AzureManagementResourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{settings.AzureManagementCapacityName}?api-version=2017-10-01";
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var azureCapacity = JsonConvert.DeserializeObject<AzureCapacityResponse>(content);

                        return new AzureCapacity
                        {
                            DisplayName = azureCapacity.Name,
                            State = azureCapacity.Properties.State.ToLower(),
                            Region = azureCapacity.Location,
                            Sku = azureCapacity.Sku.Name
                        };
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Logger.Error($"Error getting capacity status. Status: {response.StatusCode}, Content: {errorContent}");
                        throw new HttpRequestException($"Azure API returned {response.StatusCode}: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting capacity status for {settings?.AzureManagementCapacityName}", ex);
                throw;
            }
        }

        public async Task<bool> StartCapacityAsync(CapacitySettings settings)
        {
            try
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName))
                {
                    throw new ArgumentException("Azure Management API configuration is incomplete. Please configure SubscriptionId, ResourceGroup and CapacityName.");
                }

                var accessToken = await GetAccessTokenAsync(settings);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var url = $"{AzureManagementApiUrl}/subscriptions/{settings.AzureManagementSubscriptionId}/resourceGroups/{settings.AzureManagementResourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{settings.AzureManagementCapacityName}/resume?api-version=2017-10-01";
                    var response = await httpClient.PostAsync(url, null);

                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        Logger.Info($"Successfully started capacity {settings.AzureManagementCapacityName}");
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Logger.Error($"Error starting capacity. Status: {response.StatusCode}, Content: {errorContent}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error starting capacity {settings?.AzureManagementCapacityName}", ex);
                return false;
            }
        }

        public async Task<bool> PauseCapacityAsync(CapacitySettings settings)
        {
            try
            {
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName))
                {
                    throw new ArgumentException("Azure Management API configuration is incomplete. Please configure SubscriptionId, ResourceGroup and CapacityName.");
                }

                var accessToken = await GetAccessTokenAsync(settings);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var url = $"{AzureManagementApiUrl}/subscriptions/{settings.AzureManagementSubscriptionId}/resourceGroups/{settings.AzureManagementResourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/{settings.AzureManagementCapacityName}/suspend?api-version=2017-10-01";
                    var response = await httpClient.PostAsync(url, null);

                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        Logger.Info($"Successfully paused capacity {settings.AzureManagementCapacityName}");
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Logger.Error($"Error pausing capacity. Status: {response.StatusCode}, Content: {errorContent}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error pausing capacity {settings?.AzureManagementCapacityName}", ex);
                return false;
            }
        }

        public async Task<bool> IsCapacityRunningAsync(CapacitySettings settings)
        {
            var capacity = await GetCapacityStatusAsync(settings);
            return capacity != null && capacity.State == CapacityState.Active;
        }
    }

    #region Azure API Response Helper Classes

    // Helper classes for Azure API responses
    public class AzureCapacityResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public AzureCapacityProperties Properties { get; set; }
        public AzureCapacitySku Sku { get; set; }
    }

    public class AzureCapacitiesListResponse
    {
        public List<AzureCapacityResponse> Value { get; set; }
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
    #endregion
}
