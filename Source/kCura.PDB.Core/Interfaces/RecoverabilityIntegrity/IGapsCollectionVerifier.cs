namespace kCura.PDB.Core.Interfaces.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IGapsCollectionVerifier
	{
		Task<bool> VerifyGapsCollected(MetricData metricData);

		Task<bool> VerifyGapsCollected(int metricDataId);
	}
}
