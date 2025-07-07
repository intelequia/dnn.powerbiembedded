using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Services;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Controllers
{
    [SupportedModules("DotNetNuke.PowerBI")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public class CapacityManagementController : DnnApiController
    {
        private readonly ICapacityManagementService _capacityManagementService;

        public CapacityManagementController()
        {
            _capacityManagementService = new CapacityManagementService();
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetCapacityStatus(int settingsId)
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientId) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientSecret) ||
                    string.IsNullOrEmpty(settings.AzureManagementTenantId))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Azure Management API credentials are not configured");
                }

                var capacity = await _capacityManagementService.GetCapacityStatusAsync(
                    settings.AzureManagementSubscriptionId,
                    settings.AzureManagementResourceGroup,
                    settings.AzureManagementCapacityName,
                    settings.AzureManagementClientId,
                    settings.AzureManagementClientSecret,
                    settings.AzureManagementTenantId);

                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found or error retrieving capacity status");
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    capacity.Id,
                    capacity.DisplayName,
                    capacity.State,
                    capacity.Region,
                    capacity.Sku,
                    IsRunning = capacity.State == Microsoft.PowerBI.Api.Models.CapacityState.Active
                });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartCapacity(int settingsId)
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientId) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientSecret) ||
                    string.IsNullOrEmpty(settings.AzureManagementTenantId))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Azure Management API credentials are not configured");
                }

                var success = await _capacityManagementService.StartCapacityAsync(
                    settings.AzureManagementSubscriptionId,
                    settings.AzureManagementResourceGroup,
                    settings.AzureManagementCapacityName,
                    settings.AzureManagementClientId,
                    settings.AzureManagementClientSecret,
                    settings.AzureManagementTenantId);

                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Capacity started successfully" });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to start capacity");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PauseCapacity(int settingsId)
        {
            try
            {
                var settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                if (string.IsNullOrEmpty(settings.AzureManagementSubscriptionId) ||
                    string.IsNullOrEmpty(settings.AzureManagementResourceGroup) ||
                    string.IsNullOrEmpty(settings.AzureManagementCapacityName) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientId) ||
                    string.IsNullOrEmpty(settings.AzureManagementClientSecret) ||
                    string.IsNullOrEmpty(settings.AzureManagementTenantId))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Azure Management API credentials are not configured");
                }

                var success = await _capacityManagementService.PauseCapacityAsync(
                    settings.AzureManagementSubscriptionId,
                    settings.AzureManagementResourceGroup,
                    settings.AzureManagementCapacityName,
                    settings.AzureManagementClientId,
                    settings.AzureManagementClientSecret,
                    settings.AzureManagementTenantId);

                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Capacity paused successfully" });
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to pause capacity");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCapacityRules(int settingsId)
        {
            try
            {
                var rules = CapacityRulesRepository.Instance.GetRulesBySettingsId(settingsId, PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, rules);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateCapacityRule(CapacityRule rule)
        {
            try
            {
                rule.PortalId = PortalSettings.PortalId;
                rule.CreatedOn = DateTime.Now;
                rule.CreatedBy = UserInfo.UserID;
                rule.ModifiedOn = DateTime.Now;
                rule.ModifiedBy = UserInfo.UserID;
                rule.IsDeleted = false;

                CapacityRulesRepository.Instance.AddRule(rule);
                return Request.CreateResponse(HttpStatusCode.Created, rule);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateCapacityRule(CapacityRule rule)
        {
            try
            {
                var existingRule = CapacityRulesRepository.Instance.GetRuleById(rule.RuleId, PortalSettings.PortalId);
                if (existingRule == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Rule not found");
                }

                rule.PortalId = PortalSettings.PortalId;
                rule.ModifiedOn = DateTime.Now;
                rule.ModifiedBy = UserInfo.UserID;

                CapacityRulesRepository.Instance.UpdateRule(rule);
                return Request.CreateResponse(HttpStatusCode.OK, rule);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteCapacityRule(int ruleId)
        {
            try
            {
                var existingRule = CapacityRulesRepository.Instance.GetRuleById(ruleId, PortalSettings.PortalId);
                if (existingRule == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Rule not found");
                }

                CapacityRulesRepository.Instance.DeleteRule(ruleId, PortalSettings.PortalId);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Rule deleted successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}