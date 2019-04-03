namespace kCura.PDB.Service.Tests.Logic.CategoryScoring
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RecoverabilityIntegrityScoringLogicTests
	{
		private RecoverabilityIntegrityScoringLogic recoverabilityIntegrityScoringLogic;
		private Mock<IMetricDataRepository> metricDataRepository;
		private Mock<IRecoverabilityIntegritySummaryReporter> recoverabilityIntegritySummaryReporter;

		[SetUp]
		public void Setup()
		{
			this.metricDataRepository = new Mock<IMetricDataRepository>();
			this.recoverabilityIntegritySummaryReporter = new Mock<IRecoverabilityIntegritySummaryReporter>();
			this.recoverabilityIntegrityScoringLogic = new RecoverabilityIntegrityScoringLogic(
				this.metricDataRepository.Object,
				this.recoverabilityIntegritySummaryReporter.Object);
		}

		[Test]
		public async Task RecoverabilityIntegrityScoringLogic_ScoreMetrics()
		{
			// Arrange
			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var startTime = hour.HourTimeStamp.AddDays(Defaults.BackfillDays);
			var endTime = hour.HourTimeStamp.AddHours(1);
			var server = new Server();
			var metricData = new MetricData { Metric = new Metric { Hour = hour } };
			var categoryScore = new CategoryScore { Server = server };

			var rpoScore = 12;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rpo))
				.ReturnsAsync(new MetricData { Score = rpoScore });

			var rtoScore = 23;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rto))
				.ReturnsAsync(new MetricData { Score = rtoScore });

			var backupFreqScore = 34;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupFrequency))
				.ReturnsAsync(new MetricData { Score = backupFreqScore });

			var backupCovScore = 45;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupCoverage))
				.ReturnsAsync(new MetricData { Score = backupCovScore });

			var dbccFreqScore = 56;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccFrequency))
				.ReturnsAsync(new MetricData { Score = dbccFreqScore });

			var dbccCovScore = 67;
			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccCoverage))
				.ReturnsAsync(new MetricData { Score = dbccCovScore });

			var avgScore = (rpoScore + rtoScore + backupCovScore + backupFreqScore + dbccCovScore + dbccFreqScore) / 6m;

			this.recoverabilityIntegritySummaryReporter.Setup(
				m => m.CreateRecoverabilityIntegritySummaryReport(
					hour.Id,
					avgScore,
					rpoScore,
					rtoScore,
					backupFreqScore,
					backupCovScore,
					dbccFreqScore,
					dbccCovScore)).ReturnsAsyncDefault();

			// Act
			var result = await this.recoverabilityIntegrityScoringLogic.ScoreMetrics(categoryScore, new[] { metricData });

			// Assert
			Assert.That(result, Is.EqualTo(39.5));
			this.recoverabilityIntegritySummaryReporter.VerifyAll();
		}

		[Test]
		public async Task RecoverabilityIntegrityScoringLogic_GetRecoverabilityIntegrityScores()
		{
			// Arrange
			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var startTime = hour.HourTimeStamp.AddDays(Defaults.BackfillDays);
			var endTime = hour.HourTimeStamp.AddHours(1);
			var server = new Server();

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rpo))
				.ReturnsAsync(new MetricData { Score = 12 });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rto))
				.ReturnsAsync(new MetricData { Score = 23 });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupFrequency))
				.ReturnsAsync(new MetricData { Score = 34 });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupCoverage))
				.ReturnsAsync(new MetricData { Score = 45 });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccFrequency))
				.ReturnsAsync(new MetricData { Score = 56 });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccCoverage))
				.ReturnsAsync(new MetricData { Score = 67 });

			// Act
			var result = await this.recoverabilityIntegrityScoringLogic.GetRecoverabilityIntegrityScores(hour, server);

			// Assert
			Assert.That(result.Rpo, Is.EqualTo(12));
			Assert.That(result.Rto, Is.EqualTo(23));
			Assert.That(result.BackupFrequency, Is.EqualTo(34));
			Assert.That(result.BackupCoverage, Is.EqualTo(45));
			Assert.That(result.DbccFrequency, Is.EqualTo(56));
			Assert.That(result.DbccCoverage, Is.EqualTo(67));
			Assert.That(result.Average, Is.EqualTo(39.5));
		}

		[Test]
		public async Task RecoverabilityIntegrityScoringLogic_GetRecoverabilityIntegrityScores_Null()
		{
			// Arrange
			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var startTime = hour.HourTimeStamp.AddDays(Defaults.BackfillDays);
			var endTime = hour.HourTimeStamp.AddHours(1);
			var server = new Server();

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rpo))
				.ReturnsAsync(new MetricData { Score = null });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.Rto))
				.ReturnsAsync(new MetricData { Score = null });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupFrequency))
				.ReturnsAsync(new MetricData { Score = null });

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.BackupCoverage))
				.ReturnsAsync((MetricData)null);

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccFrequency))
				.ReturnsAsync((MetricData)null);

			this.metricDataRepository.Setup(r => r.ReadWorstScoreInDateRangeAsync(startTime, endTime, server, MetricType.DbccCoverage))
				.ReturnsAsync((MetricData)null);

			// Act
			var result = await this.recoverabilityIntegrityScoringLogic.GetRecoverabilityIntegrityScores(hour, server);

			// Assert
			Assert.That(result.Rpo, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.Rto, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.BackupFrequency, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.BackupCoverage, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.DbccFrequency, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.DbccCoverage, Is.EqualTo(Defaults.Scores.OneHundred));
			Assert.That(result.Average, Is.EqualTo(Defaults.Scores.OneHundred));
		}
	}
}
