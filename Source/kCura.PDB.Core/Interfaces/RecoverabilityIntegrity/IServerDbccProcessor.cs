namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IServerDbccProcessor
	{
		Task ProcessDbccsForServer(Hour hour, Server server);
	}
}
