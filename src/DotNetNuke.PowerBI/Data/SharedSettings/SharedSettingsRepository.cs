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

        public bool SaveSettings(PowerBISettings setting, int portalId, int? userId)
        {
            Requires.NotNull(setting);
            Requires.PropertyNotNegative(setting, "PortalId");

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
                if (current.PortalId != settings.PortalId)
                {
                    throw new SecurityException("Can't update this Settings - Different PortalId");
                }
                settings.ModifiedOn = DateTime.Now;
                settings.ModifiedBy = Components.Common.CurrentUser.UserID;
                repo.Update(settings);
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