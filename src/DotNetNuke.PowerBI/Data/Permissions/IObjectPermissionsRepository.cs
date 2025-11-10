using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.Models;
using System.Linq;

namespace DotNetNuke.PowerBI.Data
{
    public interface IObjectPermissionsRepository
    {
        IQueryable<ObjectPermission> GetObjectPermissions(string powerBiObjectId, int portalId);
        IQueryable<ObjectPermission> GetObjectPermissionsExtended(string powerBiObjectId, int portalId);
        IQueryable<ObjectPermission> GetObjectPermissionsByPortal(int portalId);
        IQueryable<ObjectPermission> GetObjectPermissionsByPortalExtended(int portalId);
        ObjectPermission GetObjectPermission(string powerBiObjectId, int portalId, int permissionId);
        bool CreateObjectPermission(string powerBiObjectId, int permissionId, bool allowAccess, int portalId, int? roleId, int? userId);
        bool SaveObjectPermission(string powerBiObjectId, int permissionId, bool allowAccess, int portalId, int? roleId, int? userId);
        bool DeleteObjectPermission(string powerBiObjectId, int portalId, int permissionId);
        bool DeleteObjectPermissions(string powerBiObjectId, int portalId);
        bool DeleteObjectPermissions(int portalId);
        bool HasPermissions(string powerBiObjectId, int portalId, int permissionId, UserInfo user);
    }
}