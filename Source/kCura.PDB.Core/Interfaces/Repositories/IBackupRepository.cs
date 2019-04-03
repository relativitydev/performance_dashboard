namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IBackupRepository : IBackupProvider
	{
		Task RunBackupAsync(BackupType backupType, string backupPath);
	}
}
