namespace kCura.PDB.Core.Constants
{
	using System;

	public static class DatabaseConstants
	{
		public const string TestConnectionString = @"Data Source=SomeServer;Integrated Security=SSPI";

		public const int GlassRunLogDeleteThresholdDays = -14; // 2 weeks in the past
		public const int PastHalfYearThreshold = -180; // Half a year of days
		public const int PastQuarterThreshold = -90; // Quarter (3 months) worth of days
		public const int PastWeekThreshold = -7;
		
		public const string PrimarySqlServerType = "SQL - Primary";
		public const string GoStatement = "GO";

		public static readonly TimeSpan DeploymentRetrySleepTimeoutSeconds = TimeSpan.FromSeconds(30);
	}
}
