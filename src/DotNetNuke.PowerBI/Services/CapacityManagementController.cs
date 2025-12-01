using Dnn.PersonaBar.Library;
using Dnn.PersonaBar.Library.Attributes;
using DotNetNuke.PowerBI.Data.CapacityRules;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [MenuPermission(Scope = ServiceScope.Admin, MenuName = "Dnn.PowerBI")]
    public class CapacityManagementController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCapacityRules(int capacityId)
        {
            try
            {
                IEnumerable<CapacityRule> rules = CapacityRulesRepository.Instance.GetRulesByCapacityId(capacityId, PortalSettings.PortalId);
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