using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Web;

namespace DotNetNuke.PowerBI.Models
{
    public static class PowerBIListViewExtensions
    {
        private const string LanguageRegularExpression = "(\\([a-zA-Z]{2}-[a-zA-z]{2}\\))";

        public static PowerBIListView RemoveOtherCultureItems(this PowerBIListView model)
        {
            if (model != null)
            {
                model.Reports.RemoveAll(x =>
                    Regex.Match(x.Name, LanguageRegularExpression, RegexOptions.IgnoreCase).Success
                    && !x.Name.ToLowerInvariant()
                        .Contains(
                            $"({System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()})"));
                model.Dashboards.RemoveAll(x =>
                    Regex.Match(x.DisplayName, LanguageRegularExpression, RegexOptions.IgnoreCase).Success
                    && !x.DisplayName.ToLowerInvariant()
                        .Contains(
                            $"({System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()})"));
                model.Reports.ForEach(r =>
                    r.Name = Regex.Replace(r.Name, LanguageRegularExpression, "", RegexOptions.IgnoreCase));
                model.Dashboards.ForEach(r =>
                    r.DisplayName = Regex.Replace(r.DisplayName, LanguageRegularExpression, "",
                        RegexOptions.IgnoreCase));
            }
            return model;
        }

        public static PowerBIListView RemoveUnauthorizedItems(this PowerBIListView model, UserInfo user)
        {
            if (model != null && user != null)
            {
                if (user.IsSuperUser || user.IsInRole("Administrators")) {
                    return model;
                }
                var permissionsRepo = ObjectPermissionsRepository.Instance;

                // Remove unauthorized workspaces
                model.Workspaces.RemoveAll(x =>
                    !permissionsRepo.HasPermissions(x.Id, user.PortalID, 1, user));

                // Remove unauthorized reports
                string guidPattern = "[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?";
                string urlPattern = $"https://app.powerbi.com/groups/(?<groupId>{guidPattern})/(reports|rdlreports)/(?<reportId>{guidPattern}).*";
                List<Microsoft.PowerBI.Api.Models.Report> reportsToRemove = new List<Microsoft.PowerBI.Api.Models.Report>();
                foreach (var report in model.Reports)
                {
                    var match = Regex.Match(report.WebUrl, urlPattern, RegexOptions.IgnoreCase);
                    if (!match.Success)
                    {
                        reportsToRemove.Add(report);
                    }
                    else if (model.Workspaces.FirstOrDefault(c => c.Id == match.Groups["groupId"]?.Value) == null
                        ||
                        (!model.Workspaces.FirstOrDefault(c => c.Id == match.Groups["groupId"]?.Value).InheritPermissions
                            && !permissionsRepo.HasPermissions(report.Id.ToString(), user.PortalID, 1, user)))
                    {
                        reportsToRemove.Add(report);
                    }
                }
                model.Reports.RemoveAll(x => reportsToRemove.Contains(x));

                // Remove unauthorized dashboards
                model.Dashboards.RemoveAll(x =>
                    model.Workspaces.FirstOrDefault(c => c.Id == model.WorkspaceId) == null
                    ||
                    (!model.Workspaces.FirstOrDefault(c => c.Id == model.WorkspaceId).InheritPermissions
                        && !permissionsRepo.HasPermissions(x.Id.ToString(), user.PortalID, 1, user))
                );
            }
            return model;
        }

        public static List<PowerBISettings> RemoveUnauthorizedItems(this List<PowerBISettings> settings, UserInfo user)
        {
            if (user.IsSuperUser || user.IsInRole("Administrators")) {
                return settings;
            }
            if (settings != null && user != null)
            {
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                settings.RemoveAll(x =>
                    !permissionsRepo.HasPermissions(x.SettingsGroupId, user.PortalID, 1, user));
            }
            return settings;
        }

        public static bool UserHasPermissionsToWorkspace(string settingsGroupId, UserInfo user)
        {
            return user.IsSuperUser
                || user.IsInRole("Administrators")
                || ObjectPermissionsRepository.Instance.HasPermissions(settingsGroupId, user.PortalID, 1, user);
        }
    }
}