using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.Security.Roles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Data
{
    public class ObjectPermissionsRepository : ServiceLocator<IObjectPermissionsRepository, ObjectPermissionsRepository>, IObjectPermissionsRepository
    {
        protected override Func<IObjectPermissionsRepository> GetFactory()
        {
            return () => new ObjectPermissionsRepository();
        }

        public ObjectPermission GetObjectPermission(string powerBiObjectId, int portalId, int permissionId)
        {
            Requires.NotNullOrEmpty("powerBiObjectId", powerBiObjectId);

            ObjectPermission result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ObjectPermission>();
                result = rep.Find("WHERE PowerBiObjectID = @0 AND PermissionID = @1 AND PortalId = @2", powerBiObjectId, permissionId, portalId).FirstOrDefault();
            }
            return result;
        }

        public IQueryable<ObjectPermission> GetObjectPermissionsByPortal(int portalId)
        {
            IQueryable<ObjectPermission> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ObjectPermission>();
                result = rep.Find("WHERE PortalId = @0", portalId).AsQueryable();
            }
            return result;
        }

        public IQueryable<ObjectPermission> GetObjectPermissionsByPortalExtended(int portalId)
        {
            var portalUsers = UserController.GetUsers(portalId).Cast<UserInfo>().ToList();
            var portalRoles = Security.Roles.RoleController.Instance.GetRoles(portalId);
            var result = GetObjectPermissionsByPortal(portalId);

            foreach (var item in result)
            {
                if (item.UserID != null)
                {
                    item.UserDisplayName = portalUsers.Where(user => user.UserID == item.UserID).FirstOrDefault().DisplayName;
                }
                else
                {
                    item.RoleName = portalRoles.Where(role => role.RoleID == item.RoleID).FirstOrDefault().RoleName;
                }
            }

            return result;
        }

        public IQueryable<ObjectPermission> GetObjectPermissions(string powerBiObjectId, int portalId)
        {
            Requires.NotNullOrEmpty("powerBiObjectId", powerBiObjectId);

            IQueryable<ObjectPermission> result;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ObjectPermission>();
                result = rep.Find("WHERE PowerBiObjectID = @0 AND PortalId = @1", powerBiObjectId, portalId).AsQueryable();
            }
            return result;
        }

        public IQueryable<ObjectPermission> GetObjectPermissionsExtended(string powerBiObjectId, int portalId)
        {
            var portalUsers = UserController.GetUsers(portalId).Cast<UserInfo>().ToList();
            var portalRoles = Security.Roles.RoleController.Instance.GetRoles(portalId);
            var result = GetObjectPermissions(powerBiObjectId, portalId);

            foreach (var item in result)
            {
                if (item.UserID != null)
                {
                    item.UserDisplayName = portalUsers.Where(user => user.UserID == item.UserID).FirstOrDefault().DisplayName;
                }
                else
                {
                    item.RoleName = portalRoles.Where(role => role.RoleID == item.RoleID).FirstOrDefault().RoleName;
                }
            }

            return result;
        }

        public bool CreateObjectPermission(string powerBiObjectId, int permissionId, bool allowAccess, int portalId, int? roleId, int? userId)
        {
            Requires.NotNullOrEmpty("PowerBiObjectId", powerBiObjectId);

            var permission = new ObjectPermission
            {
                ID = Guid.NewGuid(),
                PowerBiObjectID = powerBiObjectId,
                PermissionID = permissionId,
                AllowAccess = allowAccess,
                PortalID = portalId,
                RoleID = roleId,
                UserID = userId,
                CreatedByUserID = Components.Common.CurrentUser.UserID,
                CreatedOnDate = DateTime.Now,
                LastModifiedByUserID = Components.Common.CurrentUser.UserID,
                LastModifiedOnDate = DateTime.Now
            };

            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ObjectPermission>();
                rep.Insert(permission);
            }

            return true;
        }

        public bool SaveObjectPermission(string powerBiObjectId, int permissionId, bool allowAccess, int portalId, int? roleId, int? userId)
        {
            Requires.NotNullOrEmpty("PowerBiObjectId", powerBiObjectId);

            var permission = GetObjectPermission(powerBiObjectId, portalId, permissionId);
            if (permission == null)
            {
                CreateObjectPermission(powerBiObjectId, permissionId, allowAccess, portalId, roleId, userId);
            }
            else
            {
                permission.AllowAccess = allowAccess;
                permission.PortalID = portalId;
                permission.RoleID = roleId;
                permission.UserID = userId;
                permission.LastModifiedByUserID = Components.Common.CurrentUser.UserID;
                permission.LastModifiedOnDate = DateTime.Now;
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<ObjectPermission>();
                    rep.Update(permission);
                }
            }

            return true;
        }

        public bool DeleteObjectPermission(string powerBiObjectId, int portalId, int permissionId)
        {
            Requires.NotNullOrEmpty("PowerBiObjectId", powerBiObjectId);

            var permission = GetObjectPermission(powerBiObjectId, portalId, permissionId);
            if (permission != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<ObjectPermission>();
                    rep.Delete(permission);
                }
            }

            return true;
        }

        public bool DeleteObjectPermissions(string powerBiObjectId, int portalId)
        {
            Requires.NotNullOrEmpty("PowerBiObjectId", powerBiObjectId);

            var permissions = GetObjectPermissions(powerBiObjectId, portalId);
            foreach (var permission in permissions)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<ObjectPermission>();
                    rep.Delete(permission);
                }
            }

            return true;
        }
        public bool DeleteObjectPermissions(int portalId)
        {
            var permissions = GetObjectPermissionsByPortal(portalId);
            foreach (var permission in permissions)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<ObjectPermission>();
                    rep.Delete(permission);
                }
            }

            return true;
        }

        public bool HasPermissions(string powerBiObjectId, int portalId, int permissionId, UserInfo user)
        {
            Requires.NotNullOrEmpty("PowerBiObjectId", powerBiObjectId);

            if (user.IsSuperUser)
            {
                return true;
            }

            var roles = RoleController.Instance.GetRoles(portalId, x => user.Roles.Contains(x.RoleName));
            var permissions = GetObjectPermissions(powerBiObjectId, portalId);
            return permissions.Any(x =>
                x.PermissionID == permissionId
                && x.AllowAccess
                && (
                    (x.UserID.HasValue && x.UserID.Value == user.UserID)
                    ||
                    roles.Any(y => y.RoleID == x.RoleID)
                    )
            );
        }
    }
}