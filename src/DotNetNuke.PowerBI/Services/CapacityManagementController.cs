using Dnn.PersonaBar.Library;
using Dnn.PersonaBar.Library.Attributes;
using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Services.Models;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [MenuPermission(Scope = ServiceScope.Admin, MenuName = "Dnn.PowerBI")]
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
                PowerBISettings settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                AzureCapacity capacity = await _capacityManagementService.GetCapacityStatusAsync(settings);
                if (capacity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Capacity not found or error retrieving capacity status");
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    capacity.DisplayName,
                    capacity.State,
                    capacity.Region,
                    capacity.Sku
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

        [HttpPost]
        public async Task<HttpResponseMessage> StartCapacity(int settingsId)
        {
            try
            {
                PowerBISettings settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                bool success = await _capacityManagementService.StartCapacityAsync(settings);

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

        [HttpPost]
        public async Task<HttpResponseMessage> PauseCapacity(int settingsId)
        {
            try
            {
                PowerBISettings settings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, PortalSettings.PortalId);
                if (settings == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Settings not found");
                }

                bool success = await _capacityManagementService.PauseCapacityAsync(settings);
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

        [HttpGet]
        public HttpResponseMessage GetCapacityRules(int settingsId)
        {
            try
            {
                IEnumerable<CapacityRule> rules = CapacityRulesRepository.Instance.GetRulesBySettingsId(settingsId, PortalSettings.PortalId);
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

        [HttpPost]
        public HttpResponseMessage UpdateCapacityRule(CapacityRule rule)
        {
            try
            {
                CapacityRule existingRule = CapacityRulesRepository.Instance.GetRuleById(rule.RuleId, PortalSettings.PortalId);
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

        [HttpPost]
        public HttpResponseMessage DeleteCapacityRule(int ruleId)
        {
            try
            {
                CapacityRule existingRule = CapacityRulesRepository.Instance.GetRuleById(ruleId, PortalSettings.PortalId);
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