namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	[MetricType(MetricType.BackupGaps)]
	public class BackupGapMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;

		public BackupGapMetricLogic(IGapsCollectionVerifier gapsCollectionVerifier)
		{
			this.gapsCollectionVerifier = gapsCollectionVerifier;
		}

		public Task<decimal> ScoreMetric(MetricData metricData)
		{
			return Task.FromResult(Defaults.Scores.OneHundred);
		}

		public Task<object> CollectMetricData(MetricData metricData)
		{
			return Task.FromResult(new object());
		}

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData.Id);
	}
}
