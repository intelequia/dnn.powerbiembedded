using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Extensibility;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Web.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ExportsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ExportsController));

        [HttpGet]
        [HttpHead]
        public async Task<HttpResponseMessage> Download(string sid, string rid)
        {
            var model = new EmbedConfig();
            try
            {
                // Remove the objects without permissions
                
                if (!Guid.TryParse(sid, out Guid settingsGroupId) 
                    || !Guid.TryParse(rid, out Guid reportId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sid and Rid are required");
                }
                
                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId.ToString(), UserInfo))
                {
                    Logger.Error($"User {UserInfo.Username} doesn't have permissions for settings group {settingsGroupId}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                var embedService = new EmbedService(this.PortalSettings.PortalId, this.ActiveModule.TabModuleID, settingsGroupId.ToString());
                bool hasDownloadPermission = HasPermission(embedService.Settings, reportId.ToString(), 3);
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!hasDownloadPermission || (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : reportId.ToString(), UserInfo)))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                var contents = await embedService.GetContentListAsync(PortalSettings.UserId);
                var report = contents.Reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }

                Stream reportStream = await embedService.DownloadReportAsync(Guid.Parse(embedService.Settings.WorkspaceId), reportId);
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(reportStream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pbix");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{report.Name}.pbix"
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Export(string sid, string rid, string format)
        {
            try
            {
                
                FileFormat fileFormat = FileFormat.PDF;
                switch (format)
                {
                    case "pdf":
                        fileFormat = FileFormat.PDF;
                        break;
                    case "pptx":
                        fileFormat = FileFormat.PPTX;
                        break;
                    case "xlsx":
                        fileFormat = FileFormat.XLSX;
                        break;
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid format");
                }

                if (!Guid.TryParse(sid, out Guid settingsGroupId)
                    || !Guid.TryParse(rid, out Guid reportId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sid and Rid are required");
                }
                
                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId.ToString(), UserInfo))
                {
                    Logger.Error($"User {UserInfo.Username} doesn't have permissions for settings group {settingsGroupId}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                // Remove the objects without permissions
                var embedService = new EmbedService(this.PortalSettings.PortalId, this.ActiveModule.TabModuleID, settingsGroupId.ToString());
                var moduleSettings = ModuleController.Instance.GetTabModule(ActiveModule.TabModuleID).TabModuleSettings;

                bool hasDownloadPermission = HasPermission(embedService.Settings, reportId.ToString(), 3);
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!hasDownloadPermission || (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : reportId.ToString(), UserInfo)))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                var contents = await embedService.GetContentListAsync(PortalSettings.UserId);
                var report = contents.Reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }

                var user = PortalSettings.UserInfo.Username;
                var userPropertySetting = (string)moduleSettings["PowerBIEmbedded_UserProperty"];
                if (userPropertySetting?.ToLowerInvariant() == "email")
                {
                    user = PortalSettings.UserInfo.Email;
                }
                else if (userPropertySetting == "PowerBiGroup")
                {
                    var userProperty = PortalSettings.UserInfo.Profile.GetProperty("PowerBiGroup");
                    if (userProperty?.PropertyValue != null)
                    {
                        user = userProperty.PropertyValue;
                    }
                }
                else if (userPropertySetting == "Custom" || userPropertySetting == "Custom User Profile Property")
                {
                    var customProperties = (string)moduleSettings["PowerBIEmbedded_CustomUserProperty"];
                    var matches = Regex.Matches(customProperties, @"\[PROFILE:(?<PROPERTY>[A-z]*)]");

                    foreach (Match match in matches)
                    {
                        var userProperty = PortalSettings.UserInfo.Profile.GetProperty(match.Groups["PROPERTY"].Value);
                        if (userProperty?.PropertyValue != null)
                        {
                            customProperties = customProperties.Replace(match.Value, userProperty.PropertyValue);
                        }
                    }

                    user = customProperties;
                }
                else if (userPropertySetting == "Custom Extension Library")
                {
                    var customExtensionLibrary = (string)moduleSettings["PowerBIEmbedded_CustomExtensionLibrary"];
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
                            Logger.Error($"Error instancing custom extension library '{customExtensionLibrary}'", cex);
                            throw;
                        }
                    }
                }
                var roles = string.Join(",", PortalSettings.UserInfo.Roles);

                Export export = await embedService.ExportReportAsync(Guid.Parse(embedService.Settings.WorkspaceId), reportId, fileFormat, user, roles);
                if (string.IsNullOrEmpty(export.ReportName))
                {
                    export.ReportName = $"{report.Name}.{format}";
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, export);
            }
            catch(Microsoft.Rest.HttpOperationException httpEx)
            {
                if (httpEx.Response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                else if (httpEx.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }
                else if (httpEx.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
                }
                else
                {
                    Logger.Error(httpEx);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, httpEx);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ExportStatus(string sid, string rid, string exportId)
        {
            try
            {
                if (!Guid.TryParse(sid, out Guid settingsGroupId)
                    || !Guid.TryParse(rid, out Guid reportId)
                    || string.IsNullOrEmpty(exportId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sid, Rid and exportId are required");
                }

                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId.ToString(), UserInfo))
                {
                    Logger.Error($"User {UserInfo.Username} doesn't have permissions for settings group {settingsGroupId}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                // Remove the objects without permissions
                var embedService = new EmbedService(this.PortalSettings.PortalId, this.ActiveModule.TabModuleID, settingsGroupId.ToString());
                var moduleSettings = ModuleController.Instance.GetTabModule(ActiveModule.TabModuleID).TabModuleSettings;

                bool hasDownloadPermission = HasPermission(embedService.Settings, reportId.ToString(), 3);
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!hasDownloadPermission || (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : reportId.ToString(), UserInfo)))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                var contents = await embedService.GetContentListAsync(PortalSettings.UserId);
                var report = contents.Reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }
                Export export = await embedService.GetExportStatusAsync(Guid.Parse(embedService.Settings.WorkspaceId), reportId, exportId);

                return Request.CreateResponse(HttpStatusCode.OK, export);

            }
            catch (HttpOperationException httpEx)
            {
                if (httpEx.Response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                else if (httpEx.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }
                else if (httpEx.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
                }
                else
                {
                    Logger.Error(httpEx);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, httpEx);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [HttpHead]
        public async Task<HttpResponseMessage> GetExportedFile(string sid, string rid, string exportId, string resourceFileExtension)
        {
            var model = new EmbedConfig();
            try
            {
                // Remove the objects without permissions
                if (!Guid.TryParse(sid, out Guid settingsGroupId)
                    || !Guid.TryParse(rid, out Guid reportId)
                    || string.IsNullOrEmpty(exportId)
                    || string.IsNullOrEmpty(resourceFileExtension))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sid, Rid, exportId and resourceFileExtension are required");
                }

                if (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(settingsGroupId.ToString(), UserInfo))
                {
                    Logger.Error($"User {UserInfo.Username} doesn't have permissions for settings group {settingsGroupId}");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }

                var embedService = new EmbedService(this.PortalSettings.PortalId, this.ActiveModule.TabModuleID, settingsGroupId.ToString());
                bool hasDownloadPermission = HasPermission(embedService.Settings, reportId.ToString(), 3);
                var permissionsRepo = ObjectPermissionsRepository.Instance;
                if (!hasDownloadPermission || (!PowerBIListViewExtensions.UserHasPermissionsToWorkspace(embedService.Settings.InheritPermissions ?
                    embedService.Settings.SettingsGroupId : reportId.ToString(), UserInfo)))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "User doesn't have permissions for this resource");
                }
                var contents = await embedService.GetContentListAsync(PortalSettings.UserId);
                var report = contents.Reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Report not found");
                }

                Stream exportStream = await embedService.GetExportedFileAsync(Guid.Parse(embedService.Settings.WorkspaceId), reportId, exportId, resourceFileExtension);
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(exportStream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/" + resourceFileExtension);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{report.Name}.{resourceFileExtension}"
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private bool HasPermission(PowerBISettings settings, string reportId, int permissionId)
        {
            bool hasInheritPermissions = settings.InheritPermissions;
            string comparison = hasInheritPermissions ? settings.SettingsGroupId : reportId;
            UserInfo user = PortalSettings.UserInfo;
            return PowerBIListViewExtensions.UserHasPermissionsToWorkspace(comparison, user, permissionId);
        }
    }
}