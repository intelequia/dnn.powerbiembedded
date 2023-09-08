using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Services.Localization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using UserInfo = DotNetNuke.Entities.Users.UserInfo;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace DotNetNuke.PowerBI.Components
{
    public class Common
    {

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
            if (CurrentUser.UserID == -1)
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
        public async Task<TokenCredentials> GetTokenCredentials(PowerBISettings setting)
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

            return new TokenCredentials(authenticationResult.AccessToken, "Bearer");
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
        #endregion

        #region Export Report

        public async Task<Attachment> ExportPowerBIReport(
            Guid reportId,
            TokenCredentials tokenCredentials,
            PowerBISettings setting,
            string reportPages)
        {
            try
            {
                string urlFilter = null;
                int pollingtimeOutInMinutes = 1;
                FileFormat format = FileFormat.PDF;
                Pages pageNames = await GetReportPages(reportId, tokenCredentials, setting);
                string[] pages = reportPages.Split(',');
                if (reportPages != "All" && reportPages != "")
                {
                    pageNames.Value = pageNames.Value.Where(page => pages.Contains(page.Name)).ToList();
                }

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                const int c_maxNumberOfRetries = 3; /* Can be set to any desired number */
                const int c_secToMillisec = 1000;

                Export export = null;
                int retryAttempt = 1;
                do
                {
                    var exportId = await PostExportRequest(reportId, tokenCredentials, setting, format, pageNames, urlFilter);
                    var httpMessage = await PollExportRequest(reportId, exportId, pollingtimeOutInMinutes, cancellationToken, tokenCredentials, setting);
                    export = httpMessage.Body;
                    if (export == null)
                    {
                        // Error, failure in exporting the report
                        return null;
                    }
                    if (export.Status == ExportState.Failed)
                    {
                        // Some failure cases indicate that the system is currently busy. The entire export operation can be retried after a certain delay
                        // In such cases the recommended waiting time before retrying the entire export operation can be found in the RetryAfter header
                        var retryAfter = httpMessage.Response.Headers.RetryAfter;
                        if (retryAfter == null)
                        {
                            // Failed state with no RetryAfter header indicates that the export failed permanently
                            return null;
                        }

                        var retryAfterInSec = retryAfter.Delta.Value.Seconds;
                        await Task.Delay(retryAfterInSec * c_secToMillisec);
                    }
                }
                while (export.Status != ExportState.Succeeded && retryAttempt++ < c_maxNumberOfRetries);

                if (export.Status != ExportState.Succeeded)
                {
                    // Error, failure in exporting the report
                    return null;
                }

                ExportedFile exportedFile = await GetExportedFile(reportId, export, tokenCredentials, setting);

                Attachment attachment = new Attachment(exportedFile.FileStream, export.ReportName + exportedFile.FileSuffix, MediaTypeNames.Application.Pdf);
                return attachment;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<string> PostExportRequest(
    Guid reportId,
    TokenCredentials tokenCredentials,
    PowerBISettings setting,
    FileFormat format,
    Pages pageNames = null, /* Get the page names from the GetPages REST API */
    string urlFilter = null)
        {
            try
            {
                IList<ExportReportPage> pages = new List<ExportReportPage>();
                foreach (Page page in pageNames.Value)
                {
                    ExportReportPage exportPage = new ExportReportPage();
                    exportPage.PageName = page.Name;
                    pages.Add(exportPage);
                }
                var powerBIReportExportConfiguration = new PowerBIReportExportConfiguration
                {
                    Settings = new ExportReportSettings
                    {
                        Locale = "en-us"
                    },
                    // Note that page names differ from the page display names
                    // To get the page names use the GetPages REST API
                    Pages = pages,
                    // ReportLevelFilters collection needs to be instantiated explicitly
                    ReportLevelFilters = !string.IsNullOrEmpty(urlFilter) ? new List<ExportFilter>() { new ExportFilter(urlFilter) } : null,

                };

                var exportRequest = new ExportReportRequest
                {
                    Format = format,
                    PowerBIReportConfiguration = powerBIReportExportConfiguration,
                };

                // The 'Client' object is an instance of the Power BI .NET SDK
                using (var client = new PowerBIClient(new Uri(setting.ApiUrl), tokenCredentials))
                {
                    var export = await client.Reports.ExportToFileAsync(reportId, exportRequest);
                    return export.Id;

                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<HttpOperationResponse<Export>> PollExportRequest(
    Guid reportId,
    string exportId,
    int timeOutInMinutes,
    CancellationToken token,
    TokenCredentials tokenCredentials,
    PowerBISettings setting)
        {
            try
            {
                HttpOperationResponse<Export> httpMessage = null;
                Export exportStatus = null;
                DateTime startTime = DateTime.UtcNow;
                const int c_secToMillisec = 1000;

                var client = new PowerBIClient(new Uri(setting.ApiUrl), tokenCredentials);
                do
                {
                    if (DateTime.UtcNow.Subtract(startTime).TotalMinutes > timeOutInMinutes || token.IsCancellationRequested)
                    {
                        return null;
                    }

                    // The 'Client' object is an instance of the Power BI .NET SDK
                    httpMessage = await client.Reports.GetExportToFileStatusWithHttpMessagesAsync(reportId, exportId);
                    exportStatus = httpMessage.Body;

                    if (exportStatus.Status == ExportState.Running || exportStatus.Status == ExportState.NotStarted)
                    {
                        // The recommended waiting time between polling requests can be found in the RetryAfter header
                        // Note that this header is not always populated
                        var retryAfter = httpMessage.Response.Headers.RetryAfter;
                        var retryAfterInSec = retryAfter.Delta.Value.Seconds;
                        await Task.Delay(retryAfterInSec * c_secToMillisec);
                    }
                }
                // While not in a terminal state, keep polling
                while (exportStatus.Status != ExportState.Succeeded && exportStatus.Status != ExportState.Failed);

                return httpMessage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ExportedFile> GetExportedFile(
    Guid reportId,
    Export export, /* Get from the PollExportRequest response */
    TokenCredentials tokenCredentials,
    PowerBISettings setting)
        {
            try
            {
                var client = new PowerBIClient(new Uri(setting.ApiUrl), tokenCredentials);

                if (export.Status == ExportState.Succeeded)
                {
                    // The 'Client' object is an instance of the Power BI .NET SDK
                    var fileStream = await client.Reports.GetFileOfExportToFileAsync(reportId, export.Id);
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
                return null;
            }
        }

        public async Task<Pages> GetReportPages(
            Guid reportId,
            TokenCredentials tokenCredentials,
            PowerBISettings settings)
        {
            Pages pages = null;

            using (var client = new PowerBIClient(new Uri(settings.ApiUrl), tokenCredentials))
            {
                pages = client.Reports.GetPages(reportId);
            }
            return pages;
        }


        #endregion
    }

}