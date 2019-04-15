namespace kCura.PDB.Core.Interfaces.Metrics
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricReadyForDataCollectionLogic
	{
		Task<bool> IsReady(MetricData metricData);
	}
}
