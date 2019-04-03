namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class SearchViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public int CaseArtifactId;
		public string Search;
		public string User;
		public Int64? TotalRunTime;
		public int? AverageRunTime;
		public int? NumberOfRuns;
		public int? PercentLongRunning;
		public Int64? QoSHourID;
		public bool? IsComplex;
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

		public string FriendlyIsComplex
		{
			get
			{
				return IsComplex.HasValue
					? IsComplex.Value
						? "Complex"
						: "Simple"
					: string.Empty;
			}
		}
	}
}
