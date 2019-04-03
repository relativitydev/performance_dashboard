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
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.Metrics.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class RtoMetricLogicTests
	{
		private RtoMetricLogic rtoMetricLogic;
		private Mock<IGapsCollectionVerifier> gapsCollectionVerifier;
		private Mock<IDatabaseRepository> databaseRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<IRecoveryObjectivesReporter> recoveryObjectivesReporter;
		private Mock<IRecoverabilityIntegritySummaryReporter> recoverabilityIntegritySummaryReporter;


		[SetUp]
		public void Setup()
		{
			this.gapsCollectionVerifier = new Mock<IGapsCollectionVerifier>();
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.metricDataService = new Mock<IMetricDataService>();
			this.recoveryObjectivesReporter = new Mock<IRecoveryObjectivesReporter>();
			this.recoverabilityIntegritySummaryReporter = new Mock<IRecoverabilityIntegritySummaryReporter>();
			this.rtoMetricLogic = new RtoMetricLogic(
				this.gapsCollectionVerifier.Object,
				this.databaseRepository.Object,
				this.metricDataService.Object,
				this.recoveryObjectivesReporter.Object,
				this.recoverabilityIntegritySummaryReporter.Object);
		}

		[Test]
		[TestCase(3, 100, 0.0001)]
		[TestCase(4, 100, 0.0001)]
		[TestCase(14, 90, 0.0001)]
		[TestCase(24, 80, 0.0001)]
		[TestCase(36, 40, 0.001)]
		[TestCase(48, 0, 0.001)]
		[TestCase(60, 0, 0.0001)]
		public async Task Rto_ScoreMetric(int timeToRecover, double expectedResult, double errorMargin)
		{
			// Arrange
			var metricData = new MetricData();
			this.metricDataService.Setup(s => s.GetData<RtoMetricLogic.RtoMetricData>(metricData))
				.Returns(new RtoMetricLogic.RtoMetricData { TimeToRecover = timeToRecover });

			// Act
			var result = await this.rtoMetricLogic.ScoreMetric(metricData);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public async Task Rto_ScoreMetric_NullTimeToRecover()
		{
			// Arrange
			var metricData = new MetricData();
			this.metricDataService.Setup(s => s.GetData<RtoMetricLogic.RtoMetricData>(metricData))
				.Returns(new RtoMetricLogic.RtoMetricData { TimeToRecover = null });

			// Act
			var result = await this.rtoMetricLogic.ScoreMetric(metricData);

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		public void Rto_FormulaForTimeToRecoverIsNull()
		{
			// Act
			var result = RtoMetricLogic.FormulaForTimeToRecoverIsNull();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		public void Rto_FormulaForTimeToRecoverLessThan4()
		{
			// Act
			var result = RtoMetricLogic.FormulaForTimeToRecoverLessThan4();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		[TestCase(4, 100, 0.0001)]
		[TestCase(14, 90, 0.0001)]
		[TestCase(24, 80, 0.0001)]
		public void Rto_FormulaForTimeToRecoverLessThan24(int timeToRecover, double expectedResult, double errorMargin)
		{
			// Act
			var result = RtoMetricLogic.FormulaForTimeToRecoverLessThan24(timeToRecover);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		[TestCase(24, 80, 0.0001)]
		[TestCase(36, 40, 0.001)]
		[TestCase(48, 0, 0.001)]
		public void Rto_FormulaForTimeToRecoverLessThan48(int timeToRecover, double expectedResult, double errorMargin)
		{
			// Act
			var result = RtoMetricLogic.FormulaForTimeToRecoverLessThan48(timeToRecover);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public void Rto_FormulaForTimeToRecoverMoreThan48()
		{
			// Act
			var result = RtoMetricLogic.FormulaForTimeToRecoverMoreThan48();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Zero));
		}

		[Test]
		public async Task Rto_CollectMetricData()
		{
			// Arrange
			var metricData = new MetricData { Server = new Server { ServerId = 123 }, Metric = new Metric { Hour = new Hour { Id = 3 } } };
			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(metricData.Server.ServerId))
				.ReturnsAsync(new[]
				{
					new Database { LastBackupFullDuration = 1*60, LastBackupDiffDuration = 2*60, LogBackupsDuration = 3*60, Id = 1},
					new Database { LastBackupFullDuration = 2*60, LastBackupDiffDuration = 3*60, LogBackupsDuration = 4*60, Id = 2 },
					new Database { LastBackupFullDuration = 3*60, LastBackupDiffDuration = 4*60, LogBackupsDuration = 5*60, Id = 3 }, // should be picked as the database with greatest time to recover
				});

			this.recoverabilityIntegritySummaryReporter.Setup(m => m.UpdateWorstRto(metricData.Metric.Hour.Id,
				It.Is<int>(i => i == 3), It.IsInRange(11.9m, 12.1m, Range.Inclusive))).ReturnsAsyncDefault();
			this.recoveryObjectivesReporter.Setup(m => m.UpdateRtoReport(It.IsAny<IList<DatabaseRtoScoreData>>()))
				.ReturnsAsyncDefault();

			// Act
			var result = await this.rtoMetricLogic.CollectMetricData(metricData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(((RtoMetricLogic.RtoMetricData)result).TimeToRecover, Is.EqualTo(12));

			this.recoverabilityIntegritySummaryReporter.VerifyAll();
			this.recoveryObjectivesReporter.VerifyAll();
		}

		[Test]
		public async Task Rto_CollectMetricData_NullDurations()
		{
			// Arrange
			var metricData = new MetricData { Server = new Server { ServerId = 123 } };
			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(metricData.Server.ServerId))
				.ReturnsAsync(new[]
				{
					new Database (),
					new Database (),
					new Database (),
				});

			// Act
			var result = await this.rtoMetricLogic.CollectMetricData(metricData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(((RtoMetricLogic.RtoMetricData)result).TimeToRecover, Is.Null);
		}
	}
}
