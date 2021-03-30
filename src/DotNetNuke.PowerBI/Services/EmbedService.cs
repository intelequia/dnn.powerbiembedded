using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Services.Cache;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetNuke.Entities.Portals;
using DotNetNuke.PowerBI.Data.Bookmarks;

namespace DotNetNuke.PowerBI.Services
{
    public class EmbedService : IEmbedService
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EmbedService));
        private EmbedConfig embedConfig;
        private PowerBISettings powerBISettings;
        private TileEmbedConfig tileEmbedConfig;
        private TokenCredentials tokenCredentials;

        public EmbedConfig EmbedConfig
        {
            get { return embedConfig; }
        }

        public TileEmbedConfig TileEmbedConfig
        {
            get { return tileEmbedConfig; }
        }
        public PowerBISettings Settings
        {
            get { return powerBISettings; }
        }


        public EmbedService(int portalId, int tabModuleId)
        {
            tokenCredentials = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
        }

        public EmbedService(int portalId, int tabModuleId, int settingsId)
        {
            tokenCredentials = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            if (settingsId == 0)
            {
                powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
            }
            else
            {
                powerBISettings = SharedSettingsRepository.Instance.GetSettingsById(settingsId, portalId);
            }
        }

        public EmbedService(int portalId, int tabModuleId, string settingsGroupId)
        {
            tokenCredentials = null;
            embedConfig = new EmbedConfig();
            tileEmbedConfig = new TileEmbedConfig();
            if (string.IsNullOrEmpty(settingsGroupId))
            {
                powerBISettings = PowerBISettings.GetPortalPowerBISettings(portalId, tabModuleId);
            }
            else
            {
                powerBISettings = SharedSettingsRepository.Instance.GetSettingsByGroupId(settingsGroupId, portalId);
            }
        }

        public async Task<PowerBIListView> GetContentListAsync(int userId)
        {
            var model = (PowerBIListView)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_PowerBIListView");
            if (model != null)
                return model;

            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                return null;
            }
            model = new PowerBIListView()
            {
                WorkspaceId = Settings.WorkspaceId
            };

            var pbiSettings = SharedSettingsRepository.Instance.GetSettings(Settings.PortalId);
            foreach (var s in pbiSettings)
            {
                model.Workspaces.Add(new Workspace()
                {
                    Id = s.SettingsGroupId,
                    Name = s.SettingsGroupName,
                    SettingsId = s.SettingsId,
                    InheritPermissions = s.InheritPermissions
                });
            }

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(Settings.ApiUrl), tokenCredentials))
            {
                var dashboards = client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).GetAwaiter().GetResult();
                model.Dashboards.AddRange(dashboards.Value);

                // Get a list of reports.
                var reports = client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).GetAwaiter().GetResult();
                var cleanedReports = CleanUsageReports(reports.Value.ToList());
                model.Reports.AddRange(cleanedReports);
            }
            CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_PowerBIListView", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            return model;
        }

        private List<Report> CleanUsageReports(List<Report> reports)
        {
            if (reports.Any(r => r.Name.Contains("Usage Metrics Report")))
            {
                var i = reports.RemoveAll(r => r.Name.Contains("Usage Metrics Report"));
            }
            return reports;
        }

        public async Task<PowerBICalendarView> GetScheduleInGroup(string mode)
        {

            var model = (PowerBICalendarView)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{Thread.CurrentThread.CurrentUICulture.Name}_CalendarDataSet_{mode}");
            if (model != null)
                return model;

            model = new PowerBICalendarView();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                return null;
            }

            var colours = new List<string>
            {
                "#1ab394",
                "#364B45",
                "#98B0A9",
                "#A993E2",
                "#7360AA"
            };
            
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(Settings.ApiUrl), tokenCredentials))
            {
                if (mode == "-1")
                {
                    //Mode = 0 schedule for all workspaces
                    var pbiSettings = SharedSettingsRepository.Instance.GetSettings(Settings.PortalId);
                    foreach (var s in pbiSettings)
                    {
                        model.Workspaces.Add(s.WorkspaceId);
                    }
                }
                else
                {
                    //Schedule for currect workspace
                    model.Workspaces = new List<string> {Settings.WorkspaceId};
                }

                var groups = client.Groups.GetGroupsAsync().GetAwaiter().GetResult().Value;
                var capacities = client.Capacities.GetCapacitiesAsync().GetAwaiter().GetResult().Value;


                foreach (var workspace in model.Workspaces)
                {
                    //Get Schedule datasets
                    var datasets = client.Datasets.GetDatasetsInGroupAsync(Guid.Parse(workspace)).GetAwaiter().GetResult().Value;
                    datasets = CleanUsageDatasets(datasets.ToList());

                    var group = groups.FirstOrDefault(g => g.Id.ToString() == workspace.ToLowerInvariant());
                    var capacity = capacities.FirstOrDefault(c => c.Id == group.CapacityId);


                    for (var x = 0; x < datasets.Count; x++)
                    {
                        var dataset = datasets[x];

                        try
                        {
                            //Get refreshes history
                            var history = client.Datasets.GetRefreshHistoryAsync(Guid.Parse(Settings.WorkspaceId), dataset.Id, 100).GetAwaiter().GetResult().Value.ToList();

                            foreach (var refresh in history)
                            {
                                model.History.Add(new RefreshedDataset
                                {
                                    Dataset = dataset.Name,
                                    WorkSpaceName = group.Name,
                                    CapacityName = capacity.DisplayName,
                                    StartTime = refresh.StartTime,
                                    EndTime = refresh.EndTime,
                                    RefreshType = refresh.RefreshType,
                                    RequestId = refresh.RequestId,
                                    ServiceExceptionJson = refresh.ServiceExceptionJson,
                                    Status = refresh.Status
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Getting "UnsupportedMediaType"
                        }
                        try
                        {
                            //Get refresh Schedule by dataset and Workspace
                            var schedule = client.Datasets.GetRefreshScheduleInGroupAsync(Guid.Parse(Settings.WorkspaceId), dataset.Id)
                                .GetAwaiter().GetResult();
                            var timeRange = new List<Schedule>();
                            string startHour = schedule.Times[0];
                            string endHour = schedule.Times[0];

                            //Grouping of consecutive hours
                            for (int i = 0; i < schedule.Times.Count; i++)
                            {
                                if (schedule.Times[i] != schedule.Times.Last())
                                {
                                    DateTime hour = DateTime.ParseExact(schedule.Times[i], "HH:mm",
                                        CultureInfo.InvariantCulture);
                                    DateTime nextHour = DateTime.ParseExact(schedule.Times[i + 1], "HH:mm",
                                        CultureInfo.InvariantCulture);
                                    if (nextHour.Hour == hour.Hour + 1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        endHour = hour.AddHours(1).ToString("HH:mm");
                                        timeRange.Add(new Schedule
                                        {
                                            start = startHour,
                                            end = endHour
                                        });
                                        startHour = schedule.Times[i + 1];
                                        endHour = schedule.Times[i + 1];
                                    }
                                }
                                else
                                {
                                    endHour = DateTime.ParseExact(schedule.Times[i], "HH:mm",
                                        CultureInfo.InvariantCulture).AddHours(1).ToString("HH:mm");
                                    timeRange.Add(new Schedule
                                    {
                                        start = startHour,
                                        end = endHour
                                    });
                                }
                            }
                            //Create calendar event by each date and hour group
                            for (var index = 0; index < schedule.Days.Count; index++)
                            {
                                var day = schedule.Days[index];
                                var dayOfWeek = day.GetValueOrDefault().ToInteger();
                                for (var i = 0; i < timeRange.Count; i++)
                                {
                                    var time = timeRange[i];
                                    var id = Guid.NewGuid().ToString("N");
                                    var item = new CalendarItem
                                    {
                                        id = id,
                                        color = colours[x < datasets.Count ? x : 0],
                                        start = getCalendarDateTime(dayOfWeek, time.start),
                                        end = getCalendarDateTime(dayOfWeek, time.end),
                                        title = dataset.Name,                                        
                                    };
                                    item.description = $"Workspace: {group.Name}; Capacity: {capacity.DisplayName}; Dataset: {dataset.Name}; Start: {time.start}";
                                    model.RefreshSchedules.Add(item);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Warn("Error getting refresh schedule by dataset and workspace", e);
                            continue;
                        }
                        
                    }
                }
                //Order history
                model.History = model.History.OrderByDescending(dataset => dataset.StartTime).ToList();

                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{Thread.CurrentThread.CurrentUICulture.Name}_CalendarDataSet_{mode}", model, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
            }
            return model;
        }

        private List<Dataset> CleanUsageDatasets(List<Dataset> datasets)
        {
            if (datasets.Any(r => r.Name.Contains("Report Usage Metrics Model")))
            {
                var i = datasets.RemoveAll(r => r.Name.Contains("Report Usage Metrics Model"));
            }
            return datasets;
        }

        private string getCalendarDateTime(int DayOfWeek, string time)
        {
            var baseDate = new DateTime(2006, 1, 2);
            var result = baseDate.AddDays(DayOfWeek);
            result = result.AddHours(int.Parse(time.Substring(0, time.IndexOf(":"))));
            result = result.AddMinutes(int.Parse(time.Substring(time.IndexOf(":") + 1)));

            return result.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public async Task<EmbedConfig> GetReportEmbedConfigAsync(int userId, string username, string roles, string reportId)
        {
            var model = (EmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Report_{reportId}");
            if (model != null)
                return model;

            model = new EmbedConfig();

            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(Settings.ApiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var reports = await client.Reports.GetReportsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false);

                // No reports retrieved for the given workspace.
                if (reports.Value.Count() == 0)
                {
                    model.ErrorMessage = "No reports were found in the workspace";
                }
                var report = reports.Value.FirstOrDefault(r => r.Id.ToString().Equals(reportId, StringComparison.InvariantCultureIgnoreCase));
                if (report == null)
                {
                    model.ErrorMessage = "No report with the given ID was found in the workspace. Make sure ReportId is valid.";
                }

                //Delete bookmarks is report is modified
                CheckBookmarks(report,Settings.PortalId);

                // Generate Embed Token for reports without effective identities.
                GenerateTokenRequest generateTokenRequestParameters;
                // This is how you create embed token with effective identities
                if (!string.IsNullOrWhiteSpace(username))
                {
                    // Check if the dataset has effective identity required
                    // var dataset = await client.Datasets.GetDatasetAsync(report.DatasetId).ConfigureAwait(false);
                    // The line above returns an unauthorization exception when using "Service Principal" credentials. Seems a bug in the PowerBI API.
                    var dataset = client.Datasets.GetDatasets(Guid.Parse(Settings.WorkspaceId)).Value.FirstOrDefault(x => x.Id == report.DatasetId);
                    if (dataset != null 
                        && (dataset.IsEffectiveIdentityRequired.GetValueOrDefault(false) || dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false))
                        && !dataset.IsOnPremGatewayRequired.GetValueOrDefault(false))
                    {
                        var rls = new EffectiveIdentity(username, new List<string> { report.DatasetId });
                        if (!string.IsNullOrWhiteSpace(roles) && dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false))
                        {
                            var rolesList = new List<string>();
                            rolesList.AddRange(roles.Split(','));
                            rls.Roles = rolesList;
                        }
                        // Generate Embed Token with effective identities.
                        generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view", identities: new List<EffectiveIdentity> { rls });
                    }
                    else
                    {
                        generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                    }
                }
                else
                {
                    // Generate Embed Token for reports without effective identities.
                    generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                }
                var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), report.Id, generateTokenRequestParameters).ConfigureAwait(false);
                if (tokenResponse == null)
                {
                    model.ErrorMessage = "Failed to generate embed token.";
                }
                // Generate Embed Configuration.
                model.EmbedToken = tokenResponse;
                model.EmbedUrl = report.EmbedUrl;
                model.Id = report.Id.ToString();
                model.ContentType = "report";

                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Report_{reportId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;
        }

        private void CheckBookmarks(Report report, int portalId)
        {
            var bookmarks = BookmarksRepository.Instance.GetBookmarks(report.Id.ToString(), portalId);
            foreach (var bookmark in bookmarks)
            {
                if (report.ModifiedDateTime != null && bookmark.CreatedOn < report.ModifiedDateTime)
                {
                    BookmarksRepository.Instance.DeleteBookmark(bookmark.Id);
                }
                else if(bookmark.CreatedOn < DateTime.Now.AddDays(-30))
                {
                    BookmarksRepository.Instance.DeleteBookmark(bookmark.Id);
                }
            }
        }

        public async Task<EmbedConfig> GetDashboardEmbedConfigAsync(int userId, string username, string roles, string dashboardId)
        {
            var model = (EmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}");
            if (model != null)
                return model;

            model = new EmbedConfig();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(Settings.ApiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var dashboards = await client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false);

                // No dashboards retrieved for the given workspace.
                if (dashboards.Value.Count() == 0)
                {
                    model.ErrorMessage = "No dashboards were found in the workspace";
                }
                var dashboard = dashboards.Value.FirstOrDefault(r => r.Id.ToString().Equals(dashboardId, StringComparison.InvariantCultureIgnoreCase));
                if (dashboard == null)
                {
                    model.ErrorMessage = "No dashboard with the given ID was found in the workspace. Make sure ReportId is valid.";
                }
                // Generate Embed Token for reports without effective identities.
                GenerateTokenRequest generateTokenRequestParameters;
                // This is how you create embed token with effective identities
                if (!string.IsNullOrWhiteSpace(username))
                {
                    var rls = new EffectiveIdentity(username, new List<string> { dashboardId });
                    if (!string.IsNullOrWhiteSpace(roles))
                    {
                        var rolesList = new List<string>();
                        rolesList.AddRange(roles.Split(','));
                        rls.Roles = rolesList;
                    }
                    // Generate Embed Token with effective identities.
                    generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view", identities: new List<EffectiveIdentity> { rls });
                }
                else
                {
                    // Generate Embed Token for reports without effective identities.
                    generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                }
                EmbedToken tokenResponse;
                try
                {
                    tokenResponse = await client.Dashboards.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, generateTokenRequestParameters).ConfigureAwait(false);
                }
                catch (HttpOperationException ex)
                {
                    if (ex.Response.Content.Contains("shouldn't have effective identity"))
                    {
                        // HACK: Creating embed token for accessing dataset shouldn't have effective identity"
                        // See https://community.powerbi.com/t5/Developer/quot-shouldn-t-have-effective-identity-quot-error-when-passing/m-p/437177
                        generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                        tokenResponse = await client.Dashboards.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, generateTokenRequestParameters).ConfigureAwait(false);
                    }
                    else
                        throw;
                }
                if (tokenResponse == null)
                {
                    model.ErrorMessage = "Failed to generate embed token.";
                }
                // Generate Embed Configuration.
                model.EmbedToken = tokenResponse;
                model.EmbedUrl = dashboard.EmbedUrl;
                model.Id = dashboard.Id.ToString();
                model.ContentType = "dashboard";

                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;

        }

        public async Task<TileEmbedConfig> GetTileEmbedConfigAsync(int userId, string tileId, string dashboardId)
        {
            var model = (TileEmbedConfig)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}_Tile_{tileId}");
            if (model != null)
                return model;

            model = new TileEmbedConfig();
            // Get token credentials for user
            var getCredentialsResult = await GetTokenCredentials();
            if (!getCredentialsResult)
            {
                // The error message set in GetTokenCredentials
                model.ErrorMessage = "Authentication failed.";
                return model;
            }
            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri(Settings.ApiUrl), tokenCredentials))
            {
                // Get a list of dashboards.
                var dashboards = await client.Dashboards.GetDashboardsInGroupAsync(Guid.Parse(Settings.WorkspaceId)).ConfigureAwait(false);

                // Get the first report in the workspace.
                var dashboard = dashboards.Value.FirstOrDefault(r => r.Id.ToString().Equals(dashboardId, StringComparison.InvariantCultureIgnoreCase));
                if (dashboard == null)
                {
                    tileEmbedConfig.ErrorMessage = "Workspace has no dashboards.";
                    return model;
                }
                var tiles = await client.Dashboards.GetTilesInGroupAsync(Guid.Parse(Settings.WorkspaceId), Guid.Parse(dashboardId)).ConfigureAwait(false);
                // Get the first tile in the workspace.
                var tile = tiles.Value.FirstOrDefault(x => x.Id.ToString() == tileId);
                // Generate Embed Token for a tile.
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = await client.Tiles.GenerateTokenInGroupAsync(Guid.Parse(Settings.WorkspaceId), dashboard.Id, tile.Id, generateTokenRequestParameters).ConfigureAwait(false);
                if (tokenResponse == null)
                {
                    tileEmbedConfig.ErrorMessage = "Failed to generate embed token.";
                    return model;
                }

                // Generate Embed Configuration.
                tileEmbedConfig = new TileEmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = tile.EmbedUrl,
                    Id = tile.Id.ToString(),
                    dashboardId = dashboard.Id.ToString(),
                    ContentType = "tile"
                };
                CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_{userId}_{Thread.CurrentThread.CurrentUICulture.Name}_Dashboard_{dashboardId}_Tile_{tileId}", model, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);
            }
            return model;
        }

        /// <summary>
        /// Check if web.config embed parameters have valid values.
        /// </summary>
        /// <returns>Null if web.config parameters are valid, otherwise returns specific error string.</returns>
        private string ValidateSettings()
        {
            // Application Id must have a value.
            if (string.IsNullOrWhiteSpace(Settings.ApplicationId))
            {
                return "ApplicationId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Application Id must be a Guid object.
            Guid result;
            if (!Guid.TryParse(Settings.ApplicationId, out result))
            {
                return "ApplicationId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill application Id in web.config.";
            }

            // Workspace Id must have a value.
            if (string.IsNullOrWhiteSpace(Settings.WorkspaceId))
            {
                return "WorkspaceId is empty. Please select a group you own and fill its Id in web.config";
            }

            // Workspace Id must be a Guid object.
            if (!Guid.TryParse(Settings.WorkspaceId, out result))
            {
                return "WorkspaceId must be a Guid object. Please select a workspace you own and fill its Id in web.config";
            }

            if (Settings.AuthenticationType.Equals("MasterUser"))
            {
                // Username must have a value.
                if (string.IsNullOrWhiteSpace(Settings.Username))
                {
                    return "Username is empty. Please fill Power BI username in web.config";
                }

                // Password must have a value.
                if (string.IsNullOrWhiteSpace(Settings.Password))
                {
                    return "Password is empty. Please fill password of Power BI username in web.config";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalApplicationId))
                {
                    return "Service Principal ApplicationId is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalApplicationSecret))
                {
                    return "ApplicationSecret is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                // Must fill tenant Id
                if (string.IsNullOrWhiteSpace(Settings.ServicePrincipalTenant))
                {
                    return "Invalid Tenant. Please fill Tenant ID in Tenant under web.config";
                }
            }
            return null;
        }

        private async Task<AuthenticationResult> DoAuthenticationAsync()
        {
            AuthenticationResult authenticationResult = null;
            if (Settings.AuthenticationType.Equals("MasterUser"))
            {
                var authenticationContext = new AuthenticationContext(Settings.AuthorityUrl);

                // Authentication using master user credentials
                var credential = new UserPasswordCredential(Settings.Username, Settings.Password);
                authenticationResult = authenticationContext.AcquireTokenAsync(Settings.ResourceUrl, Settings.ApplicationId, credential).Result;
            }
            else
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificURL = Settings.AuthorityUrl.Replace("common", Settings.ServicePrincipalTenant);
                var authenticationContext = new AuthenticationContext(tenantSpecificURL);

                // Authentication using app credentials
                var credential = new ClientCredential(Settings.ServicePrincipalApplicationId, Settings.ServicePrincipalApplicationSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(Settings.ResourceUrl, credential).Result;
            }

            return authenticationResult;
        }

        private async Task<bool> GetTokenCredentials()
        {
            // var result = new EmbedConfig { Username = username, Roles = roles };
            var error = ValidateSettings();
            if (error != null)
            {
                embedConfig.ErrorMessage = error;
                return false;
            }

            tokenCredentials = (TokenCredentials)CachingProvider.Instance().GetItem($"PBI_{Settings.PortalId}_{Settings.SettingsId}_TokenCredentials");
            if (tokenCredentials != null)
                return true;

            // Authenticate using created credentials
            AuthenticationResult authenticationResult = null;
            try
            {
                authenticationResult = await DoAuthenticationAsync().ConfigureAwait(false);
            }
            catch (AggregateException exc)
            {
                embedConfig.ErrorMessage = exc.InnerException.Message;
                Logger.Error(embedConfig.ErrorMessage);
                return false;
            }

            if (authenticationResult == null)
            {
                embedConfig.ErrorMessage = "Authentication Failed.";
                return false;
            }

            tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
            CachingProvider.Instance().Insert($"PBI_{Settings.PortalId}_{Settings.SettingsId}_TokenCredentials", tokenCredentials, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
            return true;
        }
    }
}