namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;

	public interface IServerRecoverabilityProcessor
	{
		Task ProcessRecoverabilityForServer(int metricDataId);
	}
}
