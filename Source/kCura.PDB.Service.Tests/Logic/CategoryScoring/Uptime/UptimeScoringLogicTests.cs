namespace kCura.PDB.Service.Tests.Logic.CategoryScoring.Uptime
{
	using System;
	using System.Collections.Generic;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Moq;
	using NUnit.Framework;
	using System.Threading.Tasks;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Tests.Common;

	[TestFixture, Category("Unit")]
	public class UptimeScoringLogicTests
	{
		[SetUp]
		public void Setup()
		{
			uptimeRatingsRepository = new Mock<IUptimeRatingsRepository>();
			maintenanceWindowRepository = new Mock<IMaintenanceWindowRepository>();
			logger = TestUtilities.GetMockLogger();
		}

		private Mock<IUptimeRatingsRepository> uptimeRatingsRepository;
		private Mock<IMaintenanceWindowRepository> maintenanceWindowRepository;
		private Mock<ILogger> logger;


		[Test,
			TestCase(99.0, 98.0, true, true, 80.0, false),
			TestCase(99.0, 98.0, true, true, 80.0, false),
			TestCase(null, 99.0, true, true, 90.05, false),
			TestCase(99.0, null, true, true, 90.05, false),
			TestCase(99.0, 98.0, true, false, 90.05, false),
			TestCase(98.0, 99.0, false, true, 90.05, false)]
		public async Task UptimeScoring_ScoreMetricsAsync(double? webUptimeScore, double? agentUptimeScore, bool useWebUptimeMetricData, bool useAgentUptimeMetricData, double expectedResult, bool inMaintenanceWindow)
		{
			// Arrange 
			var hour = DateTime.Now;
			var webUptime = webUptimeScore.HasValue ? Convert.ToDecimal(webUptimeScore.Value) : (decimal?)null;
			var agentUptime = agentUptimeScore.HasValue ? Convert.ToDecimal(agentUptimeScore.Value) : (decimal?)null;
			var webUptimeMetricData = new MetricData { Score = webUptime, Metric = new Metric { MetricType = MetricType.WebUptime, Hour = new Hour { HourTimeStamp = hour } } };
			var agentUptimeMetricData = new MetricData { Score = agentUptime, Metric = new Metric { MetricType = MetricType.AgentUptime, Hour = new Hour { HourTimeStamp = hour } } };

			var metricDatas = new List<MetricData>();
			if (useWebUptimeMetricData)
				metricDatas.Add(webUptimeMetricData);
			if (useAgentUptimeMetricData)
				metricDatas.Add(agentUptimeMetricData);
			
			this.uptimeRatingsRepository.Setup(r => r.Create(It.IsAny<decimal>(), hour, It.IsAny<bool>(), inMaintenanceWindow)).Returns(Task.Delay(10));
			this.uptimeRatingsRepository.Setup(r => r.UpdateWeeklyScores()).Returns(Task.Delay(10));
			this.uptimeRatingsRepository.Setup(r => r.UpdateQuartlyScores(hour)).Returns(Task.Delay(10));
			if (metricDatas.Count > 0)
				this.maintenanceWindowRepository.Setup(r => r.HourIsInMaintenanceWindowAsync(metricDatas[0].Metric.Hour)).ReturnsAsync(inMaintenanceWindow);

			var categoryScore = new CategoryScore();

			// Act
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			var result = await logic.ScoreMetrics(categoryScore, metricDatas);

			// Assert
			Assert.That(Math.Round(result, 3), Is.EqualTo(Convert.ToDecimal(expectedResult)));
			this.uptimeRatingsRepository.Verify(r => r.Create(It.IsAny<decimal>(), hour, It.IsAny<bool>(), inMaintenanceWindow));
		}

		[Test,
			TestCase(null, null, false),
			TestCase(55.0, null, true),
			TestCase(null, 55.0, false),
			TestCase(54.0, 55.0, true),
			TestCase(55.0, 55.0, false),
			TestCase(0.0, 55.0, true),
			TestCase(100.0, 55.0, false)]
		public async Task UptimeScoring_ScoreMetricsAsync_WebDowntime(double? webUptimeScore, double? agentUptimeScore, bool expectedWebDowntimeResult)
		{
			// Arrange 
			var hour = DateTime.Now;
			var webUptime = webUptimeScore.HasValue ? Convert.ToDecimal(webUptimeScore.Value) : (decimal?)null;
			var agentUptime = agentUptimeScore.HasValue ? Convert.ToDecimal(agentUptimeScore.Value) : (decimal?)null;
			var webUptimeMetricData = new MetricData { Score = webUptime, Metric = new Metric { MetricType = MetricType.WebUptime, Hour = new Hour { HourTimeStamp = hour } } };
			var agentUptimeMetricData = new MetricData { Score = agentUptime, Metric = new Metric { MetricType = MetricType.AgentUptime, Hour = new Hour { HourTimeStamp = hour } } };

			var metricDatas = new List<MetricData> { agentUptimeMetricData, webUptimeMetricData };

			this.uptimeRatingsRepository.Setup(r => r.Create(It.IsAny<decimal>(), hour, expectedWebDowntimeResult, false)).Returns(Task.Delay(1));
			this.uptimeRatingsRepository.Setup(r => r.UpdateWeeklyScores()).Returns(Task.Delay(1));
			this.uptimeRatingsRepository.Setup(r => r.UpdateQuartlyScores(hour)).Returns(Task.Delay(1));
			if (metricDatas.Count > 0)
				this.maintenanceWindowRepository.Setup(r => r.HourIsInMaintenanceWindowAsync(metricDatas[0].Metric.Hour)).ReturnsAsync(false);

			var categoryScore = new CategoryScore();

			// Act
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			var result = logic.ScoreMetrics(categoryScore, metricDatas);
			await result;

			// Assert
			this.uptimeRatingsRepository.Verify(r => r.Create(It.IsAny<decimal>(), hour, expectedWebDowntimeResult, false));
		}

		[Test]
		public async Task UptimeScoring_ScoreMetricsAsync_NoUptimes()
		{
			// Arrange
			var someOtherMetricData = new MetricData { Metric = new Metric { MetricType = MetricType.Ram } };

			var metricDatas = new List<MetricData>() { someOtherMetricData };
			var categoryScore = new CategoryScore();
			// Act
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			var result = await logic.ScoreMetrics(categoryScore, metricDatas);

			// Assert
			Assert.That(result, Is.EqualTo(100m));
		}

		[Test]
		public void UptimeScoring_ScoreMetricsAsync_NoMetricDatas()
		{
			// Arrange
			var metricDatas = new List<MetricData>();
			var categoryScore = new CategoryScore();

			// Act & Assert
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			Assert.ThrowsAsync<Exception>(() => logic.ScoreMetrics(categoryScore, metricDatas), "Cannot score empty list of metric data.");
		}

		[Test,
			TestCase(null, null, 100),
			TestCase(null, 98, 98),
			TestCase(98, null, 98),
			TestCase(99, 50, 50),
			TestCase(50, 99, 50)]
		public void UptimeScoring_GetUptimePercentage(double? webUptimeScore, double? agentUptimeScore, double expectedResult)
		{
			// Act
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			var web = webUptimeScore.HasValue ? Convert.ToDecimal(webUptimeScore) : (decimal?)null;
			var agent = agentUptimeScore.HasValue ? Convert.ToDecimal(agentUptimeScore) : (decimal?)null;
			var result = logic.GetUptimePercentage(web, agent);

			// Assert
			Assert.That(Math.Round(result, 4), Is.EqualTo(Convert.ToDecimal(expectedResult)));
		}


		[Test,
			TestCase(200.0, 100.0),
			TestCase(100.0, 100.0),
			TestCase(99.995, 100.0),
			TestCase(99.99, 100.0),
			TestCase(99.95, 99.598),
			TestCase(99.90, 99.0955),
			TestCase(98.9, 89.0452),
			TestCase(98.5, 85.0251),
			TestCase(98.0, 80.0),
			TestCase(97.9, 78.995),
			TestCase(92, 19.6985),
			TestCase(90, 0),
			TestCase(50, 0),
			TestCase(0, 0),
			TestCase(-100, 0),
			TestCase(97.83, 78.2915)]
		public void UptimeScoring_UptimeScoreFromUptimePercentage(double uptimePercentage, double expectedResult)
		{
			// Act
			var logic = new UptimeScoringLogic(uptimeRatingsRepository.Object, maintenanceWindowRepository.Object, logger.Object);
			var result = logic.UptimeScoreFromUptimePercentage(Convert.ToDecimal(uptimePercentage));

			// Assert
			Assert.That(Math.Round(result, 4), Is.EqualTo(Convert.ToDecimal(expectedResult)));
		}
	}
}
