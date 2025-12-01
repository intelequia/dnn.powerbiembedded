using Dnn.PersonaBar.Library;
using Dnn.PersonaBar.Library.Attributes;
using DotNetNuke.PowerBI.Data.CapacitySettings;
using DotNetNuke.PowerBI.Data.CapacitySettings.Models;
using DotNetNuke.PowerBI.Services.Models;
using DotNetNuke.Web.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [MenuPermission(Scope = ServiceScope.Admin, MenuName = "Dnn.PowerBI")]
    public class CapacitySettingsController : DnnApiController
    {
        private readonly ICapacityManagementService _capacityManagementService;

        public CapacitySettingsController()
        {
            _capacityManagementService = new CapacityManagementService();
        }

        /// <summary>
        /// Gets all capacities for the current portal
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetCapacities()
        {
            try
            {
                var capacities = CapacitySettingsRepository.Instance.GetCapacities(PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, capacities);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Gets a specific capacity by ID
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetCapacity(int capacityId)
        {
            try
            {
                var capacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, PortalSettings.PortalId);
                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }
                return Request.CreateResponse(HttpStatusCode.OK, capacity);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Creates a new capacity
        /// </summary>
        [HttpPost]
        public HttpResponseMessage CreateCapacity(CapacitySettings capacity)
        {
            try
            {
                CapacitySettingsRepository.Instance.AddCapacity(capacity, PortalSettings.PortalId, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.Created, capacity);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing capacity
        /// </summary>
        [HttpPost]
        public HttpResponseMessage UpdateCapacity(CapacitySettings capacity)
        {
            try
            {
                var existingCapacity = CapacitySettingsRepository.Instance.GetCapacityById(capacity.CapacityId, PortalSettings.PortalId);
                if (existingCapacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }

                CapacitySettingsRepository.Instance.UpdateCapacity(capacity, PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, capacity);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a capacity
        /// </summary>
        [HttpPost]
        public HttpResponseMessage DeleteCapacity(int capacityId)
        {
            try
            {
                var existingCapacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, PortalSettings.PortalId);
                if (existingCapacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }

                CapacitySettingsRepository.Instance.DeleteCapacity(capacityId, PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Capacity deleted successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Gets the status of a specific capacity from Azure
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCapacityStatus(int capacityId)
        {
            try
            {
                var capacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, PortalSettings.PortalId);
                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }

                AzureCapacity azureCapacity = await _capacityManagementService.GetCapacityStatusAsync(capacity);
                if (azureCapacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found in Azure or error retrieving capacity status");
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    azureCapacity.DisplayName,
                    azureCapacity.State,
                    azureCapacity.Region,
                    azureCapacity.Sku
                });
            }
            catch (ArgumentException argEx)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, argEx.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Starts a capacity in Azure
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> StartCapacity(int capacityId)
        {
            try
            {
                var capacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, PortalSettings.PortalId);
                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }

                bool success = await _capacityManagementService.StartCapacityAsync(capacity);

                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Capacity started successfully" });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to start capacity");
                }
            }
            catch (ArgumentException argEx)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, argEx.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Pauses a capacity in Azure
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> PauseCapacity(int capacityId)
        {
            try
            {
                var capacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, PortalSettings.PortalId);
                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found");
                }

                bool success = await _capacityManagementService.PauseCapacityAsync(capacity);
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Capacity paused successfully" });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to pause capacity");
                }
            }
            catch (ArgumentException argEx)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, argEx.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
