namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class EnvironmentCheckDatabaseDetails
	{
		public String ServerName { get; set; }
		public String SQLVersion { get; set; }
		public Int32 AdHocWorkLoad { get; set; }
		public Double MaxServerMemory { get; set; }
		public Int32 MaxDegreeOfParallelism { get; set; }
		public Int32 TempDBDataFiles { get; set; }
		public DateTime LastSQLRestart { get; set; }
	}
}
