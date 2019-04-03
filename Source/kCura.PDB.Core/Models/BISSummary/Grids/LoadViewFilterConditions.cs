namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class LoadViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public string Server;
		public string ServerType;
		public int? OverallScore;
		public int? CPUScore;
		public int? RAMScore;
		public int? MemorySignalScore;
        public int? WaitsScore;
        public int? VirtualLogFilesScore;
        public int? LatencyScore;
		public bool? IsActiveWeeklySample;

		public string FriendlySummaryDayHour
		{
			get
			{
				return SummaryDayHour.HasValue
					? string.Format("{0} {1}", SummaryDayHour.Value.ToShortDateString(), SummaryDayHour.Value.ToShortTimeString())
					: string.Empty;
			}
		}

		public string FriendlyIsActiveWeeklySample
		{
			get
			{
				return IsActiveWeeklySample.HasValue
					? IsActiveWeeklySample.Value
						? "Yes"
						: "No"
					: string.Empty;
			}
		}
	}
}
