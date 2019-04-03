namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class QualityOfServiceModel
	{
		public int ServerArtifactId;
		public string ServerName;

		public DateTime SummaryDayHour;
		public bool IsSample;

		public decimal? OverallScore;
		public decimal? WeeklyScore;
		public decimal? UserExperienceScore;
		public decimal? WeeklyUserExperienceScore;
		public decimal? SystemLoadScore;
		public decimal? WeeklySystemLoadScore;
		public decimal? IntegrityScore;
        public decimal? WeeklyIntegrityScore;
		public decimal? UptimeScore;
		public decimal? WeeklyUptimeScore;
	}
}
