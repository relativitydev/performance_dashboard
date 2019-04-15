namespace kCura.PDB.Service.Metrics.UserExperience
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	[MetricType(MetricType.AuditAnalysis)]
	public class AuditAnalysisMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		public AuditAnalysisMetricLogic(
			ISearchAuditBatchRepository searchAuditBatchRepository,
			IAuditAnalysisRepository auditAnalysisRepository,
			IPoisonWaitRepository poisonWaitRepository,
			ILogger logger)
		{
			this.searchAuditBatchRepository = searchAuditBatchRepository;
			this.auditAnalysisRepository = auditAnalysisRepository;
			this.poisonWaitRepository = poisonWaitRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Metric);
		}

		private readonly ISearchAuditBatchRepository searchAuditBatchRepository;
		private readonly IAuditAnalysisRepository auditAnalysisRepository;
		private readonly IPoisonWaitRepository poisonWaitRepository;
		private readonly ILogger logger;

		public async Task<decimal> ScoreMetric(MetricData metricData)
		{
			var auditAnalyses = await this.auditAnalysisRepository.ReadByMetricData(metricData);
			if (auditAnalyses == null || auditAnalyses.Any() == false)
			{
				await this.logger.LogVerboseAsync($"Audit Analysis for metric data: {metricData.Id} was not found.");
				return Defaults.Scores.UserExperience;
			}

			var averageUserScore = 100.0m * auditAnalyses.Select(ScoreUserAuditAnalysis).Average();
			return averageUserScore;
		}

		public async Task<object> CollectMetricData(MetricData metricData)
		{
			await this.logger.LogVerboseAsync($"Collecting metric data for metric data id {metricData.Id}");

			// Read workspace audit batches
			var searchAuditBatches = this.searchAuditBatchRepository.ReadByHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value);

			// NOTE: This only returns the audits from the batch sets (DocumentQuery/Search Audits)
			var auditAnalyses =
				searchAuditBatches
				.SelectMany(b => b.BatchResults)
				.GroupBy(r => r.UserId)
				.Select(rg => new AuditAnalysis
				{
					MetricDataId = metricData.Id,
					UserId = rg.Key,
					TotalComplexQueries = rg.Sum(r => r.TotalComplexQueries),
					TotalLongRunningQueries = rg.Sum(r => r.TotalLongRunningQueries),
					TotalSimpleLongRunningQueries = rg.Sum(r => r.TotalSimpleLongRunningQueries),
					TotalQueries = rg.Sum(r => r.TotalQueries),
					TotalExecutionTime = rg.Sum(r => r.TotalExecutionTime)
				})
				.ToList();

			await this.auditAnalysisRepository.CreateAsync(auditAnalyses);

			await this.logger.LogVerboseAsync($"Created metrics for metric data id {metricData.Id}.  Number batches created: {auditAnalyses.Count} ");

			return new object();
		}

		public async Task<bool> IsReady(MetricData metricData)
		{
			var batchesAreReady = this.searchAuditBatchRepository.ExistsForHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value);
			var poisonWaitsAreCollected = this.poisonWaitRepository.ReadIfPoisonWaitsForHourAsync(metricData.Metric.Hour);
			await Task.WhenAll(batchesAreReady, poisonWaitsAreCollected);
			return await batchesAreReady && await poisonWaitsAreCollected;
		}

		internal static decimal ScoreUserAuditAnalysis(AuditAnalysis auditAnalysis)
		{
			decimal totalSimpleQueries = auditAnalysis.TotalQueries - auditAnalysis.TotalComplexQueries;
			return totalSimpleQueries <= 0
				? 1.0m
				: Math.Min(1.0m, (totalSimpleQueries - auditAnalysis.TotalSimpleLongRunningQueries) / totalSimpleQueries);
		}
	}
}
