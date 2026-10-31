$ErrorActionPreference = 'Stop'
$source = Get-Content (Join-Path $PSScriptRoot '..\src\DotNetNuke.PowerBI\Services\SubscriptionController.cs') -Raw
$helpers = [regex]::Match($source, '(?s)        private static bool UserCanManageAllSubscriptions\(.*?(?=        private bool UserHasPermission)').Value
if ([string]::IsNullOrWhiteSpace($helpers)) { throw 'Ownership helpers not found' }
$list = [regex]::Match($source, '(?s)                var subscriptions = SubscriptionsRepository.Instance.GetSubscriptionsByReportId.*?\.ToList\(\);').Value
if ([string]::IsNullOrWhiteSpace($list)) { throw 'Subscription list filter not found' }
foreach ($endpoint in @('EditSubscription', 'DeleteSubscription', 'SaveMyChanges', 'RunSubscriptionNow')) {
    $body = [regex]::Match($source, "(?s)public HttpResponseMessage $endpoint\(.*?(?=\r?\n        (?:\[Http|private ))").Value
    $guard = $body.IndexOf('!UserCanManageSubscription(')
    if ($guard -lt 0) { throw "$endpoint must enforce ownership" }
    $mutations = [regex]::Matches($body, '(?:SubscriptionsRepository.Instance.(?:Edit|Delete)Subscription|SaveMyChangesBookmark|HostingEnvironment.QueueBackgroundWorkItem)\(')
    foreach ($mutation in $mutations) {
        if ($mutation.Index -lt $guard) { throw "$endpoint mutates state before authorizing ownership" }
    }
}
if ($source -notmatch 'subscription.OwnerDisplayName = UserController.Instance.GetUserById\(portalId, subscription.CreatedBy\)\?\.DisplayName') {
    throw 'Owner must be resolved from the persisted creator'
}
$repository = Get-Content (Join-Path $PSScriptRoot '..\src\DotNetNuke.PowerBI\Data\Subscriptions\SubscriptionsRepository.cs') -Raw
if ([regex]::Matches($repository, 'subscription.CreatedBy =').Count -ne 1 -or
    $repository -notmatch 'subscription.CreatedBy = Components.Common.CurrentUser.UserID') {
    throw 'Only creation may assign ownership, using the authenticated user'
}
if ($source -notmatch '(?s)private HttpResponseMessage AddSubscription.*?PortalId = portalId,') {
    throw 'Creation must use the active portal rather than a client-supplied portal'
}
$status = [regex]::Match($source, '(?s)public HttpResponseMessage GetRunSubscriptionStatus\(.*?(?=        \[HttpPost\])').Value
if ($status -notmatch 'job.PortalId != ActiveModule.PortalID' -or $status -notmatch 'job.UserId != currentUser.UserID' -or
    $status.IndexOf('HttpStatusCode.Unauthorized') -gt $status.IndexOf('RunNowJobs.TryRemove')) {
    throw 'Run status must authorize the portal and initiating user before removing a job'
}

$harness = @'
using System;
using System.Collections.Generic;
using System.Linq;
namespace SubscriptionOwnershipTests {
public class UserInfo {
    public int UserID;
    public bool IsSuperUser;
    public bool Administrator;
    public bool IsInRole(string role) { return Administrator && role == "Administrators"; }
}
public class UserController {
    public static UserController Instance = new UserController();
    public UserInfo CurrentUser;
    public UserInfo GetCurrentUserInfo() { return CurrentUser; }
}
public class Subscription {
    public int Id;
    public int PortalId;
    public int CreatedBy;
    public string ReportId;
    public string GroupId;
}
public class SubscriptionViewModel {
    public string ReportId;
    public string GroupId;
}
public class Module { public int PortalID = 1; }
public class SubscriptionsRepository {
    public static SubscriptionsRepository Instance = new SubscriptionsRepository();
    public List<Subscription> Items;
    public List<Subscription> GetSubscriptionsByReportId(string report, int portal) {
        return Items.Where(s => s.ReportId == report && s.PortalId == portal).ToList();
    }
}
public class Checks {
    private Module ActiveModule = new Module();
    private bool ReportPermission = true;
    private string LastPermissionTarget;
    private string WorkspaceOrReport(string group, string report) { return group + ":" + report; }
    private bool UserHasPermission(string target) { LastPermissionTarget = target; return ReportPermission; }
__HELPERS__
    private List<Subscription> List(UserInfo currentUser, string workspaceId, string reportId, int portalId) {
__LIST__
        return subscriptions;
    }
    private static void Check(bool condition, string name) { if (!condition) throw new Exception(name); }
    public static string Run() {
        var checks = new Checks();
        var own = new Subscription { Id = 1, PortalId = 1, CreatedBy = 10, GroupId = "workspace", ReportId = "report" };
        var other = new Subscription { Id = 2, PortalId = 1, CreatedBy = 20, GroupId = "workspace", ReportId = "report" };
        var foreign = new Subscription { Id = 3, PortalId = 2, CreatedBy = 10, GroupId = "workspace", ReportId = "report" };
        var otherWorkspace = new Subscription { Id = 4, PortalId = 1, CreatedBy = 10, GroupId = "other", ReportId = "report" };
        SubscriptionsRepository.Instance.Items = new List<Subscription> { own, other, foreign, otherWorkspace };
        var request = new SubscriptionViewModel { GroupId = "workspace", ReportId = "report" };
        foreach (var user in new[] {
            new UserInfo { UserID = 10 },
            new UserInfo { UserID = 30, Administrator = true },
            new UserInfo { UserID = 40, IsSuperUser = true }
        }) {
            UserController.Instance.CurrentUser = user;
            bool admin = user.Administrator || user.IsSuperUser;
            Check(checks.UserCanManageSubscription(own, request), "Owner/admin/superuser may manage");
            Check(checks.UserCanManageSubscription(other, request) == admin, "Others restricted to admin/superuser");
            Check(checks.List(user, "workspace", "report", 1).Select(s => s.Id)
                .SequenceEqual(admin ? new[] { 1, 2 } : new[] { 1 }), "List must match ownership and workspace scope");
            Check(checks.LastPermissionTarget == "workspace:report", "Use persisted permission target");
            Check(!checks.UserCanManageSubscription(foreign, request), "Cross-portal denied even to superusers");
            Check(!checks.UserCanManageSubscription(null, request), "Missing subscription denied");
            Check(!checks.UserCanManageSubscription(own, new SubscriptionViewModel { GroupId = "other", ReportId = "report" }), "Spoofed workspace denied");
            Check(!checks.UserCanManageSubscription(own, new SubscriptionViewModel { GroupId = "workspace", ReportId = "other" }), "Spoofed report denied");
            checks.ReportPermission = false;
            Check(!checks.UserCanManageSubscription(own, request), "Existing report permissions preserved");
            checks.ReportPermission = true;
        }
        UserController.Instance.CurrentUser = new UserInfo { UserID = -1 };
        own.CreatedBy = -1;
        Check(!checks.UserCanManageSubscription(own, request), "Anonymous users cannot manage");
        UserController.Instance.CurrentUser = null;
        Check(!checks.UserCanManageSubscription(own, request), "Missing user denied");
        return "PASS: ownership, administrator/superuser, portal/workspace/report scope and report permission matrix; list filtering and all mutation guards checked. DNN dependencies are stubbed.";
    }
}
}
'@
Add-Type -TypeDefinition ($harness.Replace('__HELPERS__', $helpers).Replace('__LIST__', $list))
[SubscriptionOwnershipTests.Checks]::Run()
