namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class BackupDBCCMonitoringResults
	{
		public int FailedServers;
		public int FailedDatabases;
		public string ServerErrors;
		public string DatabaseErrors;

		public BackupDBCCMonitoringResults()
		{
			ServerErrors = string.Empty;
			DatabaseErrors = string.Empty;
		}
	}
}
