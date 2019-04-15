namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class HoursViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public int Server;
		public string Workspace;
		public string Search;
		public Int64? TotalRunTime;
		public int? AverageRunTime;
		public int? NumberOfRuns;
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
