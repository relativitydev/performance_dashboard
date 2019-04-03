namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System.Collections.Generic;

	public class BackupDBCCNearingViolation
	{
		public IList<DatabaseNearingViolation> Backups;
		public IList<DatabaseNearingViolation> DBCC;
	}
}