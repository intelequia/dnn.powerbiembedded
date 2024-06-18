using Microsoft.PowerBI.Api.Models;

namespace DotNetNuke.PowerBI.Models
{
    public class ReportWithWorkspace: Report
    {
        public ReportWithWorkspace() { }
        public Workspace Workspace { get; set; }
    }
}