namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class UptimeReportHourInfo
	{
		public int Index;
		public DateTime SummaryDayHour;
		public int Score;
		public string Status;
		public double Uptime;
        public bool AffectedByMaintenanceWindow;
    }
}