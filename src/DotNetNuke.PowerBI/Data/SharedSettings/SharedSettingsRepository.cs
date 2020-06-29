using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.PowerBI.Data.SharedSettings
{
    public class SharedSettingsRepository : ServiceLocator<ISharedSettingsRepository, SharedSettingsRepository>, ISharedSettingsRepository
    {
        public PowerBISettings GetSettingsById(int settingsId, int portalId)
        {
            Requires.NotNegative("settingsId", settingsId);

            PowerBISettings result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PowerBISettings>();
                result = rep.GetById(settingsId, portalId);
            }
            return result;
        }

        public PowerBISettings GetSettingsByGroupId(string settingsGroupId, int portalId)
        {
            Requires.NotNullOrEmpty("settingsGroupId", settingsGroupId);

            PowerBISettings result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PowerBISettings>();
                result = rep.Get(portalId).Where(x => x.SettingsGroupId == settingsGroupId).FirstOrDefault();
            }
            return result;
        }

        public List<PowerBISettings> GetSettings(int portalId)
        {
            List<PowerBISettings> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PowerBISettings>();
                result = rep.Find("WHERE PortalId= @0", portalId).ToList();
            }
            return result;
        }

        public bool AddSettings(PowerBISettings setting, int portalId, int? userId)
        {
            Requires.NotNull(setting);
            Requires.NotNegative("PortalId", portalId);

            setting.PortalId = portalId;
            setting.CreatedOn = DateTime.Now;
            setting.CreatedBy = userId ?? Components.Common.CurrentUser.UserID;
            setting.ModifiedOn = DateTime.Now;
            setting.ModifiedBy = setting.CreatedBy;

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<PowerBISettings>();
                repo.Insert(setting);
            }

            return true;
        }

        public bool UpdateSettings(PowerBISettings settings, int portalId)
        {
            Requires.NotNull(settings);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<PowerBISettings>();

                var current = repo.GetById(settings.SettingsId, portalId);
                if (current == null)
                {
                    throw new SecurityException("Can't update this Settings - Different PortalId");
                }
                current.SettingsGroupId = settings.SettingsGroupId;
                current.SettingsGroupName = settings.SettingsGroupName;
                current.AuthenticationType = settings.AuthenticationType;
                current.Username = settings.Username;
                current.Password = settings.Password;
                current.ServicePrincipalApplicationId = settings.ServicePrincipalApplicationId;
                current.ServicePrincipalApplicationSecret = settings.ServicePrincipalApplicationSecret;
                current.ServicePrincipalTenant = settings.ServicePrincipalTenant;
                current.ApplicationId = settings.ApplicationId;
                current.WorkspaceId = settings.WorkspaceId;
                current.ContentPageUrl = settings.ContentPageUrl;
                current.ModifiedOn = DateTime.Now;
                current.ModifiedBy = Components.Common.CurrentUser.UserID;
                repo.Update(current);
            }

            return true;
        }


        public bool DeleteSetting(int settingId, int portalId)
        {
            Requires.NotNegative("settingId", settingId);
            Requires.NotNegative("portalId", portalId);

            using (var ctx = DataContext.Instance())
            {
                var repo = ctx.GetRepository<PowerBISettings>();

                var current = repo.GetById(settingId, portalId);
                if (current.PortalId != portalId)
                {
                    throw new SecurityException("Can't delete this Settings - Different PortalId");
                }
                repo.Delete(current);
                return true;
            }
        }

        protected override Func<ISharedSettingsRepository> GetFactory()
        {
            return () => new SharedSettingsRepository();
        }
    }
}