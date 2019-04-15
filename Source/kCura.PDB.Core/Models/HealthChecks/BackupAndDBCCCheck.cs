namespace kCura.PDB.Core.Models.HealthChecks
{
	using System;

	public class BackupAndDBCCCheck
	{
		public string ServerName { get; set; }

		public string DBName { get; set; }

		public DateTime? LastCleanDBCCDate { get; set; }

		public Int32 DBCCStatus { get; set; }

		public DateTime? LastBackupDate { get; set; }

		public Int32 BackupStatus { get; set; }
	}
}
