namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class EnvironmentCheckDatabaseFilterConditions
	{
		public String ServerName = null;
		public String SQLVersion = null;
		public String AdHocWorkload = null;
		public String MaxServerMemory = null;
		public String MaxDegreeOfParallelism = null;
		public String tempDBDataFiles = null;
		public String LastSqlRestart = null;
	}
}