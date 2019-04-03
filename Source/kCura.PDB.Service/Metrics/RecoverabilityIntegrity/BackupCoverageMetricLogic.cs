namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	[MetricType(MetricType.BackupCoverage)]
	public class BackupCoverageMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;
		private readonly IGapsCoverageAnalyzer gapCoverageAnalyzer;
		private readonly IMetricDataService metricDataService;

		public BackupCoverageMetricLogic(
			IGapsCollectionVerifier gapsCollectionVerifier,
			IGapsCoverageAnalyzer gapCoverageAnalyzer,
			IMetricDataService metricDataService)
		{
			this.gapsCollectionVerifier = gapsCollectionVerifier;
			this.gapCoverageAnalyzer = gapCoverageAnalyzer;
			this.metricDataService = metricDataService;
		}

		public Task<decimal> ScoreMetric(MetricData metricData) =>
			this.metricDataService.GetData<CoverageMetricData>(metricData)
				.Pipe(this.gapCoverageAnalyzer.ScoreCoverageData)
				.Pipe(Task.FromResult);

		public async Task<object> CollectMetricData(MetricData metricData) =>
			await this.gapCoverageAnalyzer.CaptureCoverageData<BackupFullDiffGap>(
				metricData.Metric.Hour,
				metricData.Server,
				GapActivityType.BackupFullAndDiff);

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData);
	}
}
