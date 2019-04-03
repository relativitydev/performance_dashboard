namespace kCura.PDB.Core.Interfaces.DataProviders
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IBackupProvider
	{
		Task<IList<Backup>> GetBackupsAsync(Hour hour, Server server, IList<Database> databases);

		Task<IList<Backup>> GetLastBackupsBeforeHourAsync(Hour hour, Server server, IList<Database> databases, IList<BackupType> backupTypes);
	}
}
