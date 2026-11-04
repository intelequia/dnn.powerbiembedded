$ErrorActionPreference = 'Stop'
$source = Get-Content (Join-Path $PSScriptRoot '..\src\DotNetNuke.PowerBI\Components\DatasetRefreshRetry.cs') -Raw
$stubs = @'
namespace Newtonsoft.Json.Linq {
    public class JObject {
        private string json;
        public static JObject Parse(string text) {
            if (!text.StartsWith("{")) throw new System.FormatException();
            return new JObject { json = text };
        }
        public string this[string key] {
            get {
                var name = "\"" + key + "\":\"";
                var index = json.IndexOf(name, System.StringComparison.Ordinal);
                if (index < 0) return null;
                index += name.Length;
                var end = json.IndexOf("\"", index, System.StringComparison.Ordinal);
                return json.Substring(index, end - index);
            }
        }
    }
}
namespace Microsoft.PowerBI.Api.Models {
    public enum RefreshType { Scheduled, ViaApi, OnDemand }
    public class Refresh {
        public System.DateTimeOffset? StartTime { get; set; }
        public System.DateTimeOffset? EndTime { get; set; }
        public RefreshType? RefreshType { get; set; }
        public string Status { get; set; }
        public string ServiceExceptionJson { get; set; }
    }
}
'@
Add-Type -TypeDefinition ($source + $stubs)
$now = [DateTimeOffset]::UtcNow
function Refresh($type, $status, [int]$minutes, $error) {
    return [Microsoft.PowerBI.Api.Models.Refresh]@{
        RefreshType = [Microsoft.PowerBI.Api.Models.RefreshType]::$type
        Status = $status
        StartTime = $now.AddMinutes($minutes)
        EndTime = $now.AddMinutes($minutes + 1)
        ServiceExceptionJson = $error
    }
}
function Check($condition, $message) {
    if (-not $condition) { throw $message }
}
$transient = '{"errorCode":"GatewayTimeout","errorDescription":"Gateway timed out"}'
$permanent = '{"errorCode":"InvalidCredentials","errorDescription":"Invalid credentials"}'
$scheduled = Refresh 'Scheduled' 'Failed' -20 $transient
$retry = Refresh 'ViaApi' 'Failed' -10 $transient
$policy = [DotNetNuke.PowerBI.Components.DatasetRefreshRetry]
function Evaluate($items, $max, $delay) {
    return $policy::Evaluate([Microsoft.PowerBI.Api.Models.Refresh[]]$items, $max, $delay, $now)
}
Check ((Evaluate @($scheduled) 2 5).Status -eq 'Due') 'Scheduled transient failure must be retried'
Check ((Evaluate @($scheduled) 2 30).Status -eq 'Pending') 'Delay must be honored'
Check ((Evaluate @($scheduled) 0 5).Status -eq 'None') 'Disabled policy must not retry'
Check ((Evaluate @($retry, $scheduled) 2 5).Attempts -eq 1) 'Retry history must count attempts'
Check ((Evaluate @($retry, $scheduled) 1 5).Status -eq 'FinalFailure') 'Retry limit must stop attempts'
Check ((Evaluate @((Refresh 'Scheduled' 'Failed' -1 $transient), $retry, $scheduled) 1 5).Status -eq 'Pending') 'A new scheduled failure starts a new retry chain'
Check ((Evaluate @((Refresh 'ViaApi' 'Completed' -1 $null), $retry, $scheduled) 3 5).Status -eq 'None') 'Success stops retries'
Check ((Evaluate @((Refresh 'OnDemand' 'Failed' -1 $transient), $scheduled) 3 5).Status -eq 'None') 'Manual refresh does not join a retry chain'
Check ((Evaluate @((Refresh 'ViaApi' 'InProgress' -1 $null), $scheduled) 2 5).Status -eq 'Active') 'Active refresh blocks retries'
Check ((Evaluate @((Refresh 'Scheduled' 'Failed' -20 $permanent)) 2 5).Status -eq 'FinalFailure') 'Invalid credentials must not retry'
Check ((Evaluate @((Refresh 'Scheduled' 'Failed' -20 $null)) 2 5).Status -eq 'FinalFailure') 'Unknown errors must not retry'
Check ((Evaluate @((Refresh 'ViaApi' 'Failed' -20 $transient)) 2 5).Status -eq 'None') 'Manual API refresh must not start a chain'
'PASS: scheduled refresh retry policy, timing, terminal errors and overlapping refresh'
