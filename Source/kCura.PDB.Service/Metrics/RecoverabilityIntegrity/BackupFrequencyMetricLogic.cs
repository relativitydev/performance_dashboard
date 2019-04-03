namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	[MetricType(MetricType.BackupFrequency)]
	public class BackupFrequencyMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;
		private readonly IGapsFrequencyAnalyzer gapsFrequencyAnalyzer;
		private readonly IMetricDataService metricDataService;

		public BackupFrequencyMetricLogic(
			IGapsCollectionVerifier gapsCollectionVerifier,
			IGapsFrequencyAnalyzer gapsFrequencyAnalyzer,
			IMetricDataService metricDataService)
		{
			this.gapsCollectionVerifier = gapsCollectionVerifier;
			this.gapsFrequencyAnalyzer = gapsFrequencyAnalyzer;
			this.metricDataService = metricDataService;
		}

		public Task<decimal> ScoreMetric(MetricData metricData) =>
			this.metricDataService.GetData<FrequencyMetricData>(metricData)
				.Pipe(this.gapsFrequencyAnalyzer.ScoreFrequencyData)
				.Pipe(Task.FromResult);

		public async Task<object> CollectMetricData(MetricData metricData) =>
			await this.gapsFrequencyAnalyzer.CaptureFrequencyData<BackupFullDiffGap>(
				metricData.Metric.Hour,
				metricData.Server,
				GapActivityType.BackupFullAndDiff);

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData);
	}
}
