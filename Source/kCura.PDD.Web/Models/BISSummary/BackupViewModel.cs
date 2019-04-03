using System;
using System.Collections.Generic;

namespace kCura.PDD.Web.Models.BISSummary
{
    public class BackupViewModel
    {
        public IList<WorkspaceIssues> Workspaces { get; set; }
        public IList<ServerIssues> Servers { get; set; } 

        public BackupViewModel()
        {
            Workspaces = new List<WorkspaceIssues>();
            Servers = new List<ServerIssues>();
        }
    }

    public class ServerIssues
    {
        public string ServerName { get; set; }
        public int UnremediedGaps { get; set; }
        public double TotalPointDeduction { get; set; }
    }

    public class WorkspaceIssues
    {
        public WorkspaceInfo Info { get; set; }

        public IList<Incident> Incidents { get; set; }
 
        public WorkspaceIssues()
        {
            Info = new WorkspaceInfo();
            Incidents = new List<Incident>();
        }
    }

    public class WorkspaceInfo
    {
        public int ArtifactId { get; set; }
        public string Name { get; set; }
    }

    public class Incident
    {
        public string WorkspaceName { get; set; }
        public string ServerName { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime? DateRemedied { get; set; }
        public string ResolutionStatus
        {
            get
            {
                return PointDeduction > 0
                    ? DateRemedied.HasValue
                        ? String.Format("Resolved {0}", DateRemedied.Value.ToShortDateString())
                        : "Unresolved"
                    : "Compliant";
            }
        }
        public int MissedDays { get; set; }
        public bool IsMissingBackup { get; set; }
        public string ActivityType
        {
            get { return IsMissingBackup ? "Backup" : "DBCC"; }
        }
        public double PointDeduction { get; set; }
    }
}