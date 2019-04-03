namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IServerBackupProcessor
	{
		Task ProcessBackupsForServer(Hour hour, Server server);
	}
}
