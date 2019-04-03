namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class RecoverabilityIntegrityViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public int? RecoverabilityIntegrityScore;
		public int? BackupFrequencyScore;
		public int? BackupCoverageScore;
		public int? DbccFrequencyScore;
		public int? DbccCoverageScore;
		public int? RPOScore;
		public int? RTOScore;

		public string FriendlySummaryDayHour
		{
			get
			{
				return SummaryDayHour.HasValue
					? string.Format("{0} {1}", SummaryDayHour.Value.ToShortDateString(), SummaryDayHour.Value.ToShortTimeString())
					: string.Empty;
			}
		}
	}
}
