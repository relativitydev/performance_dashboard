namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class ServerViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public string Server;
		public string Workspace;
		public int? TotalUsers;
		public int? TotalLongRunning;
		public int? TotalSearchAudits;
		public int? TotalNonSearchAudits;
		public Int64? TotalExecutionTime;
		public int? TotalAudits;
		public int? Score;
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
