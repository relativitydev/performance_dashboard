namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class UserExperienceSearchUserInfo
	{
		public int Index;
		public int CaseArtifactId;
		public long AuditId;
		public int SearchArtifactId;
		public string Search;
		public int UserArtifactId;
		public string User;
		public Int64 TotalRunTime;
		public int AverageRunTime;
		public int TotalRuns;
		public int PercentLongRunning;
		public bool IsComplex;
		public DateTime SummaryDayHour;
		public Int64 QoSHourID;
		public bool IsActiveWeeklySample;
	}
}
