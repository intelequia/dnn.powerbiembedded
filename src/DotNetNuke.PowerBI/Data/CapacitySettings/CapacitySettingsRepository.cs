using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.CapacitySettings.Models;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Data.CapacitySettings
{
    public class CapacitySettingsRepository : ServiceLocator<ICapacitySettingsRepository, CapacitySettingsRepository>, ICapacitySettingsRepository
    {
        public Models.CapacitySettings GetCapacityById(int capacityId, int portalId)
        {
            Requires.NotNegative("capacityId", capacityId);

            Models.CapacitySettings result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Models.CapacitySettings>();
                result = rep.GetById(capacityId, portalId);
            }
            return result;
        }

        public Models.CapacitySettings GetCapacityByName(string capacityName, int portalId)
        {
            Requires.NotNullOrEmpty("capacityName", capacityName);

            Models.CapacitySettings result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Models.CapacitySettings>();
                result = rep.Get(portalId).Where(x => x.CapacityName == capacityName && !x.IsDeleted).FirstOrDefault();
            }
            return result;
        }

        public List<Models.CapacitySettings> GetCapacities(int portalId)
        {
            List<Models.CapacitySettings> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Models.CapacitySettings>();
                result = rep.Find("WHERE PortalId = @0 AND IsDeleted = 0", portalId).ToList();
            }
            return result;
        }

        public List<Models.CapacitySettings> GetAllCapacities()
        {
            List<Models.CapacitySettings> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Models.CapacitySettings>();
                result = rep.Find("WHERE IsDeleted = 0").ToList();
            }
            return result;
        }

        public List<Models.CapacitySettings> GetEnabledCapacities(int portalId)
        {
            List<Models.CapacitySettings> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Models.CapacitySettings>();
                result = rep.Find("WHERE PortalId = @0 AND IsEnabled = 1 AND IsDeleted = 0", portalId).ToList();
            }
            return result;
        }

        public bool AddCapacity(Models.CapacitySettings capacity, int portalId, int? userId)
        {
            Requires.NotNull(capacity);
            Requires.NotNegative("PortalId", portalId);

            capacity.PortalId = portalId;
            capacity.CreatedOn = DateTime.Now;
            capacity.CreatedBy = userId ?? Components.Common.CurrentUser.UserID;
            capacity.ModifiedOn = DateTime.Now;
            capacity.ModifiedBy = capacity.CreatedBy;
            capacity.IsDeleted = false;

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Models.CapacitySettings>();
                repo.Insert(capacity);
            }

            return true;
        }

        public bool UpdateCapacity(Models.CapacitySettings capacity, int portalId)
        {
            Requires.NotNull(capacity);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Models.CapacitySettings>();

                var current = repo.GetById(capacity.CapacityId, portalId);
                if (current == null)
                {
                    throw new SecurityException("Can't update this Capacity - Different PortalId or not found");
                }
                
                current.CapacityName = capacity.CapacityName;
                current.CapacityDisplayName = capacity.CapacityDisplayName;
                current.Description = capacity.Description;
                current.ServicePrincipalApplicationId = capacity.ServicePrincipalApplicationId;
                current.ServicePrincipalApplicationSecret = capacity.ServicePrincipalApplicationSecret;
                current.ServicePrincipalTenant = capacity.ServicePrincipalTenant;
                current.AzureManagementSubscriptionId = capacity.AzureManagementSubscriptionId;
                current.AzureManagementResourceGroup = capacity.AzureManagementResourceGroup;
                current.AzureManagementCapacityName = capacity.AzureManagementCapacityName;
                current.AzureManagementPollingInterval = capacity.AzureManagementPollingInterval;
                current.DisabledCapacityMessage = capacity.DisabledCapacityMessage;
                current.IsEnabled = capacity.IsEnabled;
                current.ModifiedOn = DateTime.Now;
                current.ModifiedBy = Components.Common.CurrentUser.UserID;
                
                repo.Update(current);
            }

            return true;
        }

        public bool DeleteCapacity(int capacityId, int portalId)
        {
            Requires.NotNegative("capacityId", capacityId);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<Models.CapacitySettings>();

                var current = repo.GetById(capacityId, portalId);
                if (current == null)
                {
                    throw new SecurityException("Can't delete this Capacity - Different PortalId or not found");
                }
                if (current.PortalId != portalId)
                {
                    throw new SecurityException("Can't delete this Capacity - Different PortalId");
                }
                
                // Soft delete
                current.IsDeleted = true;
                current.ModifiedOn = DateTime.Now;
                current.ModifiedBy = Components.Common.CurrentUser.UserID;
                repo.Update(current);
                
                return true;
            }
        }

        protected override Func<ICapacitySettingsRepository> GetFactory()
        {
            return () => new CapacitySettingsRepository();
        }
    }
}
