namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	[MetricType(MetricType.Rto)]
	public class RtoMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;
		private readonly IDatabaseRepository databaseRepository;
		private readonly IMetricDataService metricDataService;
		private readonly IRecoveryObjectivesReporter recoveryObjectivesReporter;
		private readonly IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter;

		public RtoMetricLogic(
			IGapsCollectionVerifier gapsCollectionVerifier,
			IDatabaseRepository databaseRepository,
			IMetricDataService metricDataService,
			IRecoveryObjectivesReporter recoveryObjectivesReporter,
			IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter)
		{
			this.gapsCollectionVerifier = gapsCollectionVerifier;
			this.databaseRepository = databaseRepository;
			this.metricDataService = metricDataService;
			this.recoveryObjectivesReporter = recoveryObjectivesReporter;
			this.recoverabilityIntegritySummaryReporter = recoverabilityIntegritySummaryReporter;
		}

		public Task<decimal> ScoreMetric(MetricData metricData)
		{
			var rtoMetricData = this.metricDataService.GetData<RtoMetricData>(metricData);

			// time to recover in hours
			var timeToRecover = rtoMetricData?.TimeToRecover;
			return ScoreTimeToRecover(timeToRecover).Pipe(Task.FromResult);
		}

		internal static decimal ScoreTimeToRecover(int? timeToRecover)
		{
			if (!timeToRecover.HasValue)
			{
				return FormulaForTimeToRecoverIsNull();
			}

			if (timeToRecover.Value <= 4)
			{
				return FormulaForTimeToRecoverLessThan4();
			}

			if (timeToRecover.Value <= 24)
			{
				return FormulaForTimeToRecoverLessThan24(timeToRecover.Value);
			}

			if (timeToRecover.Value <= 48)
			{
				return FormulaForTimeToRecoverLessThan48(timeToRecover.Value);
			}

			return FormulaForTimeToRecoverMoreThan48();
		}

		internal static decimal FormulaForTimeToRecoverIsNull() => Defaults.Scores.OneHundred;

		internal static decimal FormulaForTimeToRecoverLessThan4() => Defaults.Scores.OneHundred;

		internal static decimal FormulaForTimeToRecoverLessThan24(int timeToRecover) => 100 - (timeToRecover - 4);

		internal static decimal FormulaForTimeToRecoverLessThan48(int timeToRecover) => (decimal)(80 - (3.3333 * (timeToRecover - 24)));

		internal static decimal FormulaForTimeToRecoverMoreThan48() => Defaults.Scores.Zero;

		public async Task<object> CollectMetricData(MetricData metricData)
		{
			// Grab all databases for the server
			var databases = await this.databaseRepository.ReadByServerIdAsync(metricData.Server.ServerId);

			// Calculate all of the EstimatedTimeToRecover and sort
			var databasesWithRto = databases.Select(db => new
			{
				Database = db,
				TimeToRecover = db.LastBackupFullDuration.HasValue || db.LastBackupDiffDuration.HasValue || db.LogBackupsDuration.HasValue
						? (int?)(db.LastBackupFullDuration ?? 0) + (db.LastBackupDiffDuration ?? 0) + (db.LogBackupsDuration ?? 0)
						: null
			})
			.OrderByDescending(db => db.TimeToRecover);

			// Score them for reports
			var databasesScoreList = databasesWithRto.Where(g => g.TimeToRecover.HasValue).Select(g =>
				new DatabaseRtoScoreData
				{
					DatabaseId = g.Database.Id,
					TimeToRecoverHours = (decimal)TimeSpan.FromMinutes(g.TimeToRecover.Value).TotalHours,
					RtoScore = ScoreTimeToRecover(g.TimeToRecover)
				}).ToList();
			await this.recoveryObjectivesReporter.UpdateRtoReport(databasesScoreList);
			var worstRto = databasesScoreList.FirstOrDefault();
			if (worstRto != null)
			{
				await this.recoverabilityIntegritySummaryReporter.UpdateWorstRto(
					metricData.Metric.Hour.Id,
					worstRto.DatabaseId,
					worstRto.TimeToRecoverHours);
			}

			// Return Metric Data
			var timeToRecoverInMinutes = databasesWithRto.First().TimeToRecover;
			var timeToRecoverInHours = timeToRecoverInMinutes.HasValue
				? (int?)TimeSpan.FromMinutes(timeToRecoverInMinutes.Value).TotalHours
				: null;

			return new RtoMetricData { TimeToRecover = timeToRecoverInHours };
		}

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData);

		internal class RtoMetricData
		{
			public int? TimeToRecover { get; set; }
		}
	}
}
