using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Data.SharedSettings;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetNuke.PowerBI.Services
{
    /// <summary>
    /// WebAPI endpoints that feed the CalendarView module from the client side.
    /// They replace the previous full-page MVC postbacks (workspace switch and
    /// history pagination) with asynchronous AJAX calls.
    /// </summary>
    [SupportedModules("DotNetNuke.PowerBI.CalendarView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class CalendarController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(CalendarController));

        /// <summary>
        /// Returns the refresh schedules (FullCalendar events) for the given workspace
        /// selection. "cid" is the workspace id, or "-1" for all workspaces.
        /// </summary>
        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetSchedule(string cid)
        {
            try
            {
                var model = GetCalendarModel(cid);
                if (model == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { schedules = new object[0], workspaces = new object[0] });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    schedules = model.RefreshSchedules,
                    workspaces = model.Workspaces
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Returns a page of the dataset refresh history for the given workspace selection.
        /// </summary>
        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetHistory(string cid, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }
                if (pageSize < 1)
                {
                    pageSize = 10;
                }

                var model = GetCalendarModel(cid);
                if (model == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { history = new object[0], currentPage = 1, totalPages = 0, count = 0 });
                }

                var count = model.History.Count;
                var totalPages = pageSize > 0 ? (int)Math.Ceiling(decimal.Divide(count, pageSize)) : 0;
                if (totalPages > 0 && page > totalPages)
                {
                    page = totalPages;
                }

                var history = model.History
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new
                    {
                        dataset = h.Dataset,
                        workspace = h.WorkSpaceName,
                        capacity = h.CapacityName,
                        startTime = h.StartTime.HasValue ? h.StartTime.Value.ToString("g") : string.Empty,
                        endTime = h.EndTime.HasValue ? h.EndTime.Value.ToString("g") : string.Empty,
                        refreshType = h.RefreshType.HasValue ? h.RefreshType.Value.ToString() : string.Empty,
                        status = h.Status,
                        errorDetails = ExtractRefreshError(h.ServiceExceptionJson)
                    })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { history, currentPage = page, totalPages, count });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Resolves the settings group and loads the calendar model, mirroring the
        /// resolution that the MVC controller used to do server-side. The underlying
        /// EmbedService caches the result for 15 minutes, so calling this from both
        /// endpoints (schedule + history) is cheap.
        /// </summary>
        private PowerBICalendarView GetCalendarModel(string cid)
        {
            var portalId = ActiveModule.PortalID;
            var pbiSettings = SharedSettingsRepository.Instance.GetSettings(portalId);

            var settingsGroupId = cid;
            if (string.IsNullOrEmpty(settingsGroupId) || settingsGroupId == "-1")
            {
                var defaultPbiSettingsGroupId = GetModuleSetting("PowerBIEmbedded_SettingsGroupId");
                if (!string.IsNullOrEmpty(defaultPbiSettingsGroupId) && pbiSettings.Any(x => x.SettingsGroupId == defaultPbiSettingsGroupId))
                {
                    settingsGroupId = defaultPbiSettingsGroupId;
                }
                else
                {
                    settingsGroupId = pbiSettings.FirstOrDefault(x => !string.IsNullOrEmpty(x.SettingsGroupId))?.SettingsGroupId;
                }
            }

            var mode = (string.IsNullOrEmpty(cid) || cid == "-1") ? "-1" : cid;
            var embedService = new EmbedService(portalId, ActiveModule.TabModuleID, settingsGroupId);
            return embedService.GetScheduleInGroup(mode).Result;
        }

        private string GetModuleSetting(string key)
        {
            if (ActiveModule == null)
            {
                return null;
            }

            // TabModuleSettings take precedence over ModuleSettings, matching how the
            // MVC ModuleContext.Settings dictionary resolves the value.
            Hashtable tabModuleSettings = ActiveModule.TabModuleSettings;
            if (tabModuleSettings != null && tabModuleSettings.ContainsKey(key))
            {
                return Convert.ToString(tabModuleSettings[key]);
            }

            Hashtable moduleSettings = ActiveModule.ModuleSettings;
            if (moduleSettings != null && moduleSettings.ContainsKey(key))
            {
                return Convert.ToString(moduleSettings[key]);
            }

            return null;
        }

        /// <summary>
        /// Extracts a human readable error message from the Power BI refresh
        /// ServiceExceptionJson payload (e.g. {"errorCode":"...","errorDescription":"..."}).
        /// Falls back to the raw payload when it cannot be parsed.
        /// </summary>
        private static string ExtractRefreshError(string serviceExceptionJson)
        {
            if (string.IsNullOrWhiteSpace(serviceExceptionJson))
            {
                return string.Empty;
            }

            try
            {
                var json = JObject.Parse(serviceExceptionJson);
                var description = (string)json["errorDescription"];
                var code = (string)json["errorCode"];

                if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(code))
                {
                    return $"[{code}] {description}";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description;
                }

                if (!string.IsNullOrWhiteSpace(code))
                {
                    return code;
                }
            }
            catch
            {
                // Not valid JSON - return the raw payload below.
            }

            return serviceExceptionJson;
        }
    }
}
