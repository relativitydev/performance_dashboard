namespace kCura.PDB.Service.Metrics.RecoverabilityIntegrity
{
	using System;
	using System.Collections.Generic;
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
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	[MetricType(MetricType.Rpo)]
	public class RpoMetricLogic : IMetricLogic, IMetricReadyForDataCollectionLogic
	{
		private readonly IGapsCollectionVerifier gapsCollectionVerifier;
		private readonly IDatabaseGapsRepository databaseGapsRepository;
		private readonly IMetricDataService metricDataService;
		private readonly IDatabaseRepository databaseRepository;

		private readonly IRecoveryObjectivesReporter recoveryObjectivesReporter;
		private readonly IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter;

		public RpoMetricLogic(
			IGapsCollectionVerifier gapsCollectionVerifier,
			IDatabaseGapsRepository databaseGapsRepository,
			IMetricDataService metricDataService,
			IDatabaseRepository databaseRepository,
			IRecoveryObjectivesReporter recoveryObjectivesReporter,
			IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter)
		{
			this.gapsCollectionVerifier = gapsCollectionVerifier;
			this.databaseGapsRepository = databaseGapsRepository;
			this.metricDataService = metricDataService;
			this.databaseRepository = databaseRepository;
			this.recoveryObjectivesReporter = recoveryObjectivesReporter;
			this.recoverabilityIntegritySummaryReporter = recoverabilityIntegritySummaryReporter;
		}

		public Task<decimal> ScoreMetric(MetricData metricData)
		{
			var rpoMetricData = this.metricDataService.GetData<RpoMetricData>(metricData);
			return Task.FromResult(ScorePotentialDataLoss(rpoMetricData?.PotentialDataLoss));
		}

		internal static decimal ScorePotentialDataLoss(int? potentialDataLoss)
		{
			if (potentialDataLoss.HasValue == false)
			{
				return Defaults.Scores.OneHundred;
			}

			var potentialDataLossMinutes = TimeSpan.FromSeconds(potentialDataLoss.Value).TotalMinutes;

			if (potentialDataLossMinutes <= 15)
			{
				return FormulaForLessThan15Minutes();
			}

			if (potentialDataLossMinutes <= 240)
			{
				return FormulaForLessThan240Minutes(potentialDataLossMinutes);
			}

			if (potentialDataLossMinutes < 1440)
			{

				return FormulaForBetween240And1440Minutes(potentialDataLossMinutes);
			}

			return FormulaForMoreThan1440Minutes();
		}

		internal static decimal FormulaForLessThan15Minutes() => Defaults.Scores.OneHundred;
		internal static decimal FormulaForLessThan240Minutes(double potentialDataLossMinutes) => (decimal)(100 - (0.0888 * (potentialDataLossMinutes - 15)));
		internal static decimal FormulaForBetween240And1440Minutes(double potentialDataLossMinutes) => (decimal)(80 - (0.0666 * (potentialDataLossMinutes - 240)));
		internal static decimal FormulaForMoreThan1440Minutes() => Defaults.Scores.Zero;

		// For all databases, grab the RPO data relevant for the hour and return it
		public async Task<object> CollectMetricData(MetricData metricData)
		{
			// Get the largest gap in the gap table for each database
			var largestBackupGapsInHour = await this.databaseGapsRepository.ReadLargestGapsForEachDatabaseAsync<BackupAllGap>(
												metricData.Server,
												metricData.Metric.Hour,
												GapActivityType.Backup);

			// grab all the unresolved gaps
			var unresolvedGaps = await this.GetUnresolvedGaps(metricData.Metric.Hour, metricData.Server);

			// Join the list and take the largest gap values
			var mergedList = largestBackupGapsInHour.Concat(unresolvedGaps).GroupBy(g => g.DatabaseId)
				.Select(x => x.OrderByDescending(g => g.Duration).First());

			// Score the list for the reports (not great, but best solution at the moment)
			var mergedScoredList = mergedList.Where(g => g.Duration.HasValue).Select(
				g => new DatabaseRpoScoreData
				{
					DatabaseId = g.DatabaseId,
					PotentialDataLoss = g.Duration,
					RpoScore = ScorePotentialDataLoss(g.Duration)
				}).ToList();

			var worstRpo = mergedScoredList.FirstOrDefault();

			// Report the scored list
			await this.recoveryObjectivesReporter.UpdateRpoReport(mergedScoredList); // This doesn't care about hour order, so it will be whatever the last reporter ran was. O_O

			// Report the worst database
			if (worstRpo != null)
			{
				await this.recoverabilityIntegritySummaryReporter.UpdateWorstRpo(metricData.Metric.Hour.Id, worstRpo.DatabaseId, worstRpo.PotentialDataLoss.Value);

				return new RpoMetricData { PotentialDataLoss = worstRpo.PotentialDataLoss };
			}

			return null;
		}

		internal async Task<int?> LargestUnresolvedGapForServer(Hour hour, Server server)
		{
			var unresolvedGaps = await this.GetUnresolvedGaps(hour, server);

			//var databases = await this.databaseRepository.ReadByServerIdAsync(server.ServerId);
			var largestUnresolvedGap = unresolvedGaps.OrderByDescending(g => g.Duration).FirstOrDefault();

			return largestUnresolvedGap?.Duration;
		}

		internal async Task<IList<Gap>> GetUnresolvedGaps(Hour hour, Server server)
		{
			var databases = await this.databaseRepository.ReadByServerIdAsync(server.ServerId);
			var unresolvedGaps = databases.Where(db => db.MostRecentBackupAnyType.HasValue).Select(
				db => new Gap
				{
					DatabaseId = db.Id,
					ActivityType = GapActivityType.Backup,
					Start = db.MostRecentBackupAnyType.Value,
					Duration = (int)(hour.GetNextHour() - db.MostRecentBackupAnyType.Value).TotalSeconds
				}).ToList();
			return unresolvedGaps;
		}

		public Task<bool> IsReady(MetricData metricData) =>
			this.gapsCollectionVerifier.VerifyGapsCollected(metricData);

		internal class RpoMetricData
		{
			public int? PotentialDataLoss { get; set; }
		}
	}
}
