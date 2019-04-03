namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class UptimeViewFilterConditions
	{
		public DateTime? SummaryDayHour;
		public int? Score;
		public string Status;
		public double? Uptime;

		public string FriendlySummaryDayHour
		{
			get
			{
				return SummaryDayHour.HasValue
					? string.Format("{0} {1}", SummaryDayHour.Value.ToShortDateString(), SummaryDayHour.Value.ToShortTimeString())
					: string.Empty;
			}
		}

		public string FriendlyUptime
		{
			get
			{
				return Uptime.GetValueOrDefault(100).ToString("N2");
			}
		}
	}
}
