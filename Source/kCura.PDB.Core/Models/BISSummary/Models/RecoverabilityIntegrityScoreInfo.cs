namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class RecoverabilityIntegrityScoreInfo
	{
		public int Index;
		public DateTime SummaryDayHour;
		public string FriendlySummaryDayHour => $"{SummaryDayHour.ToShortDateString()} {SummaryDayHour:t}";
		public int RecoverabilityIntegrityScore;
		public int BackupFrequencyScore;
		public int BackupCoverageScore;
		public int DbccFrequencyScore;
		public int DbccCoverageScore;
		public int RPOScore;
		public int RTOScore;
		public string WorstRPODatabase;
		public string WorstRTODatabase;
		public int PotentialDataLossMinutes;
		public int EstimatedTimeToRecoverHours;
	}
}