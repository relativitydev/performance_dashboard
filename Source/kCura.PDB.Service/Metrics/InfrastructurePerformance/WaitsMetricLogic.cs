namespace kCura.PDB.Service.Metrics.InfrastructurePerformance
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	[MetricType(MetricType.SqlServerWaits)]
	public class WaitsMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IProcessControlRepository processControlRepository;

		public WaitsMetricLogic(IProcessControlRepository processControlRepository)
		{
			this.processControlRepository = processControlRepository;
		}

		// Stub
		public Task<decimal> ScoreMetric(MetricData metricData)
		{
			return Task.FromResult(Defaults.Scores.OneHundred);
		}

		// Stub
		public Task<object> CollectMetricData(MetricData metricData)
		{
			return Task.FromResult(new object());
		}

		public async Task<bool> IsReady(MetricData metricData)
		{
			// Get threshold (ran after the hour completed)
			var timeThreshold = metricData.Metric.Hour.GetHourEnd();

			// Get process controls stats
			return await this.processControlRepository.HasRunSuccessfully(ProcessControlId.CollectWaitStatistics, timeThreshold);
		}
	}
}
