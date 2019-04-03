namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class GapsCoverageAnalyzerTests
	{
		private GapsCoverageAnalyzer gapCoverageAnalyzer;
		private Mock<IDatabaseRepository> databaseRepository;
		private Mock<IDatabaseGapsRepository> databaseGapsRepository;
		private Mock<ILogger> logger;
		private Mock<IGapReporter> gapReporter;

		[SetUp]
		public void Setup()
		{
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.databaseGapsRepository = new Mock<IDatabaseGapsRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.gapReporter = new Mock<IGapReporter>();
			this.gapCoverageAnalyzer = new GapsCoverageAnalyzer(
				this.databaseRepository.Object,
				this.databaseGapsRepository.Object,
				this.logger.Object,
				this.gapReporter.Object);
		}

		[Test]
		[TestCase(9000, 0, 0.0001, TestName = "9k out of 10k are covered so 1k are not covered which is 10% with 0 score")]
		[TestCase(9925, 100, 0.0001, TestName = "9925 out of 10k are covered so 75 are not covered which is 0_75% with 100 score")]
		[TestCase(9463, 50, 0.1, TestName = "9462_5 out of 10k are covered so 537.5 are not covered which is 5_375% with 50 score")]
		public void GapCoverageAnalyzer_ScoreCoverageData(int coveredDatabases, double expectedResult, double errorMargin)
		{
			// Arrange
			// Arrange
			var coverageMetricData = new CoverageMetricData { DatabasesCovered = coveredDatabases, TotalDatabases = 10 * 1000 };

			// Act
			var result = this.gapCoverageAnalyzer.ScoreCoverageData(coverageMetricData);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public async Task GapCoverageAnalyzer_CaptureCoverageData()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 4, 4) };
			var window = Defaults.RecoverabilityIntegrity.WindowInDays;
			var windowInSecs = (int)TimeSpan.FromDays(window).TotalSeconds;
			var gapDurations = new[] { 4, 3 }; // 3 and 4 days exceeding window are bad but there are only two gaps so there will be no gap for 3rd db
			var unresolvedGapDurations = new[] { -2, -3 };
			var coveredDatabases = 3;
			var totalDatabases = gapDurations.Length + unresolvedGapDurations.Length + coveredDatabases;

			var gaps = gapDurations.Select((duration, i) => new DbccGap
			{
				DatabaseId = i,
				Duration = (int)TimeSpan.FromDays(window + duration).TotalSeconds
			}).ToList();

			var databases = unresolvedGapDurations
				.Select((duration, i) => new Database
				{
					Id = i + gapDurations.Length, // gapDurations.Length is added so that they are different database id's from gaps above
					LastDbccDate = hour.HourTimeStamp.Add(-TimeSpan.FromDays(window + duration))
				})
				.ToList();

			this.databaseRepository.Setup(r => r.ReadCountByServerAsync(server))
				.ReturnsAsync(totalDatabases);
			this.databaseGapsRepository.Setup(r => r.ReadGapsLargerThanForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc, windowInSecs))
				.ReturnsAsync(gaps);
			this.databaseRepository.Setup(r=>r.ReadOutOfDateDatabasesAsync(server, It.IsAny<DateTime>(), GapActivityType.Dbcc))
				.ReturnsAsync(databases);
			this.gapReporter.Setup(m => m.CreateGapReport(hour, server, gaps, GapActivityType.Dbcc)).ReturnsAsyncDefault();

			// Act
			var result = await this.gapCoverageAnalyzer.CaptureCoverageData<DbccGap>(hour, server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.DatabasesCovered, Is.EqualTo(coveredDatabases));
			Assert.That(result.TotalDatabases, Is.EqualTo(totalDatabases));

			this.gapReporter.VerifyAll();
		}

		[Test]
		public void GapCoverageAnalyzer_FormulaForNoncoveredRatioLessThan0_75Percent()
		{
			// Act
			var result = GapsCoverageAnalyzer.FormulaForNoncoveredRatioLessThan0_75Percent();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		public void GapCoverageAnalyzer_FormulaForNoncoveredRatioMoreThan10Percent()
		{
			// Act
			var result = GapsCoverageAnalyzer.FormulaForNoncoveredRatioMoreThan10Percent();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Zero));
		}

		[Test]
		[TestCase(0.1, 0, 0.0001)]
		[TestCase(0.0075, 100, 0.0001)]
		[TestCase(0.05375, 50, 0.0001)]
		public void GapCoverageAnalyzer_FormulaForNoncoveredRatioBetween0_075And0_1Percent(double noncoveredRatio, double expectedResult, double errorMargin)
		{
			// Act
			var result = GapsCoverageAnalyzer.FormulaForNoncoveredRatioBetween0_075And0_1Percent(noncoveredRatio);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}
	}
}
