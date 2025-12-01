using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using System;
using System.Collections.Generic;

namespace DotNetNuke.PowerBI.Data.CapacityRules
{
    public interface ICapacityRulesRepository
    {
        IEnumerable<CapacityRule> GetRulesByPortalId(int portalId);
        IEnumerable<CapacityRule> GetRulesByCapacityId(int capacityId, int portalId);
        IEnumerable<CapacityRule> GetActiveRules(int portalId);
        CapacityRule GetRuleById(int ruleId, int portalId);
        void AddRule(CapacityRule rule);
        void UpdateRule(CapacityRule rule);
        void DeleteRule(int ruleId, int portalId);
        void DeleteRulesByCapacityId(int capacityId, int portalId);
    }
}