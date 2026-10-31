$ErrorActionPreference = 'Stop'
$source = Get-Content (Join-Path $PSScriptRoot '..\src\DotNetNuke.PowerBI\Services\StatsController.cs') -Raw
$method = [regex]::Match($source, '(?s)        private async Task<WorkspaceDatasetSnapshot> GetWorkspaceDatasetSnapshotAsync\(.*?(?=        private async Task FillApplicationInsightsStatsAsync)').Value
$snapshot = [regex]::Match($source, '(?s)        private class WorkspaceDatasetSnapshot.*?(?=        private class ReportTabInfo)').Value
if ([string]::IsNullOrWhiteSpace($method) -or [string]::IsNullOrWhiteSpace($snapshot)) {
    throw 'Dataset snapshot method or model not found'
}
$harness = @'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure;
namespace Azure {
    public class RequestFailedException : Exception {
        public int Status { get; private set; }
        public string ErrorCode { get; private set; }
        public RequestFailedException(int status, string message, string code) : base(message) {
            Status = status;
            ErrorCode = code;
        }
    }
}
namespace StatsRefreshHistoryTests {
public class PowerBISettings {
    public string WorkspaceId = "c6b30960-fc9f-42bd-8f58-09ad17237be3";
    public string ApiUrl;
}
public class Dataset {
    public string Id;
    public string Name;
    public bool? IsRefreshable;
}
public class Refresh {
    public DateTimeOffset? StartTime;
    public DateTimeOffset? EndTime;
    public string RefreshType = "Scheduled";
    public string Status = "Completed";
    public string ServiceExceptionJson;
}
public class Response<T> { public T Value; }
public class Items<T> { public List<T> Value; }
public class DatasetClient {
    public List<Dataset> Items = new List<Dataset>();
    public Dictionary<string, List<Refresh>> Histories = new Dictionary<string, List<Refresh>>();
    public Dictionary<string, Exception> Errors = new Dictionary<string, Exception>();
    public List<string> Calls = new List<string>();
    public Task<Response<Items<Dataset>>> GetDatasetsInGroupAsync(Guid workspace) {
        return Task.FromResult(new Response<Items<Dataset>> { Value = new Items<Dataset> { Value = Items } });
    }
    public Task<Response<Items<Refresh>>> GetRefreshHistoryInGroupAsync(Guid workspace, string id, int top) {
        if (top != 1) throw new Exception("Snapshot must request only the latest refresh");
        Calls.Add(id);
        if (Errors.ContainsKey(id)) throw Errors[id];
        return Task.FromResult(new Response<Items<Refresh>> {
            Value = new Items<Refresh> { Value = Histories.ContainsKey(id) ? Histories[id] : new List<Refresh>() }
        });
    }
}
public class PowerBIClient {
    public static DatasetClient Current;
    public DatasetClient Datasets;
    public PowerBIClient(string token, Uri url) { Datasets = Current; }
}
public class TestLogger {
    public int Warnings;
    public int DebugMessages;
    public void Warn(string message, Exception ex) { Warnings++; }
    public void Debug(string message) { DebugMessages++; }
}
public class Checks {
    private static TestLogger Logger = new TestLogger();
    private Task<string> AcquireAccessTokenAsync(PowerBISettings setting) { return Task.FromResult("token"); }
    private bool IsAutoRefresh(string type) { return type == "Scheduled"; }
__SNAPSHOT__
__METHOD__
    private static void Check(bool condition, string name) { if (!condition) throw new Exception(name); }
    private static DatasetClient Setup() {
        Logger = new TestLogger();
        var client = new DatasetClient();
        client.Items.Add(new Dataset { Id = "model", Name = "Model", IsRefreshable = true });
        client.Histories["model"] = new List<Refresh> {
            new Refresh { StartTime = DateTimeOffset.UtcNow.AddMinutes(-10), EndTime = DateTimeOffset.UtcNow.AddMinutes(-5) }
        };
        PowerBIClient.Current = client;
        return client;
    }
    private static WorkspaceDatasetSnapshot RunSnapshot() {
        return new Checks().GetWorkspaceDatasetSnapshotAsync(new PowerBISettings()).GetAwaiter().GetResult();
    }
    public static string Run() {
        const string unsupported = "Invalid dataset. This API can only be called on a Model-based dataset";
        var client = Setup();
        client.Items.Add(new Dataset { Id = "push", Name = "Push", IsRefreshable = false });
        client.Items.Add(new Dataset { Id = "usage", Name = "Report Usage Metrics Model", IsRefreshable = true });
        var result = RunSnapshot();
        Check(client.Calls.SequenceEqual(new[] { "model" }), "Do not query non-refreshable or usage datasets");
        Check(result.DatasetCount == 2, "Keep non-refreshable datasets in the total");
        Check(result.Health == "green" && result.AutoRefreshedLast24h == 1 &&
            result.LastRefreshUtc == client.Histories["model"][0].EndTime.Value.UtcDateTime,
            "Preserve supported refresh metrics and health");
        Check(Logger.Warnings == 0, "Skipped datasets must not warn");

        foreach (bool? refreshable in new bool?[] { null, true }) {
            client = Setup();
            client.Items.Add(new Dataset { Id = "unsupported", Name = "Unsupported", IsRefreshable = refreshable });
            client.Errors["unsupported"] = new RequestFailedException(415, unsupported, "InvalidRequest");
            result = RunSnapshot();
            Check(client.Calls.Contains("unsupported"), "Attempt unknown or supported refreshability");
            Check(result.DatasetCount == 2 && result.Health == "green" && result.AutoRefreshedLast24h == 1,
                "Specific unsupported response must not degrade supported statistics");
            Check(Logger.Warnings == 0 && Logger.DebugMessages == 1, "Unsupported response logged only at debug level");
        }

        foreach (var error in new Exception[] {
            new RequestFailedException(403, unsupported, "InvalidRequest"),
            new RequestFailedException(429, "Too many requests", "TooManyRequests"),
            new RequestFailedException(500, "Service failure", "InternalError"),
            new RequestFailedException(415, "Different invalid request", "InvalidRequest"),
            new RequestFailedException(415, unsupported, "OtherError"),
            new Exception(unsupported)
        }) {
            client = Setup();
            client.Items.Add(new Dataset { Id = "error", Name = "Error", IsRefreshable = true });
            client.Errors["error"] = error;
            result = RunSnapshot();
            Check(result.Health == "yellow" && Logger.Warnings == 1 && Logger.DebugMessages == 0,
                "Other errors must retain warnings and degrade health");
            Check(result.AutoRefreshedLast24h == 1, "Other failures must not discard successful metrics");
        }

        client = Setup();
        client.Histories["model"][0].Status = "Failed";
        Check(RunSnapshot().Health == "yellow", "Failed refresh must still degrade health");
        client = Setup();
        client.Histories["model"][0].ServiceExceptionJson = "{}";
        Check(RunSnapshot().Health == "yellow", "Service exceptions must still degrade health");
        client = Setup();
        client.Histories["model"].Clear();
        result = RunSnapshot();
        Check(result.Health == "yellow" && !result.LastRefreshUtc.HasValue, "Preserve empty history indication");
        client = Setup();
        client.Items.Clear();
        client.Items.Add(new Dataset { Id = "push", Name = "Push", IsRefreshable = false });
        result = RunSnapshot();
        Check(result.DatasetCount == 1 && result.Health == "yellow" && client.Calls.Count == 0,
            "Workspace with only non-refreshable datasets preserves no-data indication");
        client.Items[0].IsRefreshable = null;
        client.Errors["push"] = new RequestFailedException(415, unsupported, "InvalidRequest");
        result = RunSnapshot();
        Check(result.Health == "yellow" && !result.HasFatalError && Logger.Warnings == 0,
            "Workspace with only unsupported histories preserves no-data indication without warnings");
        return "PASS: refreshability, exact unsupported error filtering, dataset totals, refresh metrics, health and logging. SDK/DNN dependencies are stubbed.";
    }
}
}
'@
Add-Type -TypeDefinition ($harness.Replace('__METHOD__', $method).Replace('__SNAPSHOT__', $snapshot))
[StatsRefreshHistoryTests.Checks]::Run()
