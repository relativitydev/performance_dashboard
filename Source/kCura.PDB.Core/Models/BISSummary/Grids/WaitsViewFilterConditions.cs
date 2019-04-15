namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class WaitsViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public string Server;
		public int? OverallScore;
		public int? SignalWaitsRatio;
        public string WaitType;
		public long? SignalWaitTime;
		public long? TotalWaitTime;
        public bool? IsPoisonWait;
		public bool? IsActiveWeeklySample;
        public decimal? PercentOfCPUThreshold;
        public decimal? DifferentialWaitingTasksCount;

		public string FriendlySummaryDayHour
		{
			get
			{
				return SummaryDayHour.HasValue
					? string.Format("{0} {1}", SummaryDayHour.Value.ToShortDateString(), SummaryDayHour.Value.ToShortTimeString())
					: string.Empty;
			}
		}

        public string FriendlyIsPoisonWait
        {
            get
            {
                return IsPoisonWait.HasValue
                    ? IsPoisonWait.Value
                        ? "Yes"
                        : "No"
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
