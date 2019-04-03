namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class GapsFrequencyAnalyzerTests
	{
		private Mock<IDatabaseRepository> databaseRepository;
		private Mock<IDatabaseGapsRepository> databaseGapsRepository;
		private Mock<ILogger> logger;
		private GapsFrequencyAnalyzer gapsFrequencyAnalyzer;

		[SetUp]
		public void Setup()
		{
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.databaseGapsRepository = new Mock<IDatabaseGapsRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.gapsFrequencyAnalyzer = new GapsFrequencyAnalyzer(
				this.databaseRepository.Object,
				this.databaseGapsRepository.Object,
				this.logger.Object);
		}

		[Test]
		[TestCase(4, 3, 4, TestName = "From gaps")]
		[TestCase(4, 5, 5, TestName = "From unresolved gaps")]
		public async Task GapsFrequencyAnalyzer_CaptureFrequencyData(int gapDuration, int unresolvedGapDuration, int expectedValue)
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 4, 4) };
			var window = Defaults.RecoverabilityIntegrity.WindowInDays;

			var largestGap = new DbccGap { Duration = (int)TimeSpan.FromDays(window + gapDuration).TotalSeconds };

			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc))
				.ReturnsAsync(largestGap);

			this.databaseRepository.Setup(r => r.ReadMostOutOfDateActivityByServerAsync(server, GapActivityType.Dbcc))
				.ReturnsAsync(hour.HourTimeStamp.Add(-TimeSpan.FromDays(window + unresolvedGapDuration)));

			// Act
			var result = await this.gapsFrequencyAnalyzer.CaptureFrequencyData<DbccGap>(hour, server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.WindowExceededBy, Is.EqualTo(expectedValue));
		}

		[Test]
		public async Task GapsFrequencyAnalyzer_CaptureFrequencyData_DatabaseLastDbccDateIsNull()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 4, 4) };
			var window = Defaults.RecoverabilityIntegrity.WindowInDays;

			var largestGap = new DbccGap { Duration = (int)TimeSpan.FromDays(window + 4).TotalSeconds };

			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc))
				.ReturnsAsync(largestGap);

			this.databaseRepository.Setup(r => r.ReadMostOutOfDateActivityByServerAsync(server, GapActivityType.Dbcc))
				.ReturnsAsync((DateTime?)null);

			// Act
			var result = await this.gapsFrequencyAnalyzer.CaptureFrequencyData<DbccGap>(hour, server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.WindowExceededBy, Is.EqualTo(4));
		}

		[Test]
		public async Task GapsFrequencyAnalyzer_CaptureFrequencyData_NoGaps()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 4, 4) };
			var window = Defaults.RecoverabilityIntegrity.WindowInDays;

			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc))
				.ReturnsAsync((DbccGap)null);

			this.databaseRepository.Setup(r => r.ReadMostOutOfDateActivityByServerAsync(server, GapActivityType.Dbcc))
				.ReturnsAsync(hour.HourTimeStamp.Add(-TimeSpan.FromDays(window + 5)));

			// Act
			var result = await this.gapsFrequencyAnalyzer.CaptureFrequencyData<DbccGap>(hour, server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.WindowExceededBy, Is.EqualTo(5));
		}

		[Test]
		public async Task GapsFrequencyAnalyzer_CaptureFrequencyData_NoGapsAndNoDatabaseUnresolvedGap()
		{
			// Arrange
			var server = new Server { ServerId = 123 };
			var hour = new Hour { HourTimeStamp = new DateTime(2018, 4, 4) };

			this.databaseGapsRepository.Setup(r => r.ReadLargestGapsForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc))
				.ReturnsAsync((DbccGap)null);

			this.databaseRepository.Setup(r => r.ReadMostOutOfDateActivityByServerAsync(server, GapActivityType.Dbcc))
				.ReturnsAsync((DateTime?)null);

			// Act
			var result = await this.gapsFrequencyAnalyzer.CaptureFrequencyData<DbccGap>(hour, server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.WindowExceededBy.HasValue, Is.False);
		}

		[Test]
		public void GapsFrequencyAnalyzer_FormulaForWindowExceededByMoreThan20Days()
		{
			// Act
			var result = GapsFrequencyAnalyzer.FormulaForWindowExceededByMoreThan20Days();

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Zero));
		}

		[Test]
		[TestCase(0, 100, 0.0001)]
		[TestCase(20, 0, 0.0001)]
		[TestCase(1, 95, 0.0001)]
		[TestCase(10, 50, 0.0001)]
		[TestCase(19, 5, 0.0001)]
		public void GapsFrequencyAnalyzer_FormulaForWindowExceededByMoreThan20Days(int windowExceededBy, double expectedResult, double errorMargin)
		{
			// Act
			var result = GapsFrequencyAnalyzer.FormulaForWindowExceededLessThan20Days(windowExceededBy);

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		[TestCase(0, 100, 0.0001)]
		[TestCase(1, 95, 0.0001)]
		[TestCase(10, 50, 0.0001)]
		[TestCase(19, 5, 0.0001)]
		[TestCase(20, 0, 0.0001)]
		[TestCase(50, 0, 0.0001)]
		public void GapsFrequencyAnalyzer_ScoreMetric(int windowExceededBy, double expectedResult, double errorMargin)
		{
			// Act
			var result = this.gapsFrequencyAnalyzer.ScoreFrequencyData(new FrequencyMetricData { WindowExceededBy = windowExceededBy });

			// Assert
			Assert.That(Math.Abs((double)result - expectedResult), Is.LessThanOrEqualTo(errorMargin));
		}

		[Test]
		public void GapsFrequencyAnalyzer_ScoreMetric_WindowExceededByIsNull()
		{
			// Act
			var result = this.gapsFrequencyAnalyzer.ScoreFrequencyData(null);

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}
	}
}
