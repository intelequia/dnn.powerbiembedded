using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI.Components
{
    public class DatasetRefreshRetry
    {
        public string Status { get; set; }
        public int Attempts { get; set; }
        public DateTimeOffset? NextAttemptUtc { get; set; }

        public static DatasetRefreshRetry Evaluate(IEnumerable<Refresh> history, int maxRetries, int delayMinutes, DateTimeOffset now)
        {
            var result = new DatasetRefreshRetry { Status = "None" };
            if (maxRetries <= 0 || delayMinutes <= 0 || history == null)
                return result;

            var ordered = history.Where(h => h.StartTime.HasValue).OrderByDescending(h => h.StartTime).ToList();
            if (ordered.Count == 0)
                return result;

            var latest = ordered[0];
            if (string.Equals(latest.Status, "InProgress", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(latest.Status, "Unknown", StringComparison.OrdinalIgnoreCase))
            {
                result.Status = "Active";
                return result;
            }
            if (!string.Equals(latest.Status, "Failed", StringComparison.OrdinalIgnoreCase))
                return result;

            // Only a scheduled failure can start a chain. ViaApi entries after it are retry
            // attempts; a successful or unrelated refresh terminates the chain.
            foreach (var refresh in ordered)
            {
                if (!string.Equals(refresh.Status, "Failed", StringComparison.OrdinalIgnoreCase))
                    return result;
                if (refresh.RefreshType == RefreshType.Scheduled)
                    break;
                if (refresh.RefreshType != RefreshType.ViaApi)
                    return result;
                result.Attempts++;
            }
            if (!ordered.Skip(result.Attempts).Any(h => h.RefreshType == RefreshType.Scheduled &&
                string.Equals(h.Status, "Failed", StringComparison.OrdinalIgnoreCase)))
                return new DatasetRefreshRetry { Status = "None" };

            if (!IsRecoverable(latest.ServiceExceptionJson))
            {
                result.Status = "FinalFailure";
                return result;
            }
            if (result.Attempts >= maxRetries)
            {
                result.Status = "FinalFailure";
                return result;
            }

            result.NextAttemptUtc = latest.EndTime ?? latest.StartTime;
            result.NextAttemptUtc = result.NextAttemptUtc.Value.AddMinutes(delayMinutes);
            result.Status = result.NextAttemptUtc <= now ? "Due" : "Pending";
            return result;
        }

        public static bool IsRecoverable(string errorJson)
        {
            if (string.IsNullOrWhiteSpace(errorJson))
                return false;
            try
            {
                var error = JObject.Parse(errorJson);
                var code = ((string)error["errorCode"] ?? "").ToLowerInvariant();
                var description = ((string)error["errorDescription"] ?? "").ToLowerInvariant();
                var text = code + " " + description;
                if (text.Contains("credential") || text.Contains("unauthorized") ||
                    text.Contains("permission") || text.Contains("forbidden") ||
                    text.Contains("invalid") || text.Contains("authentication"))
                    return false;
                return text.Contains("timeout") || text.Contains("timed out") ||
                    text.Contains("throttl") || text.Contains("too many requests") ||
                    text.Contains("temporar") || text.Contains("service unavailable") ||
                    text.Contains("internal server error") || text.Contains("gateway unavailable");
            }
            catch
            {
                return false;
            }
        }
    }
}
