using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Extensibility;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Azure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using UserInfo = DotNetNuke.Entities.Users.UserInfo;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Page = Microsoft.PowerBI.Api.Models.Page;
using System.Web.Security;

namespace DotNetNuke.PowerBI.Components
{
    public class Common
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(Common));

        #region Portal Info
        /// <summary>
        /// Current portal Id.
        /// </summary>
        //public static int PortalId => PortalSettings.Current != null ? PortalSettings.Current.PortalId : 0;

        /// <summary>
        /// Current portal Email.
        /// </summary>
        public static string PortalEmail => PortalSettings.Current != null ? PortalSettings.Current.Email : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static string PortalAlias => PortalSettings.Current != null ? PortalSettings.Current.PortalAlias.HTTPAlias : "";

        /// <summary>
        /// Current portal alias url with protocol
        /// </summary>
        public static string PortalAliasUrl => PortalSettings.Current != null ? CurrentPortalSettings.SSLEnabled ? $"https://{PortalSettings.Current.PortalAlias.HTTPAlias}" : $"http://{PortalSettings.Current.PortalAlias.HTTPAlias}" : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static string PortalLogo => PortalSettings.Current != null ? PortalSettings.Current.LogoFile : "";

        /// <summary>
        /// Current portal alias.
        /// </summary>
        public static PortalSettings CurrentPortalSettings => PortalSettings.Current ?? null;

        #endregion

        #region User Info

        public static UserInfo CurrentUser => UserController.Instance.GetCurrentUserInfo();

        public static bool IsSuperUser()
        {            
            // Usuario no logueado
            if (CurrentUser == null || CurrentUser.UserID == -1)
                return false;

            return CurrentUser.IsSuperUser;
        }

        #endregion

        #region Global string localization

        public static string LocalizeGlobalString(string key)
        {
            string defaultFileName = "GlobalResources.resx";
            string filePath = "~/DesktopModules/MVC/PowerBIEmbedded/App_GlobalResources/";
            var portalId = PortalSettings.Current != null ? PortalSettings.Current.PortalId : 0;
            string resourceFileName = Localization.GetResourceFileName(defaultFileName, CultureInfo.CurrentCulture.Name, string.Empty, portalId);
            string keyValue = Localization.GetString(key, filePath + resourceFileName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                return keyValue;
            }
            keyValue = Localization.GetString(key, filePath + defaultFileName);
            return keyValue;
        }

        #endregion

        #region Token Validation
        public async Task<string> GetTokenCredentials(PowerBISettings setting)
        {
            var error = ValidateSettings(setting);
            if (error != null)
            {
                throw new Exception($"Error in GetTokenCredentials: {error}");
            }
            AuthenticationResult authenticationResult = null;
            try
            {
                authenticationResult = await DoAuthenticationAsync(setting).ConfigureAwait(false);
            }
            catch (AggregateException exc)
            {
                throw new Exception(exc.InnerException.Message);
            }

            if (authenticationResult == null)
            {
                throw new Exception("Authentication Failed.");
            }

            return authenticationResult.AccessToken;
        }
        public string ValidateSettings(PowerBISettings settings)
        {
            // Application Id must have a value.
            if (string.IsNullOrWhiteSpace(settings.ApplicationId))
            {
                return "ApplicationId is empty. please register your application as Native app in https://dev.powerbi.com/apps and fill client Id in web.config.";
            }

            // Application Id must be a Guid object.
            Guid result;
            if (!Guid.TryParse(settings.ApplicationId, out result))
            {
                return "ApplicationId must be a Guid object. please register your application as Native app in https://dev.powerbi.com/apps and fill application Id in web.config.";
            }

            // Workspace Id must have a value.
            if (string.IsNullOrWhiteSpace(settings.WorkspaceId))
            {
                return "WorkspaceId is empty. Please select a group you own and fill its Id in web.config";
            }

            // Workspace Id must be a Guid object.
            if (!Guid.TryParse(settings.WorkspaceId, out result))
            {
                return "WorkspaceId must be a Guid object. Please select a workspace you own and fill its Id in web.config";
            }

            if (settings.AuthenticationType.Equals("MasterUser"))
            {
                // Username must have a value.
                if (string.IsNullOrWhiteSpace(settings.Username))
                {
                    return "Username is empty. Please fill Power BI username in web.config";
                }

                // Password must have a value.
                if (string.IsNullOrWhiteSpace(settings.Password))
                {
                    return "Password is empty. Please fill password of Power BI username in web.config";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(settings.ServicePrincipalApplicationId))
                {
                    return "Service Principal ApplicationId is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                if (string.IsNullOrWhiteSpace(settings.ServicePrincipalApplicationSecret))
                {
                    return "ApplicationSecret is empty. please register your application as Web app and fill appSecret in web.config.";
                }

                // Must fill tenant Id
                if (string.IsNullOrWhiteSpace(settings.ServicePrincipalTenant))
                {
                    return "Invalid Tenant. Please fill Tenant ID in Tenant under web.config";
                }
            }
            return null;
        }
        public async Task<AuthenticationResult> DoAuthenticationAsync(PowerBISettings setting)
        {
            AuthenticationResult authenticationResult = null;
            if (setting.AuthenticationType.Equals("MasterUser"))
            {
                var authenticationContext = new AuthenticationContext(setting.AuthorityUrl);

                // Authentication using master user credentials
                var credential = new UserPasswordCredential(setting.Username, setting.Password);
                authenticationResult = authenticationContext.AcquireTokenAsync(setting.ResourceUrl, setting.ApplicationId, credential).Result;
            }
            else
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificURL = setting.AuthorityUrl.Replace("common", setting.ServicePrincipalTenant);
                var authenticationContext = new AuthenticationContext(tenantSpecificURL);

                // Authentication using app credentials
                var credential = new ClientCredential(setting.ServicePrincipalApplicationId, setting.ServicePrincipalApplicationSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(setting.ResourceUrl, credential).Result;
            }
            await Task.CompletedTask;

            return authenticationResult;
        }

        public string GetUsernameProperty(int moduleId, UserInfo userInfo)
        {
            var moduleController = new ModuleController();
            string user = userInfo.Username;
            var userPropertySetting = (string)moduleController.GetModule(moduleId).TabModuleSettings["PowerBIEmbedded_UserProperty"];
            if (userPropertySetting?.ToLowerInvariant() == "email")
            {
                user = userInfo.Email;
            }
            else if (userPropertySetting == "PowerBiGroup")
            {
                var userProperty = userInfo.Profile.GetProperty("PowerBiGroup");
                if (userProperty?.PropertyValue != null)
                {
                    user = userProperty.PropertyValue;
                }
            }
            else if (userPropertySetting == "Custom" || userPropertySetting == "Custom User Profile Property")
            {
                var customProperties = (string)moduleController.GetModule(moduleId).TabModuleSettings["PowerBIEmbedded_CustomUserProperty"];
                var matches = Regex.Matches(customProperties, @"\[PROFILE:(?<PROPERTY>[A-z]*)]");

                foreach (Match match in matches)
                {
                    var userProperty = userInfo.Profile.GetProperty(match.Groups["PROPERTY"].Value);
                    if (userProperty?.PropertyValue != null)
                    {
                        customProperties = customProperties.Replace(match.Value, userProperty.PropertyValue);
                    }
                }

                user = customProperties;
            }
            else if (userPropertySetting == "Custom Extension Library")
            {
                var customExtensionLibrary = (string)moduleController.GetModule(moduleId).TabModuleSettings["PowerBIEmbedded_CustomExtensionLibrary"];
                if (!string.IsNullOrEmpty(customExtensionLibrary))
                {
                    try
                    {
                        var type = Type.GetType(customExtensionLibrary, true);
                        if (type.GetInterfaces().Contains(typeof(IRlsCustomExtension)))
                        {
                            IRlsCustomExtension extensionInstance = (IRlsCustomExtension)Activator.CreateInstance(type);
                            user = extensionInstance.GetRlsValue(System.Web.HttpContext.Current);
                        }
                        else
                        {
                            throw new Exception($"Library '{customExtensionLibrary}' does not implement IRlsCustomExtension");
                        }
                    }
                    catch (Exception cex)
                    {
                        throw new Exception($"Error instancing custom extension library '{customExtensionLibrary}'", cex);
                    }
                }
            }
            return user;
        }
        #endregion

        #region Export Report

        public async Task<Attachment> ExportPowerBIReport(
            Guid reportId,
            string accessToken,
            PowerBISettings setting,
            string reportPages,
            string rolesString,
            string username,
            string locale = "en-us")
        {
            try
            {
                string urlFilter = null;
                int pollingtimeOutInMinutes = 5;
                FileFormat format = FileFormat.PDF;
                Pages pageNames = await GetReportPages(reportId, accessToken, setting);
                string[] pages = reportPages.Split(',');
                IList<Page> filteredPages = pageNames.Value.ToList();
                if (reportPages != "All" && reportPages != "")
                {
                    filteredPages = filteredPages.Where(page => pages.Contains(page.Name)).ToList();
                }

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                if (!int.TryParse(ConfigurationManager.AppSettings["PowerBI.Export.NumberOfRetries"], out int c_maxNumberOfRetries))
                    c_maxNumberOfRetries = 2;


                Export export = null;
                int retryAttempt = 1;
                do
                {
                    var exportId = await PostExportRequest(reportId, accessToken, setting, format, rolesString, username, filteredPages, urlFilter, locale);
                    var pollResponse = await PollExportRequest(reportId, exportId, pollingtimeOutInMinutes, cancellationToken, accessToken, setting);
                    export = pollResponse?.Value;
                    if (export == null)
                    {
                        throw new ApplicationException("There was a failure exporting the report");
                    }
                    if (export.Status == ExportState.Failed)
                    {
                        // Some failure cases indicate that the system is currently busy. The entire export operation can be retried after a certain delay
                        // In such cases the recommended waiting time before retrying the entire export operation can be found in the Retry-After header
                        var retryAfterInSec = GetRetryAfterSeconds(pollResponse?.GetRawResponse())
                            ?? throw new ApplicationException("Export Error: Failed state with no RetryAfter header indicates that the export failed permanently.");
                        await Task.Delay(retryAfterInSec * 1000);
                    }
                }
                while (export.Status != ExportState.Succeeded && retryAttempt++ < c_maxNumberOfRetries);

                if (export.Status != ExportState.Succeeded)
                {
                    throw new ApplicationException("Export Error: The export didn't succeed");
                }

                ExportedFile exportedFile = await GetExportedFile(reportId, export, accessToken, setting);
                return new Attachment(exportedFile.FileStream, export.ReportName + exportedFile.FileSuffix, MediaTypeNames.Application.Pdf);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Export Error: {ex.Message}");
            }
        }

        private static int? GetRetryAfterSeconds(Azure.Response rawResponse)
        {
            if (rawResponse == null)
                return null;
            if (rawResponse.Headers.TryGetValue("Retry-After", out string retryAfter)
                && int.TryParse(retryAfter, out int seconds))
            {
                return seconds;
            }
            return null;
        }

        public async Task<string> PostExportRequest(
    Guid reportId,
    string accessToken,
    PowerBISettings setting,
    FileFormat format,
    string rolesString,
    string username,
    IList<Page> pageNames = null, /* Get the page names from the GetPages REST API */
    string urlFilter = null,
    string locale = "en-us")
        {
            try
            {
                PortalSettings portalSettings = new PortalSettings(0);

                PowerBIClient client = new PowerBIClient(accessToken, new Uri(setting.ApiUrl));
                Reports reports = (await client.Reports.GetReportsInGroupAsync(Guid.Parse(setting.WorkspaceId)).ConfigureAwait(false)).Value;
                Report report = reports.Value.FirstOrDefault(r => r.Id.ToString().Equals(reportId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                if (report == null)
                {
                    throw new ApplicationException($"No report with ID '{reportId}' was found in workspace '{setting.WorkspaceId}'");
                }

                // The dataset may live in a different workspace (shared/golden dataset). Resolve its workspace first.
                var reportWorkspaceId = Guid.Parse(setting.WorkspaceId);
                var datasetWorkspaceId = await GetReportDatasetWorkspaceIdAsync(reportWorkspaceId, report.Id, accessToken, setting).ConfigureAwait(false)
                                         ?? reportWorkspaceId;
                Dataset dataset = null;
                try
                {
                    dataset = (await client.Datasets.GetDatasetInGroupAsync(datasetWorkspaceId, report.DatasetId).ConfigureAwait(false)).Value;
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Couldn't find dataset '{report.DatasetId}' in workspace '{datasetWorkspaceId}'", ex);
                    try
                    {
                        dataset = (await client.Datasets.GetDatasetsInGroupAsync(datasetWorkspaceId).ConfigureAwait(false)).Value.Value.FirstOrDefault(x => x.Id == report.DatasetId);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn($"Couldn't list datasets in workspace '{datasetWorkspaceId}'", ex2);
                    }
                }
                var powerBIReportExportConfiguration = new PowerBIReportExportConfiguration
                {
                    Settings = new ExportReportSettings
                    {
                        Locale = locale
                    },
                };
                if (pageNames != null)
                {
                    foreach (var page in pageNames)
                    {
                        powerBIReportExportConfiguration.Pages.Add(new ExportReportPage(pageName: page.Name));
                    }
                }
                if (!string.IsNullOrEmpty(urlFilter))
                {
                    powerBIReportExportConfiguration.ReportLevelFilters.Add(new ExportFilter { Filter = urlFilter });
                }

                // Let's check if RLS is required
                if (dataset != null
                    && (dataset.IsEffectiveIdentityRequired.GetValueOrDefault(false) || dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false)))
                {
                    var rls = new EffectiveIdentity { Username = username };
                    rls.Datasets.Add(report.DatasetId);
                    if (!string.IsNullOrWhiteSpace(rolesString) && dataset.IsEffectiveIdentityRolesRequired.GetValueOrDefault(false))
                    {
                        foreach (var role in rolesString.Split(','))
                        {
                            rls.Roles.Add(role);
                        }
                    }
                    powerBIReportExportConfiguration.Identities.Add(rls);
                }

                var exportRequest = new ExportReportRequest(format)
                {
                    PowerBIReportConfiguration = powerBIReportExportConfiguration,
                };

                // The 'Client' object is an instance of the Power BI .NET SDK                
                var export = (await client.Reports.ExportToFileInGroupAsync(Guid.Parse(setting.WorkspaceId), reportId, exportRequest).ConfigureAwait(false)).Value;
                return export.Id;
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Post Export Error: {e.Message}");
            }
        }

        private static async Task<Guid?> GetReportDatasetWorkspaceIdAsync(Guid reportWorkspaceId, Guid reportId, string accessToken, PowerBISettings setting)
        {
            try
            {
                var apiUrl = (setting.ApiUrl ?? "https://api.powerbi.com").TrimEnd('/');
                var requestUrl = $"{apiUrl}/v1.0/myorg/groups/{reportWorkspaceId}/reports/{reportId}";
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await http.GetAsync(requestUrl).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Warn($"GetReport REST call returned {response.StatusCode} for report {reportId} in workspace {reportWorkspaceId}");
                        return null;
                    }
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var token = JObject.Parse(json);
                    var dsWsId = (string)token["datasetWorkspaceId"];
                    if (!string.IsNullOrEmpty(dsWsId) && Guid.TryParse(dsWsId, out var parsed))
                    {
                        return parsed;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Error retrieving datasetWorkspaceId for report {reportId} in workspace {reportWorkspaceId}", ex);
                return null;
            }
        }

        public async Task<Response<Export>> PollExportRequest(Guid reportId, string exportId, int timeOutInMinutes,
                    CancellationToken token, string accessToken, PowerBISettings setting)
        {
            try
            {
                Response<Export> response = null;
                Export exportStatus = null;
                DateTime startTime = DateTime.UtcNow;
                if (!int.TryParse(ConfigurationManager.AppSettings["PowerBI.Export.PollInterval"], out int c_secToMillisec))
                    c_secToMillisec = 4000;

                var client = new PowerBIClient(accessToken, new Uri(setting.ApiUrl));
                do
                {
                    if (DateTime.UtcNow.Subtract(startTime).TotalMinutes > timeOutInMinutes || token.IsCancellationRequested)
                    {
                        return null;
                    }

                    // The 'Client' object is an instance of the Power BI .NET SDK
                    response = await client.Reports.GetExportToFileStatusInGroupAsync(Guid.Parse(setting.WorkspaceId), reportId, exportId).ConfigureAwait(false);
                    exportStatus = response.Value;

                    if (exportStatus.Status == ExportState.Running || exportStatus.Status == ExportState.NotStarted)
                    {
                        // The recommended waiting time between polling requests can be found in the Retry-After header
                        // Note that this header is not always populated
                        var retryAfterInSec = GetRetryAfterSeconds(response.GetRawResponse()) ?? 1;
                        await Task.Delay(retryAfterInSec * c_secToMillisec);
                    }
                }
                // While not in a terminal state, keep polling
                while (exportStatus.Status != ExportState.Succeeded && exportStatus.Status != ExportState.Failed);

                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Poll Export Error: {ex.Message}");
            }
        }

        public async Task<ExportedFile> GetExportedFile(
    Guid reportId,
    Export export, /* Get from the PollExportRequest response */
    string accessToken,
    PowerBISettings setting)
        {
            try
            {
                var client = new PowerBIClient(accessToken, new Uri(setting.ApiUrl));

                if (export.Status == ExportState.Succeeded)
                {
                    // The 'Client' object is an instance of the Power BI .NET SDK
                    var fileStream = (await client.Reports.GetFileOfExportToFileInGroupAsync(Guid.Parse(setting.WorkspaceId), reportId, export.Id).ConfigureAwait(false)).Value;
                    return new ExportedFile
                    {
                        FileStream = fileStream,
                        FileSuffix = export.ResourceFileExtension,
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Get Exported File Error: {ex.Message}");
            }
        }

        public async Task<Pages> GetReportPages(
            Guid reportId,
            string accessToken,
            PowerBISettings settings)
        {
            try
            {
                Pages pages = null;

                {
                    var client = new PowerBIClient(accessToken, new Uri(settings.ApiUrl));
                    pages = (await client.Reports.GetPagesInGroupAsync(Guid.Parse(settings.WorkspaceId), reportId).ConfigureAwait(false)).Value;
                }
                return pages;
            }
            catch
            (Exception ex)
            {
                throw new ApplicationException($"Get Report Pages Error: {ex.Message}");
            }

        }


        #endregion
    }

}