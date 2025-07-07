using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Data.CapacityRules
{
    public class CapacityRulesRepository : ServiceLocator<ICapacityRulesRepository, CapacityRulesRepository>, ICapacityRulesRepository
    {
        public IEnumerable<CapacityRule> GetRulesByPortalId(int portalId)
        {
            using (var context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE PortalId = @0 AND IsDeleted = 0",
                    portalId).ToList();
            }
        }

        public IEnumerable<CapacityRule> GetRulesBySettingsId(int settingsId, int portalId)
        {
            using (var context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE SettingsId = @0 AND PortalId = @1 AND IsDeleted = 0",
                    settingsId, portalId).ToList();
            }
        }

        public IEnumerable<CapacityRule> GetActiveRules(int portalId)
        {
            using (var context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE PortalId = @0 AND IsEnabled = 1 AND IsDeleted = 0",
                    portalId).ToList();
            }
        }

        public CapacityRule GetRuleById(int ruleId, int portalId)
        {
            using (var context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE RuleId = @0 AND PortalId = @1 AND IsDeleted = 0",
                    ruleId, portalId).FirstOrDefault();
            }
        }

        public void AddRule(CapacityRule rule)
        {
            using (var context = DataContext.Instance())
            {
                context.Insert(rule);
            }
        }

        public void UpdateRule(CapacityRule rule)
        {
            using (var context = DataContext.Instance())
            {
                context.Update(rule);
            }
        }

        public void DeleteRule(int ruleId, int portalId)
        {
            using (var context = DataContext.Instance())
            {
                context.Execute(
                    "UPDATE {databaseOwner}[{objectQualifier}PBI_CapacityRules] SET IsDeleted = 1 WHERE RuleId = @0 AND PortalId = @1",
                    ruleId, portalId);
            }
        }

        public void DeleteRulesBySettingsId(int settingsId, int portalId)
        {
            using (var context = DataContext.Instance())
            {
                context.Execute(
                    "UPDATE {databaseOwner}[{objectQualifier}PBI_CapacityRules] SET IsDeleted = 1 WHERE SettingsId = @0 AND PortalId = @1",
                    settingsId, portalId);
            }
        }

        protected override System.Func<ICapacityRulesRepository> GetFactory()
        {
            return () => new CapacityRulesRepository();
        }
    }
}