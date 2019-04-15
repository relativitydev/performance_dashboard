namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class MissedBackupIntegrityInfo
	{
		public int MissedBackups;
		public int MissedIntegrityChecks;
		public int FailedServers;
		public int FailedDatabases;
		public int BackupFrequencyScore;
		public int BackupCoverageScore;
		public int DbccFrequencyScore;
		public int DbccCoverageScore;
		public int MaxDataLossMinutes;
		public int RPOScore;
		public int TimeToRecoverHours;
		public int RTOScore;
	}
}
