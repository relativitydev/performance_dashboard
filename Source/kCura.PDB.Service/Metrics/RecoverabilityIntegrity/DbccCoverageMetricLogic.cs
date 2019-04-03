namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	[MetricType(MetricType.DbccCoverage)]
	public class DbccCoverageMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;
		private readonly IGapsCoverageAnalyzer gapCoverageAnalyzer;
		private readonly IMetricDataService metricDataService;

		public DbccCoverageMetricLogic(
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
			await this.gapCoverageAnalyzer.CaptureCoverageData<DbccGap>(
				metricData.Metric.Hour,
				metricData.Server,
				GapActivityType.Dbcc);

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData);
	}
}
