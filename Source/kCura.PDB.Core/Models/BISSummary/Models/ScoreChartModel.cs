namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class ScoreChartModel
	{
		public int ServerArtifactId;
		public string ServerName;
		public DateTime SummaryDayHour;
		public decimal? UserExperienceScore;
		public decimal? SystemLoadScore;
		public decimal? BackupDbccScore;
        public decimal? UptimeScore;
		public bool IsSample;
	}
}
