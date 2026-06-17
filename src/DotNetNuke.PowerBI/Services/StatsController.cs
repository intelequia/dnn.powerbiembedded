using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.StatsView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class StatsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(StatsController));

        private const string ReportTag = "Report";
        private const string DefaultColor = "#6c757d";

        private const string LocalResourceFile = "~/DesktopModules/MVC/PowerBIEmbedded/App_LocalResources/StatsView.resx";

        private static readonly string[] Palette =
        {
            "#4263eb", "#2fb344", "#4299e1", "#f76707", "#ae3ec9", "#d63939",
            "#0ca678", "#d6336c", "#f59f00", "#1098ad", "#7048e8", "#74b816"
        };

        [HttpGet]
        [DnnAuthorize]
        public async Task<HttpResponseMessage> Get(string range = "14d")
        {
            try
            {
                var normalizedRange = NormalizeRange(range);
                var userId = UserInfo?.UserID ?? -1;
                var lastRefreshSetting = GetModuleSetting("PowerBIEmbedded_Stats_LastRefreshUrl");
                var cacheKey = $"PBI_{PortalSettings.PortalId}_{ActiveModule.ModuleID}_{userId}_StatsView_{normalizedRange}_{(lastRefreshSetting ?? string.Empty).GetHashCode()}";
                var cached = CachingProvider.Instance().GetItem(cacheKey) as StatsViewModel;
                if (cached != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, cached);
                }

                var reportPages = GetVisibleReportPages();
                var nowUtc = DateTime.UtcNow;
                var monthStartUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var model = new StatsViewModel
                {
                    Range = normalizedRange,
                    PublishedReports = reportPages.Count,
                    PublishedReportsThisMonth = reportPages.Count(r => r.CreatedOnUtc >= monthStartUtc),
                    ActiveUsers24h = CountActiveUsers24h(PortalSettings.PortalId),
                    ActiveUsersYesterday = CountActiveUsersYesterday(PortalSettings.PortalId),
                    LastRefreshHealth = "green",
                    LastRefreshNote = "",
                    LastRefreshClickUrl = ResolveAllowedTabUrl(lastRefreshSetting)
                };

                await FillPowerBiDatasetStatsAsync(model).ConfigureAwait(false);
                await FillApplicationInsightsStatsAsync(model, reportPages, normalizedRange).ConfigureAwait(false);

                CachingProvider.Instance().Insert(cacheKey, model, null, DateTime.Now.AddDays(1), TimeSpan.Zero);
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, new StatsViewModel
                {
                    ErrorMessage = Localize("Error"),
                    LastRefreshHealth = "red",
                    LastRefreshNote = Localize("LastRefreshError")
                });
            }
        }

        private async Task FillPowerBiDatasetStatsAsync(StatsViewModel model)
        {
            var settings = SharedSettingsRepository.Instance.GetSettings(PortalSettings.PortalId).RemoveUnauthorizedItems(UserInfo);
            if (settings == null || settings.Count == 0)
            {
                model.LastRefreshHealth = "red";
                model.LastRefreshNote = Localize("NoWorkspaceConfigured");
                return;
            }

            var tasks = settings
                .Where(s => !string.IsNullOrWhiteSpace(s.WorkspaceId))
                .Select(GetWorkspaceDatasetSnapshotAsync)
                .ToList();

            var snapshots = await Task.WhenAll(tasks).ConfigureAwait(false);

            var nowUtc = DateTime.UtcNow;
            var successfulSnapshots = snapshots.Where(s => s != null && !s.HasFatalError).ToList();

            model.DatasetsTotal = successfulSnapshots.Sum(s => s.DatasetCount);
            model.DatasetsAutoRefreshed24h = successfulSnapshots.Sum(s => s.AutoRefreshedLast24h);

            var allRefreshes = successfulSnapshots
                .Where(s => s.LastRefreshUtc.HasValue)
                .Select(s => s.LastRefreshUtc.Value)
                .ToList();

            if (allRefreshes.Count > 0)
            {
                model.LastRefreshUtc = allRefreshes.Max();
            }

            var healthy = successfulSnapshots.Count(s => s.Health == "green");
            var warning = successfulSnapshots.Count(s => s.Health == "yellow");
            var error = snapshots.Length - healthy - warning;

            if (snapshots.Length == 0 || error == snapshots.Length)
            {
                model.LastRefreshHealth = "red";
                model.LastRefreshNote = Localize("LastRefreshError");
                model.IsPartialData = true;
                return;
            }

            if (error > 0 || warning > 0)
            {
                model.LastRefreshHealth = "yellow";
                model.LastRefreshNote = Localize("LastRefreshWarning");
                model.IsPartialData = error > 0;
                return;
            }

            model.LastRefreshHealth = "green";
            model.LastRefreshNote = Localize("LastRefreshOk");

            if (!model.LastRefreshUtc.HasValue)
            {
                model.LastRefreshHealth = "yellow";
                model.LastRefreshNote = Localize("LastRefreshNoData");
            }
        }

        private async Task<WorkspaceDatasetSnapshot> GetWorkspaceDatasetSnapshotAsync(PowerBISettings setting)
        {
            var result = new WorkspaceDatasetSnapshot();
            try
            {
                var token = await AcquireAccessTokenAsync(setting).ConfigureAwait(false);
                if (string.IsNullOrEmpty(token))
                {
                    result.HasFatalError = true;
                    result.Health = "red";
                    return result;
                }

                var apiUrl = string.IsNullOrWhiteSpace(setting.ApiUrl) ? "https://api.powerbi.com" : setting.ApiUrl;
                var client = new PowerBIClient(token, new Uri(apiUrl));
                {
                    var workspaceId = Guid.Parse(setting.WorkspaceId);
                    var datasets = (await client.Datasets.GetDatasetsInGroupAsync(workspaceId).ConfigureAwait(false)).Value.Value
                        .Where(d => d != null && (d.Name ?? string.Empty).IndexOf("Report Usage Metrics Model", StringComparison.OrdinalIgnoreCase) < 0)
                        .ToList();

                    result.DatasetCount = datasets.Count;

                    foreach (var dataset in datasets)
                    {
                        try
                        {
                            var history = (await client.Datasets.GetRefreshHistoryInGroupAsync(workspaceId, dataset.Id, 1).ConfigureAwait(false)).Value.Value;
                            var last = history.FirstOrDefault();
                            if (last == null)
                            {
                                continue;
                            }

                            var reference = last.EndTime ?? last.StartTime;
                            if (reference.HasValue)
                            {
                                var referenceUtc = reference.Value.UtcDateTime;
                                if (!result.LastRefreshUtc.HasValue || referenceUtc > result.LastRefreshUtc.Value)
                                {
                                    result.LastRefreshUtc = referenceUtc;
                                }

                                if (referenceUtc >= DateTime.UtcNow.AddHours(-24)
                                    && IsAutoRefresh(last.RefreshType))
                                {
                                    result.AutoRefreshedLast24h++;
                                }
                            }

                            var status = (last.Status ?? string.Empty).Trim();
                            var hasServiceException = !string.IsNullOrWhiteSpace(last.ServiceExceptionJson);
                            if (status.Equals("Failed", StringComparison.OrdinalIgnoreCase) || hasServiceException)
                            {
                                result.Health = "yellow";
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn($"Error getting refresh history for dataset {dataset.Id} in workspace {setting.WorkspaceId}", ex);
                            result.Health = result.Health == "red" ? "red" : "yellow";
                        }
                    }
                }

                if (result.DatasetCount > 0 && !result.LastRefreshUtc.HasValue && result.Health == "green")
                {
                    result.Health = "yellow";
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error loading dataset stats for workspace {setting.WorkspaceId}", ex);
                result.HasFatalError = true;
                result.Health = "red";
                return result;
            }
        }

        private async Task FillApplicationInsightsStatsAsync(StatsViewModel model, List<ReportTabInfo> reportPages, string range)
        {
            var appId = GetModuleSetting("PowerBIEmbedded_Stats_AppInsightsAppId");
            var apiKey = GetModuleSetting("PowerBIEmbedded_Stats_AppInsightsApiKey");
            var endpoint = GetModuleSetting("PowerBIEmbedded_Stats_AppInsightsApiUrl");

            model.HasApplicationInsightsConfig = !string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(apiKey);
            if (!model.HasApplicationInsightsConfig)
            {
                return;
            }

            var resolvedEndpoint = string.IsNullOrWhiteSpace(endpoint)
                ? $"https://api.applicationinsights.io/v1/apps/{appId}/query"
                : endpoint;
            resolvedEndpoint = resolvedEndpoint.Replace("{appId}", appId);

            var bucket = range == "24h" ? "1h" : "1d";
            var aiSpan = range == "24h" ? "24h" : range;

            int mostViewedTopCount;
            if (!int.TryParse(GetModuleSetting("PowerBIEmbedded_Stats_MostViewedTopCount"), out mostViewedTopCount) || mostViewedTopCount <= 0)
            {
                mostViewedTopCount = 5;
            }

            var trendQuery =
                "pageViews " +
                "| where client_Type == 'Browser' " +
                $"| where timestamp >= ago({aiSpan}) " +
                "| summarize viewCount=sum(itemCount) by bucket=bin(timestamp, " + bucket + "), url=tostring(url), name=tostring(name)";

            var mostViewedQuery =
                "pageViews " +
                "| where client_Type == 'Browser' " +
                $"| where timestamp >= ago({aiSpan}) " +
                "| summarize viewCount=sum(itemCount) by url=tostring(url), name=tostring(name) " +
                "| order by viewCount desc | take 500";

            var activeUsers24hQuery =
                "pageViews " +
                "| where client_Type == 'Browser' " +
                "| where timestamp >= ago(24h) " +
                "| where isnotempty(user_Id) " +
                "| summarize userCount=dcount(user_Id)";

            var activeUsersYesterdayQuery =
                "pageViews " +
                "| where client_Type == 'Browser' " +
                "| where timestamp between(ago(48h) .. ago(24h)) " +
                "| where isnotempty(user_Id) " +
                "| summarize userCount=dcount(user_Id)";

            try
            {
                var trendTask = QueryAppInsightsAsync(resolvedEndpoint, apiKey, trendQuery);
                var mostTask = QueryAppInsightsAsync(resolvedEndpoint, apiKey, mostViewedQuery);
                var activeUsers24hTask = QueryAppInsightsAsync(resolvedEndpoint, apiKey, activeUsers24hQuery);
                var activeUsersYesterdayTask = QueryAppInsightsAsync(resolvedEndpoint, apiKey, activeUsersYesterdayQuery);
                await Task.WhenAll(trendTask, mostTask, activeUsers24hTask, activeUsersYesterdayTask).ConfigureAwait(false);

                var trendRows = trendTask.Result;
                var mostRows = mostTask.Result;
                var activeUsers24hRows = activeUsers24hTask.Result;
                var activeUsersYesterdayRows = activeUsersYesterdayTask.Result;

                // Override active users with App Insights data only when it reports a positive count.
                // A zero (e.g. telemetry without user_Id) must not clobber the DNN-based fallback count.
                var aiActiveUsers24h = activeUsers24hRows.Count > 0 ? ReadInt(activeUsers24hRows[0], 0) : -1;
                var aiActiveUsersYesterday = activeUsersYesterdayRows.Count > 0 ? ReadInt(activeUsersYesterdayRows[0], 0) : -1;
                if (aiActiveUsers24h > 0) { model.ActiveUsers24h = aiActiveUsers24h; }
                if (aiActiveUsersYesterday > 0) { model.ActiveUsersYesterday = aiActiveUsersYesterday; }

                var reportByTabId = reportPages.ToDictionary(r => r.TabId, r => r);

                var trendBuckets = new Dictionary<DateTime, int>();
                foreach (var row in trendRows)
                {
                    var bucketUtc = ReadDate(row, 0);
                    var url = ReadString(row, 1);
                    var name = ReadString(row, 2);
                    var views = ReadInt(row, 3);

                    var match = ResolveReportMatch(reportPages, url, name);
                    if (match == null)
                    {
                        continue;
                    }

                    if (!trendBuckets.ContainsKey(bucketUtc))
                    {
                        trendBuckets[bucketUtc] = 0;
                    }
                    trendBuckets[bucketUtc] += views;
                }

                foreach (var bucketPoint in trendBuckets.OrderBy(x => x.Key))
                {
                    model.Trend.Add(new StatsTrendPoint
                    {
                        BucketUtcIso = bucketPoint.Key.ToString("o"),
                        Label = range == "24h" ? bucketPoint.Key.ToString("HH:mm") : bucketPoint.Key.ToString("MM-dd"),
                        Views = bucketPoint.Value
                    });
                }

                var mostByTab = new Dictionary<int, int>();
                foreach (var row in mostRows)
                {
                    var url = ReadString(row, 0);
                    var name = ReadString(row, 1);
                    var views = ReadInt(row, 2);

                    var match = ResolveReportMatch(reportPages, url, name);
                    if (match == null)
                    {
                        continue;
                    }

                    if (!mostByTab.ContainsKey(match.TabId))
                    {
                        mostByTab[match.TabId] = 0;
                    }
                    mostByTab[match.TabId] += views;
                }

                model.MostViewed = mostByTab
                    .OrderByDescending(x => x.Value)
                    .Take(mostViewedTopCount)
                    .Where(x => reportByTabId.ContainsKey(x.Key))
                    .Select(x =>
                    {
                        var report = reportByTabId[x.Key];
                        return new StatsMostViewedItem
                        {
                            TabId = report.TabId,
                            Title = report.Title,
                            Url = report.Url,
                            Category = report.PrimaryCategory,
                            Color = report.Color,
                            Views = x.Value,
                            Tags = report.Categories ?? new List<string>()
                        };
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Warn("Error querying Application Insights for StatsView", ex);
                model.IsPartialData = true;
                model.ErrorMessage = $"Error querying Application Insights: {ex.Message}";
            }
        }

        private async Task<List<JArray>> QueryAppInsightsAsync(string endpoint, string apiKey, string query)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("x-api-key", apiKey);
                
                var requestObject = new { query = query };
                var requestBody = JsonConvert.SerializeObject(requestObject);
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                
                Logger.Trace($"QueryAppInsightsAsync - Endpoint: {endpoint}");
                Logger.Trace($"QueryAppInsightsAsync - Request body: {requestBody}");
                
                var response = await http.PostAsync(endpoint, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Logger.Trace($"QueryAppInsightsAsync - Response: {response.StatusCode} - {responseBody}");
                    throw new ApplicationException($"Application Insights query failed: {response.StatusCode} - {responseBody}");
                }

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var root = JObject.Parse(json);

                // Check for KQL error in response
                var error = root["error"];
                if (error != null)
                {
                    var errorMessage = error["message"]?.ToString() ?? "Unknown KQL error";
                    throw new ApplicationException($"KQL query error: {errorMessage}");
                }

                var table = root["tables"]?.FirstOrDefault();
                if (table == null)
                {
                    return new List<JArray>();
                }

                var rows = table["rows"] as JArray;
                if (rows == null)
                {
                    return new List<JArray>();
                }

                return rows.OfType<JArray>().ToList();
            }
        }

        private string EscapeJson(string input)
        {
            return (input ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private async Task<string> AcquireAccessTokenAsync(PowerBISettings setting)
        {
            try
            {
                var cacheKey = $"PBI_{PortalSettings.PortalId}_{setting.SettingsId}_TokenCredentials";
                var cached = CachingProvider.Instance().GetItem(cacheKey) as string;
                if (!string.IsNullOrEmpty(cached))
                {
                    return cached;
                }

                AuthenticationResult authenticationResult;
                if (string.Equals(setting.AuthenticationType, "MasterUser", StringComparison.OrdinalIgnoreCase))
                {
                    var authContext = new AuthenticationContext(setting.AuthorityUrl);
                    var credential = new UserPasswordCredential(setting.Username, setting.Password);
                    authenticationResult = await authContext.AcquireTokenAsync(setting.ResourceUrl, setting.ApplicationId, credential).ConfigureAwait(false);
                }
                else
                {
                    var tenantSpecificURL = setting.AuthorityUrl.Replace("common", setting.ServicePrincipalTenant);
                    var authContext = new AuthenticationContext(tenantSpecificURL);
                    var credential = new ClientCredential(setting.ServicePrincipalApplicationId, setting.ServicePrincipalApplicationSecret);
                    authenticationResult = await authContext.AcquireTokenAsync(setting.ResourceUrl, credential).ConfigureAwait(false);
                }

                if (authenticationResult == null || string.IsNullOrWhiteSpace(authenticationResult.AccessToken))
                {
                    return null;
                }

                CachingProvider.Instance().Insert(cacheKey, authenticationResult.AccessToken, null, authenticationResult.ExpiresOn.AddMinutes(-2).UtcDateTime, TimeSpan.Zero);
                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error acquiring Power BI access token for workspace {setting.WorkspaceId}", ex);
                return null;
            }
        }

        private static bool IsAutoRefresh(RefreshType? refreshType)
        {
            if (!refreshType.HasValue)
            {
                return false;
            }

            var value = refreshType.Value.ToString();
            return value.Equals("Scheduled", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Automatic", StringComparison.OrdinalIgnoreCase);
        }

        private List<ReportTabInfo> GetVisibleReportPages()
        {
            var tabs = TabController.Instance.GetTabsByPortal(PortalSettings.PortalId).AsList();
            var rows = new List<ReportTabInfo>();
            var allCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var tab in tabs)
            {
                if (tab == null || tab.IsDeleted || tab.DisableLink || tab.IsSuperTab)
                {
                    continue;
                }

                var tags = GetTabTags(tab);
                if (!tags.Any(t => string.Equals(t, ReportTag, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (!TabPermissionController.CanViewPage(tab))
                {
                    continue;
                }

                var categories = tags
                    .Where(t => !string.Equals(t, ReportTag, StringComparison.OrdinalIgnoreCase))
                    .Select(t => (t ?? string.Empty).Trim())
                    .Where(t => t.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var category in categories)
                {
                    allCategories.Add(category);
                }

                rows.Add(new ReportTabInfo
                {
                    TabId = tab.TabID,
                    Title = string.IsNullOrWhiteSpace(tab.Title) ? tab.TabName : tab.Title,
                    Url = Globals.NavigateURL(tab.TabID),
                    CreatedOnUtc = tab.CreatedOnDate.Kind == DateTimeKind.Utc ? tab.CreatedOnDate : tab.CreatedOnDate.ToUniversalTime(),
                    PrimaryCategory = categories.FirstOrDefault(),
                    Categories = categories
                });
            }

            var sortedCategories = allCategories.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToList();
            var colorByCategory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < sortedCategories.Count; i++)
            {
                colorByCategory[sortedCategories[i]] = Palette[i % Palette.Length];
            }

            foreach (var row in rows)
            {
                row.Color = row.PrimaryCategory != null && colorByCategory.ContainsKey(row.PrimaryCategory)
                    ? colorByCategory[row.PrimaryCategory]
                    : DefaultColor;
                row.NormalizedUrl = NormalizeUrl(row.Url);
            }

            return rows;
        }

        private static List<string> GetTabTags(TabInfo tab)
        {
            var tags = new List<string>();
            try
            {
                if (tab.Terms != null && tab.Terms.Count > 0)
                {
                    tags.AddRange(tab.Terms.Select(t => t.Name));
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not read terms for tab {tab.TabID}", ex);
            }

            return tags;
        }

        private ReportTabInfo ResolveReportMatch(List<ReportTabInfo> reports, string url, string title)
        {
            if (reports == null || reports.Count == 0)
            {
                return null;
            }

            var normalizedUrl = NormalizeUrl(url);
            if (!string.IsNullOrWhiteSpace(normalizedUrl))
            {
                var byUrl = reports.FirstOrDefault(r => string.Equals(r.NormalizedUrl, normalizedUrl, StringComparison.OrdinalIgnoreCase));
                if (byUrl != null)
                {
                    return byUrl;
                }

                var parsedTabId = TryParseTabId(url);
                if (parsedTabId.HasValue)
                {
                    var byTabId = reports.FirstOrDefault(r => r.TabId == parsedTabId.Value);
                    if (byTabId != null)
                    {
                        return byTabId;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                return reports.FirstOrDefault(r => string.Equals(r.Title, title, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        private static int? TryParseTabId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            try
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var tabId = query["tabid"];
                    int parsed;
                    if (int.TryParse(tabId, out parsed))
                    {
                        return parsed;
                    }
                }
                else
                {
                    var query = System.Web.HttpUtility.ParseQueryString(new Uri("http://dummy" + (url.StartsWith("/") ? url : "/" + url)).Query);
                    var tabId = query["tabid"];
                    int parsed;
                    if (int.TryParse(tabId, out parsed))
                    {
                        return parsed;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private static string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            try
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    return (uri.PathAndQuery ?? string.Empty).TrimEnd('/').ToLowerInvariant();
                }

                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }

                var fake = new Uri("http://local" + url);
                return (fake.PathAndQuery ?? string.Empty).TrimEnd('/').ToLowerInvariant();
            }
            catch
            {
                return (url ?? string.Empty).Trim().TrimEnd('/').ToLowerInvariant();
            }
        }

        private static int CountActiveUsers24h(int portalId)
        {
            var nowUtc = DateTime.UtcNow;
            var fromUtc = nowUtc.AddHours(-24);
            return CountActiveUsersInWindow(portalId, fromUtc, nowUtc);
        }

        private static int CountActiveUsersYesterday(int portalId)
        {
            var nowUtc = DateTime.UtcNow;
            var fromUtc = nowUtc.AddHours(-48);
            var toUtc = nowUtc.AddHours(-24);
            return CountActiveUsersInWindow(portalId, fromUtc, toUtc);
        }

        private static int CountActiveUsersInWindow(int portalId, DateTime fromUtc, DateTime toUtc)
        {
            return UserController.GetUsers(portalId)
                .Cast<DotNetNuke.Entities.Users.UserInfo>()
                .Count(u => u != null
                            && !u.IsDeleted
                            && u.Membership != null
                            && u.Membership.Approved
                            && u.Membership.LastLoginDate.ToUniversalTime() >= fromUtc
                            && u.Membership.LastLoginDate.ToUniversalTime() < toUtc);
        }

        private string ResolveAllowedTabUrl(string urlOrTabId)
        {
            if (string.IsNullOrWhiteSpace(urlOrTabId))
            {
                return null;
            }

            // Try as direct integer TabId
            int directTabId;
            if (int.TryParse(urlOrTabId.Trim(), out directTabId))
            {
                var tab = TabController.Instance.GetTab(directTabId, PortalSettings.PortalId);
                if (tab != null && !tab.IsDeleted && TabPermissionController.CanViewPage(tab))
                {
#pragma warning disable CS0618
                    return Globals.NavigateURL(tab.TabID);
#pragma warning restore CS0618
                }
                return null;
            }

            // Try to parse tabid from URL querystring
            var parsedTabId = TryParseTabId(urlOrTabId);
            if (parsedTabId.HasValue)
            {
                var tab = TabController.Instance.GetTab(parsedTabId.Value, PortalSettings.PortalId);
                if (tab != null && !tab.IsDeleted && TabPermissionController.CanViewPage(tab))
                {
#pragma warning disable CS0618
                    return Globals.NavigateURL(tab.TabID);
#pragma warning restore CS0618
                }
                return null;
            }

            // Try to match by normalized URL path across portal tabs
            var normalizedInput = NormalizeUrl(urlOrTabId);
            if (!string.IsNullOrWhiteSpace(normalizedInput))
            {
                var allTabs = TabController.Instance.GetTabsByPortal(PortalSettings.PortalId).AsList();
                foreach (var t in allTabs)
                {
                    if (t.IsDeleted) { continue; }
#pragma warning disable CS0618
                    var tabUrl = NormalizeUrl(Globals.NavigateURL(t.TabID));
#pragma warning restore CS0618
                    if (string.Equals(tabUrl, normalizedInput, StringComparison.OrdinalIgnoreCase) && TabPermissionController.CanViewPage(t))
                    {
#pragma warning disable CS0618
                        return Globals.NavigateURL(t.TabID);
#pragma warning restore CS0618
                    }
                }
            }

            return null;
        }

        private string GetModuleSetting(string key)
        {
            if (ActiveModule == null)
            {
                return string.Empty;
            }

            Hashtable tabModuleSettings = ActiveModule.TabModuleSettings;
            if (tabModuleSettings != null && tabModuleSettings.ContainsKey(key))
            {
                return tabModuleSettings[key].ToString();
            }

            return string.Empty;
        }

        private string Localize(string key)
        {
            var value = Localization.GetString(key, LocalResourceFile);
            return string.IsNullOrEmpty(value) ? key : value;
        }

        private static DateTime ReadDate(JArray row, int columnIndex)
        {
            if (row == null || columnIndex >= row.Count)
            {
                return DateTime.MinValue;
            }

            var val = row[columnIndex]?.Value<string>();
            if (DateTime.TryParse(val, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var result))
            {
                return result;
            }

            return DateTime.MinValue;
        }

        private static string ReadString(JArray row, int columnIndex)
        {
            if (row == null || columnIndex >= row.Count)
            {
                return string.Empty;
            }

            return row[columnIndex]?.Value<string>() ?? string.Empty;
        }

        private static int ReadInt(JArray row, int columnIndex)
        {
            if (row == null || columnIndex >= row.Count)
            {
                return 0;
            }

            return row[columnIndex]?.Value<int>() ?? 0;
        }

        private string NormalizeRange(string range)
        {
            if (string.IsNullOrWhiteSpace(range))
            {
                return "14d";
            }

            range = range.ToLowerInvariant().Trim();
            if (!new[] { "24h", "7d", "14d", "30d" }.Contains(range))
            {
                return "14d";
            }

            return range;
        }

        private class WorkspaceDatasetSnapshot
        {
            public int DatasetCount { get; set; }
            public int AutoRefreshedLast24h { get; set; }
            public DateTime? LastRefreshUtc { get; set; }
            public string Health { get; set; } = "green";
            public bool HasFatalError { get; set; }
        }

        private class ReportTabInfo
        {
            public int TabId { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public string NormalizedUrl { get; set; }
            public string PrimaryCategory { get; set; }
            public string Color { get; set; }
            public List<string> Categories { get; set; }
        }
    }
}
