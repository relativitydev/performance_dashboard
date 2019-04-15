namespace kCura.PDB.Core.Interfaces.Metrics
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricLogic
	{
		Task<decimal> ScoreMetric(MetricData metricData);

		Task<object> CollectMetricData(MetricData metricData);
	}
}
