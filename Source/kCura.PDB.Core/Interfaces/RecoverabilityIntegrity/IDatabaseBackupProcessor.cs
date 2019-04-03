namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IDatabaseBackupProcessor
	{
		IList<TGap> CreateGaps<TGap>(Database database, IList<Backup> backups, Backup lastBackupBeforeHour)
			where TGap : Gap, new();

		Task UpdateLatestDatabaseBackups(Database database, IList<Backup> backups);
	}
}
