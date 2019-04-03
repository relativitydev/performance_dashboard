using System;
using System.Collections.Generic;
using kCura.PDB.Core.Models;

namespace kCura.PDD.Web.Models.BISSummary
{
	public class QualityOfServiceViewModel
	{
		public int OverallScore { get; set; }
		public int WeeklyScore { get; set; }
		public int WorstServerId { get; set; }
		public string WorstServerName { get; set; }
		public int BackupWindow { get; set; }
		public string PartnerName { get; set; }
		public string InstanceName { get; set; }
		public ServerDetailCategory UserExperience { get; set; }
		public ServerDetailCategory SystemLoad { get; set; }
		public MaintenanceCategory Backup { get; set; }
		public AvailabilityCategory Uptime { get; set; }

		public string SampleRange { get; set; }
	}

	public class ServerDetailCategory
	{
		public int QuarterlyScore;
		public int WeeklyScore;
		public IList<ServerScore> Servers;
		public string PageUrl { get; set; }
	}

	public class MaintenanceCategory
	{
		public int Score;
		public int WeeklyScore;

		public decimal BackupCoverageScore;
		public decimal BackupFrequencyScore;
		public int MissedBackups;

		public decimal DbccCoverageScore;
		public decimal DbccFrequencyScore;
		public int MissedIntegrityChecks;

        public decimal RPOScore;
        public int MaxDataLossMinutes;

        public decimal RTOScore;
        public int TimeToRecoverHours;

		public int FailedServers;
		public int FailedDatabases;

		public bool DBCCMonitoringEnabled;

		public string PageUrl { get; set; }
	}

	public class AvailabilityCategory
	{
		public int Score;
		public int WeeklyScore;
		public double UptimePercentage;
		public double WeeklyUptimePercentage;
		public IList<DateToReview> DatesToReview;
		public string PageUrl { get; set; }
	}

	public class ServiceCategory
	{
		public int Score { get; set; }
		public int WeeklyScore { get; set; }
		public string WorstServer { get; set; }
		public IList<DateToReview> DatesToReview { get; set; }
		public IList<CurrentGap> KnownIssues { get; set; }
	}

	public class ServerScore
	{
		public int ArtifactId { get; set; }
		public string Name { get; set; }
		public int QuarterlyScore { get; set; }
		public int WeeklyScore { get; set; }
		public string ServerUrl { get; set; }
	}

	public class DateToReview
	{
		public string Date { get; set; }
		public string Hour { get; set; }
		public double Score { get; set; }
		public decimal HoursDown { get; set; }
	}

	public class CurrentGap
	{
		public int WorkspaceArtifactId { get; set; }
		public string Workspace { get; set; }
		public int Gap { get; set; }
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
		public double PointDeduction { get; set; }
		public bool IsMissingBackup { get; set; }
		public string ActivityType
		{
			get { return IsMissingBackup ? "Backup" : "DBCC"; }
		}
	}
}
