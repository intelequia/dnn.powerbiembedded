using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.CapacityRules.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetNuke.PowerBI.Data.CapacityRules
{
    public class CapacityRulesRepository : ServiceLocator<ICapacityRulesRepository, CapacityRulesRepository>, ICapacityRulesRepository
    {
        public IEnumerable<CapacityRule> GetRulesByPortalId(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    CommandType.Text,
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE PortalId = @0 AND IsDeleted = 0",
                    portalId).ToList();
            }
        }

        public IEnumerable<CapacityRule> GetRulesByCapacityId(int capacityId, int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    CommandType.Text,
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE CapacityId = @0 AND PortalId = @1 AND IsDeleted = 0",
                    capacityId, portalId).ToList();
            }
        }

        public IEnumerable<CapacityRule> GetActiveRules(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    CommandType.Text,
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE PortalId = @0 AND IsEnabled = 1 AND IsDeleted = 0",
                    portalId).ToList();
            }
        }

        public CapacityRule GetRuleById(int ruleId, int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                return context.ExecuteQuery<CapacityRule>(
                    CommandType.Text,
                    "SELECT * FROM {databaseOwner}[{objectQualifier}PBI_CapacityRules] WHERE RuleId = @0 AND PortalId = @1 AND IsDeleted = 0",
                    ruleId, portalId).FirstOrDefault();
            }
        }

        public void AddRule(CapacityRule rule)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sql = @"INSERT INTO {databaseOwner}[{objectQualifier}PBI_CapacityRules] 
                           (PortalId, CapacityId, RuleName, RuleDescription, IsEnabled, RuleType, 
                            ScheduleExpression, ExecutionTime, DaysOfWeek, Action, TimeZoneId, 
                            CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, IsDeleted)
                           VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15)";

                context.Execute(
                    CommandType.Text,
                    sql,
                    rule.PortalId,
                    rule.CapacityId,
                    rule.RuleName,
                    rule.RuleDescription,
                    rule.IsEnabled,
                    rule.RuleType,
                    rule.ScheduleExpression,
                    rule.ExecutionTime,
                    rule.DaysOfWeek,
                    rule.Action,
                    rule.TimeZoneId,
                    DateTime.UtcNow,
                    rule.CreatedBy,
                    DateTime.UtcNow,
                    rule.ModifiedBy,
                    false);
            }
        }

        public void UpdateRule(CapacityRule rule)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sql = @"UPDATE {databaseOwner}[{objectQualifier}PBI_CapacityRules] 
                           SET CapacityId = @0,
                               RuleName = @1, 
                               RuleDescription = @2, 
                               IsEnabled = @3, 
                               RuleType = @4, 
                               ScheduleExpression = @5, 
                               ExecutionTime = @6, 
                               DaysOfWeek = @7, 
                               Action = @8, 
                               TimeZoneId = @9, 
                               LastExecutedOn = @10,
                               ModifiedOn = @11, 
                               ModifiedBy = @12
                           WHERE RuleId = @13 AND PortalId = @14";

                context.Execute(
                    CommandType.Text,
                    sql,
                    rule.CapacityId,
                    rule.RuleName,
                    rule.RuleDescription,
                    rule.IsEnabled,
                    rule.RuleType,
                    rule.ScheduleExpression,
                    rule.ExecutionTime,
                    rule.DaysOfWeek,
                    rule.Action,
                    rule.TimeZoneId,
                    rule.LastExecutedOn,
                    DateTime.UtcNow,
                    rule.ModifiedBy,
                    rule.RuleId,
                    rule.PortalId);
            }
        }

        public void DeleteRule(int ruleId, int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                context.Execute(
                    CommandType.Text,
                    "UPDATE {databaseOwner}[{objectQualifier}PBI_CapacityRules] SET IsDeleted = 1 WHERE RuleId = @0 AND PortalId = @1",
                    ruleId, portalId);
            }
        }

        public void DeleteRulesByCapacityId(int capacityId, int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                context.Execute(
                    CommandType.Text,
                    "UPDATE {databaseOwner}[{objectQualifier}PBI_CapacityRules] SET IsDeleted = 1 WHERE CapacityId = @0 AND PortalId = @1",
                    capacityId, portalId);
            }
        }

        protected override System.Func<ICapacityRulesRepository> GetFactory()
        {
            return () => new CapacityRulesRepository();
        }
    }
}