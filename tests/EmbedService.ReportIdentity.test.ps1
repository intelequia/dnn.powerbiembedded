$ErrorActionPreference = 'Stop'
$sourcePath = Join-Path $PSScriptRoot '../src/DotNetNuke.PowerBI/Services/EmbedService.cs'
$source = Get-Content $sourcePath -Raw
$helper = [regex]::Match($source, '(?s)        private async Task<ReportIdentityContext> ResolveReportIdentityAsync\(.*?(?=        private class ReportIdentityContext)').Value
if ([string]::IsNullOrWhiteSpace($helper)) { throw 'Shared resolver not found' }
if ([regex]::Matches($source, 'await ResolveReportIdentityAsync\(').Count -ne 2) {
    throw 'Both render and export must use the shared resolver'
}

$harness = @'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ReportIdentityTests {
public class Dataset {
    public string Id;
    public bool? IsEffectiveIdentityRequired;
    public bool? IsEffectiveIdentityRolesRequired;
}
public class Report { public Guid Id; public string DatasetId; }
public class EffectiveIdentity {
    public string Username;
    public List<string> Datasets = new List<string>();
    public List<string> Roles = new List<string>();
}
public class Response<T> { public T Value; }
public class DatasetList { public List<Dataset> Value; }
public class DatasetClient {
    public Dataset Primary;
    public bool FailLookup;
    public Task<Response<Dataset>> GetDatasetInGroupAsync(Guid workspace, string id) {
        if (FailLookup) throw new Exception("lookup failed");
        return Task.FromResult(new Response<Dataset> { Value = Primary });
    }
    public Task<Response<DatasetList>> GetDatasetsInGroupAsync(Guid workspace) {
        return Task.FromResult(new Response<DatasetList> {
            Value = new DatasetList { Value = new List<Dataset> { Primary } }
        });
    }
}
public class PowerBIClient { public DatasetClient Datasets = new DatasetClient(); }
public class ResolverCheck {
    private class ChainedDatasetInfo {
        public string DatasetId;
        public bool IsEffectiveIdentityRequired = false;
        public bool IsEffectiveIdentityRolesRequired;
    }
    private class ReportIdentityContext {
        public Guid DatasetWorkspaceId;
        public List<ChainedDatasetInfo> ChainedDatasets;
        public EffectiveIdentity Identity;
    }
    private static class Logger { public static void Warn(string message, Exception error) {} }
    private Guid? RemoteWorkspace;
    private List<ChainedDatasetInfo> Chained = new List<ChainedDatasetInfo>();
    private Task<Guid?> GetReportDatasetWorkspaceIdAsync(Guid workspace, Guid report) {
        return Task.FromResult(RemoteWorkspace);
    }
    private Task<List<ChainedDatasetInfo>> GetChainedDatasetsAsync(PowerBIClient client, Guid workspace, string dataset) {
        return Task.FromResult(Chained);
    }
    private void AppendEffectiveRoles(EffectiveIdentity identity, string roles) {
        identity.Roles.AddRange(roles.Split(','));
    }
__HELPER__
    private static void Check(bool condition, string name) {
        if (!condition) throw new Exception(name);
    }
    public static string Run() {
        var workspace = Guid.NewGuid();
        var report = new Report { Id = Guid.NewGuid(), DatasetId = "primary" };
        var client = new PowerBIClient();
        client.Datasets.Primary = new Dataset { Id = "primary" };
        var resolver = new ResolverCheck();
        var context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA,RoleB").GetAwaiter().GetResult();
        Check(context.Identity == null, "No RLS must not create identity");

        client.Datasets.Primary.IsEffectiveIdentityRolesRequired = true;
        context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA,RoleB").GetAwaiter().GetResult();
        Check(context.Identity.Username == "alice"
            && context.Identity.Datasets.SequenceEqual(new[] { "primary" })
            && context.Identity.Roles.Count == 2, "Primary RLS");

        client.Datasets.Primary.IsEffectiveIdentityRolesRequired = false;
        resolver.Chained.Add(new ChainedDatasetInfo { DatasetId = "remote", IsEffectiveIdentityRolesRequired = true });
        resolver.Chained.Add(new ChainedDatasetInfo { DatasetId = "remote", IsEffectiveIdentityRolesRequired = true });
        resolver.Chained.Add(new ChainedDatasetInfo { DatasetId = "no-rls" });
        context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA,RoleB").GetAwaiter().GetResult();
        Check(context.Identity.Datasets.SequenceEqual(new[] { "remote" })
            && context.Identity.Roles.Count == 2, "Remote RLS, deduplication and exclusion of non-RLS datasets");

        client.Datasets.Primary.IsEffectiveIdentityRequired = true;
        context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA").GetAwaiter().GetResult();
        Check(context.Identity.Datasets.SequenceEqual(new[] { "primary", "remote" }), "Primary and remote RLS");

        resolver.Chained.Clear();
        context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA").GetAwaiter().GetResult();
        Check(context.Identity.Roles.Count == 0, "Identity-only model must not receive roles");

        context = resolver.ResolveReportIdentityAsync(client, workspace, report, " ", "RoleA").GetAwaiter().GetResult();
        Check(context.Identity == null, "Empty username");

        resolver.RemoteWorkspace = Guid.NewGuid();
        client.Datasets.FailLookup = true;
        context = resolver.ResolveReportIdentityAsync(client, workspace, report, "alice", "RoleA").GetAwaiter().GetResult();
        Check(context.DatasetWorkspaceId == resolver.RemoteWorkspace
            && context.Identity.Datasets.Contains("primary"), "Cross-workspace and dataset-list fallback");
        return "PASS: 7 resolver scenarios; render and export use the shared resolver. SDK calls and role normalization are stubbed.";
    }
}
}
'@

Add-Type -TypeDefinition ($harness.Replace('__HELPER__', $helper))
[ReportIdentityTests.ResolverCheck]::Run()