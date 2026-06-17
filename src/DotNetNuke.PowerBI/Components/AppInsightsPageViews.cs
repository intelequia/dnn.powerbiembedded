using DotNetNuke.Instrumentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetNuke.PowerBI.Components
{
    /// <summary>
    /// Queries Application Insights for page-view counts of report pages, reusing the same
    /// KQL "Most Viewed" query used by the Stats module (<c>StatsController</c>). The returned
    /// rows are matched back to DNN tabs by normalized URL, then by the tabid querystring and
    /// finally by page title, so the catalog can show real analytics per page.
    /// </summary>
    public static class AppInsightsPageViews
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(AppInsightsPageViews));

        /// <summary>Minimal description of a report page used to match Application Insights rows.</summary>
        public class ReportPageRef
        {
            public int TabId { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string NormalizedUrl { get; set; }
        }

        /// <summary>
        /// Returns a dictionary mapping TabId to total page views in the given range. Returns an
        /// empty dictionary when Application Insights is not configured or the query fails (the
        /// caller can then fall back to zero views).
        /// </summary>
        /// <param name="appId">Application Insights application id.</param>
        /// <param name="apiKey">Read API key with access to query telemetry.</param>
        /// <param name="endpoint">Query endpoint (may contain the <c>{appId}</c> token).</param>
        /// <param name="range">Time range, one of 24h/7d/14d/30d/90d. Defaults to 30d.</param>
        /// <param name="reportPages">Pages tagged with the "Report" term to resolve views for.</param>
        public static async Task<Dictionary<int, int>> GetViewsByTabAsync(
            string appId, string apiKey, string endpoint, string range, IEnumerable<ReportPageRef> reportPages)
        {
            var result = new Dictionary<int, int>();

            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(apiKey))
            {
                return result;
            }

            var pages = reportPages?.ToList() ?? new List<ReportPageRef>();
            if (pages.Count == 0)
            {
                return result;
            }

            foreach (var page in pages)
            {
                page.NormalizedUrl = NormalizeUrl(page.Url);
            }

            var resolvedEndpoint = string.IsNullOrWhiteSpace(endpoint)
                ? $"https://api.applicationinsights.io/v1/apps/{appId}/query"
                : endpoint.Replace("{appId}", appId);

            var aiSpan = NormalizeRange(range);

            // Same query the Stats module runs to compute "Most Viewed".
            var mostViewedQuery =
                "pageViews " +
                "| where client_Type == 'Browser' " +
                $"| where timestamp >= ago({aiSpan}) " +
                "| summarize viewCount=sum(itemCount) by url=tostring(url), name=tostring(name) " +
                "| order by viewCount desc | take 500";

            try
            {
                var rows = await QueryAsync(resolvedEndpoint, apiKey, mostViewedQuery).ConfigureAwait(false);

                foreach (var row in rows)
                {
                    var url = ReadString(row, 0);
                    var name = ReadString(row, 1);
                    var views = ReadInt(row, 2);

                    var match = ResolveReportMatch(pages, url, name);
                    if (match == null)
                    {
                        continue;
                    }

                    if (!result.ContainsKey(match.TabId))
                    {
                        result[match.TabId] = 0;
                    }
                    result[match.TabId] += views;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Error querying Application Insights for Report Catalog page views", ex);
            }

            return result;
        }

        private static async Task<List<JArray>> QueryAsync(string endpoint, string apiKey, string query)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("x-api-key", apiKey);

                var requestBody = JsonConvert.SerializeObject(new { query });
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

                var response = await http.PostAsync(endpoint, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApplicationException($"Application Insights query failed: {response.StatusCode} - {responseBody}");
                }

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var root = JObject.Parse(json);

                var error = root["error"];
                if (error != null)
                {
                    var errorMessage = error["message"]?.ToString() ?? "Unknown KQL error";
                    throw new ApplicationException($"KQL query error: {errorMessage}");
                }

                var table = root["tables"]?.FirstOrDefault();
                var rows = table?["rows"] as JArray;
                if (rows == null)
                {
                    return new List<JArray>();
                }

                return rows.OfType<JArray>().ToList();
            }
        }

        private static ReportPageRef ResolveReportMatch(List<ReportPageRef> pages, string url, string title)
        {
            if (pages == null || pages.Count == 0)
            {
                return null;
            }

            var normalizedUrl = NormalizeUrl(url);
            if (!string.IsNullOrWhiteSpace(normalizedUrl))
            {
                var byUrl = pages.FirstOrDefault(r => string.Equals(r.NormalizedUrl, normalizedUrl, StringComparison.OrdinalIgnoreCase));
                if (byUrl != null)
                {
                    return byUrl;
                }

                var parsedTabId = TryParseTabId(url);
                if (parsedTabId.HasValue)
                {
                    var byTabId = pages.FirstOrDefault(r => r.TabId == parsedTabId.Value);
                    if (byTabId != null)
                    {
                        return byTabId;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                return pages.FirstOrDefault(r => string.Equals(r.Title, title, StringComparison.OrdinalIgnoreCase));
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
                var query = Uri.TryCreate(url, UriKind.Absolute, out uri)
                    ? System.Web.HttpUtility.ParseQueryString(uri.Query)
                    : System.Web.HttpUtility.ParseQueryString(new Uri("http://dummy" + (url.StartsWith("/") ? url : "/" + url)).Query);

                int parsed;
                if (int.TryParse(query["tabid"], out parsed))
                {
                    return parsed;
                }
            }
            catch
            {
                // Ignore malformed URLs; the title fallback still applies.
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

        private static string NormalizeRange(string range)
        {
            if (string.IsNullOrWhiteSpace(range))
            {
                return "30d";
            }

            range = range.ToLowerInvariant().Trim();
            return new[] { "24h", "7d", "14d", "30d", "90d" }.Contains(range) ? range : "30d";
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
    }
}
