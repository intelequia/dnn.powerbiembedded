using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var permissionsRepo = ObjectPermissionsRepository.Instance;

                // Remove unauthorized workspaces
                model.Workspaces.RemoveAll(x =>
                    !permissionsRepo.HasPermissions(x.Id, user.PortalID, 1, user));

                // Remove unauthorized reports
                model.Reports.RemoveAll(x => 
                    model.Workspaces.FirstOrDefault(c => c.Id == HttpUtility.ParseQueryString((new Uri(x.EmbedUrl)).Query)?.Get("groupId")) == null
                    ||
                    (!model.Workspaces.FirstOrDefault(c => c.Id == HttpUtility.ParseQueryString((new Uri(x.EmbedUrl)).Query)?.Get("groupId")).InheritPermissions
                        && !permissionsRepo.HasPermissions(x.Id, user.PortalID, 1, user))
                );


                // Remove unauthorized dashboards
                model.Dashboards.RemoveAll(x =>
                    model.Workspaces.FirstOrDefault(c => c.Id == HttpUtility.ParseQueryString((new Uri(x.EmbedUrl)).Query)?.Get("groupId")) == null
                    ||
                    (!model.Workspaces.FirstOrDefault(c => c.Id == HttpUtility.ParseQueryString((new Uri(x.EmbedUrl)).Query)?.Get("groupId")).InheritPermissions
                        && !permissionsRepo.HasPermissions(x.Id, user.PortalID, 1, user))
                );
            }
            return model;
        }

        public static List<PowerBISettings> RemoveUnauthorizedItems(this List<PowerBISettings> settings, UserInfo user)
        {
            if (settings != null && user != null)
            {
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                settings.RemoveAll(x =>
                    !permissionsRepo.HasPermissions(x.SettingsGroupId, user.PortalID, 1, user));
            }
            return settings;
        }
    }
}