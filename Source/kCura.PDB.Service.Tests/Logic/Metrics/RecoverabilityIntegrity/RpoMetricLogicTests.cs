namespace kCura.PDB.Service.Tests.Logic.Metrics.RecoverabilityIntegrity
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.Metrics.RecoverabilityIntegrity;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RpoMetricLogicTests
	{
		private RpoMetricLogic rpoMetricLogic;
		private Mock<IGapsCollectionVerifier> gapsCollectionVerifier;
		private Mock<IDatabaseGapsRepository> databaseGapsRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<IDatabaseRepository> databaseRepository;

		private Mock<IRecoveryObjectivesReporter> recoveryObjectivesReporterMock;
		private Mock<IRecoverabilityIntegritySummaryReporter> recoverabilityIntegritySummaryReporterMock;

		[SetUp]
		public void Setup()
		{
			this.gapsCollectionVerifier = new Mock<IGapsCollectionVerifier>();
			this.databaseGapsRepository = new Mock<IDatabaseGapsRepository>();
			this.metricDataService = new Mock<IMetricDataService>();
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.recoveryObjectivesReporterMock = new Mock<IRecoveryObjectivesReporter>();
			this.recoverabilityIntegritySummaryReporterMock = new Mock<IRecoverabilityIntegritySummaryReporter>();
			this.rpoMetricLogic = new RpoMetricLogic(
				this.gapsCollectionVerifier.Object,
				this.databaseGapsRepository.Object,
				this.metricDataService.Object,
				this.databaseRepository.Object,
				this.recoveryObjectivesReporterMock.Object,
				this.recoverabilityIntegritySummaryReporterMock.Object);
		}

		[Test]
		public async Task RpoMetricLogic_CollectMetricData_NotNull()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 1, 1) };
			var backupGap = new BackupAllGap { Duration = 120 };
			var backupGapList = new[] { backupGap };
			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForEachDatabaseAsync<BackupAllGap>(server, hour, GapActivityType.Backup))
				.ReturnsAsync(backupGapList);

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(new[] { new Database { }, });

			// Act
			var result = await this.rpoMetricLogic.CollectMetricData(new MetricData { Metric = new Metric { Hour = hour }, Server = server });

			// Assert
			Assert.That(((RpoMetricLogic.RpoMetricData)result).PotentialDataLoss, Is.EqualTo(120));
		}

		[Test]
		public async Task RpoMetricLogic_CollectMetricData_Null()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 1, 1) };
			var backupGapList = new List<BackupAllGap>();
			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForEachDatabaseAsync<BackupAllGap>(server, hour, GapActivityType.Backup))
				.ReturnsAsync(backupGapList);

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(new[] { new Database { }, });

			// Act
			var result = await this.rpoMetricLogic.CollectMetricData(new MetricData { Metric = new Metric { Hour = hour }, Server = server });

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RpoMetricLogic_CollectMetricData_FromUnresolvedGaps_BackupGapsNull()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 1, 1) };
			var backupGapList = new List<BackupAllGap>();
			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForEachDatabaseAsync<BackupAllGap>(server, hour, GapActivityType.Backup))
				.ReturnsAsync(backupGapList);

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(new[] { new Database { LastBackupFullDate = hour.HourTimeStamp.AddHours(1).AddSeconds(-100) }, });

			// Act
			var result = await this.rpoMetricLogic.CollectMetricData(new MetricData { Metric = new Metric { Hour = hour }, Server = server });

			// Assert
			Assert.That(((RpoMetricLogic.RpoMetricData)result).PotentialDataLoss, Is.EqualTo(100));
			this.databaseGapsRepository.VerifyAll();
		}

		[Test]
		public async Task RpoMetricLogic_CollectMetricData_FromUnresolvedGaps_BackupGapsNotNull()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 1, 1) };
			var backupGap = new BackupAllGap { Duration = 120 };
			var backupGapList = new[] { backupGap };
			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForEachDatabaseAsync<BackupAllGap>(server, hour, GapActivityType.Backup))
				.ReturnsAsync(backupGapList);

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(new[] { new Database { LastBackupFullDate = hour.HourTimeStamp.AddHours(1).AddSeconds(-200) }, });

			// Act
			var result = await this.rpoMetricLogic.CollectMetricData(new MetricData { Metric = new Metric { Hour = hour }, Server = server });

			// Assert
			Assert.That(((RpoMetricLogic.RpoMetricData)result).PotentialDataLoss, Is.EqualTo(200));

			this.databaseGapsRepository.VerifyAll();
		}

		[Test]
		[TestCase(0, 100, 0, false)]
		[TestCase(5, 100, 0, true)]
		[TestCase(240, 80, 0.1, true)]
		[TestCase(500, 62.684, 0.0001, true)]
		[TestCase(1440, 0, 0.1, true)]
		[TestCase(50000, 0, 0, true)]
		public async Task RpoMetricLogic_ScoreMetric(double potentialDataLoss, double expectedResult, double errorMargin, bool metricDataIsNotNull)
		{
			var metricData = new MetricData();
			var metricLogicData = metricDataIsNotNull ? new RpoMetricLogic.RpoMetricData { PotentialDataLoss = (int)TimeSpan.FromMinutes(potentialDataLoss).TotalSeconds } : null;
			this.metricDataService.Setup(s => s.GetData<RpoMetricLogic.RpoMetricData>(metricData))
				.Returns(metricLogicData);

			// Act
			var result = await this.rpoMetricLogic.ScoreMetric(metricData);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public void RpoMetricLogic_FormulaForLessThan15Minutes()
		{
			// Act
			var result = RpoMetricLogic.FormulaForLessThan15Minutes();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		[TestCase(240, 80, 0.1)]
		[TestCase(100, 92.452, 0.0001)]
		[TestCase(15, 100, 0.0001)]
		public void RpoMetricLogic_FormulaForLessThan240Minutes(double potentialDataLossMinutes, double expectedResult, double errorMargin)
		{
			// Act
			var result = RpoMetricLogic.FormulaForLessThan240Minutes(potentialDataLossMinutes);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		[TestCase(1440, 0, 0.1)]
		[TestCase(500, 62.684, 0.0001)]
		[TestCase(240, 80, 0.0001)]
		public void RpoMetricLogic_FormulaForBetween240and1440Minutes(double potentialDataLossMinutes, double expectedResult, double errorMargin)
		{
			// Act
			var result = RpoMetricLogic.FormulaForBetween240And1440Minutes(potentialDataLossMinutes);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public void RpoMetricLogic_FormulaForMoreThan1440Minutes()
		{
			// Act
			var result = RpoMetricLogic.FormulaForMoreThan1440Minutes();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Zero));
		}
	}
}
