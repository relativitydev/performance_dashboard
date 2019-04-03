namespace kCura.PDB.Service.CategoryScoring
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[CategoryType(CategoryType.RecoverabilityIntegrity)]
	public class RecoverabilityIntegrityScoringLogic : ICategoryScoringLogic
	{
		private readonly IMetricDataRepository metricDataRepository;
		private readonly IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter;

		public RecoverabilityIntegrityScoringLogic(
			IMetricDataRepository metricDataRepository,
			IRecoverabilityIntegritySummaryReporter recoverabilityIntegritySummaryReporter)
		{
			this.metricDataRepository = metricDataRepository;
			this.recoverabilityIntegritySummaryReporter = recoverabilityIntegritySummaryReporter;
		}

		public async Task<decimal> ScoreMetrics(CategoryScore categoryScore, IList<MetricData> metricDatas)
		{
			var hour = metricDatas.First().Metric.Hour;
			var riScores = await this.GetRecoverabilityIntegrityScores(hour, categoryScore.Server);

			// Report
			await this.recoverabilityIntegritySummaryReporter.CreateRecoverabilityIntegritySummaryReport(
				hour.Id,
				riScores.Average,
				riScores.Rpo,
				riScores.Rto,
				riScores.BackupFrequency,
				riScores.BackupCoverage,
				riScores.DbccFrequency,
				riScores.DbccCoverage);

			return riScores.Average;
		}

		internal async Task<RecoverabilityIntegrityScores> GetRecoverabilityIntegrityScores(Hour hour, Server server)
		{
			var startTime = hour.HourTimeStamp.AddDays(Defaults.BackfillDays);
			var endTime = hour.HourTimeStamp.AddHours(1);

			// RPO
			var worstRpo = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rpo);

			// RTO
			var worstRto = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rto);

			// Backup Frequency
			var worstBackupFrequency = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupFrequency);

			// Backup Coverage
			var worstBackupCoverage = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupCoverage);

			// Dbcc Frequency
			var worstDbccFrequency = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccFrequency);

			// Dbcc Coverage
			var worstDbccCoverage = this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccCoverage);

			return new RecoverabilityIntegrityScores
			{
				Rpo = (await worstRpo)?.Score ?? Defaults.Scores.OneHundred,
				Rto = (await worstRto)?.Score ?? Defaults.Scores.OneHundred,
				BackupFrequency = (await worstBackupFrequency)?.Score ?? Defaults.Scores.OneHundred,
				BackupCoverage = (await worstBackupCoverage)?.Score ?? Defaults.Scores.OneHundred,
				DbccFrequency = (await worstDbccFrequency)?.Score ?? Defaults.Scores.OneHundred,
				DbccCoverage = (await worstDbccCoverage)?.Score ?? Defaults.Scores.OneHundred,
			};
		}

		internal class RecoverabilityIntegrityScores
		{
			public decimal Rpo { get; set; }
			public decimal Rto { get; set; }
			public decimal BackupFrequency { get; set; }
			public decimal BackupCoverage { get; set; }
			public decimal DbccFrequency { get; set; }
			public decimal DbccCoverage { get; set; }
			public decimal Average =>
				new[] { Rpo, Rto, BackupFrequency, BackupCoverage, DbccFrequency, DbccCoverage }.Average();
		}
	}
}
