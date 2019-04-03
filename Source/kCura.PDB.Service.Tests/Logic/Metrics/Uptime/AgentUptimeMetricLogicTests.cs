using System;

namespace kCura.PDB.Service.Tests.Logic.Metrics.Uptime
{
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Constants;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using Core.Models.MetricDataSources;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Metrics.Uptime;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class AgentUptimeMetricLogicTests
	{
		[SetUp]
		public void Setup()
		{
			agentHistoryRepository = new Mock<IAgentHistoryRepository>();
			metricDataService = new Mock<IMetricDataService>();
			this.loggerMock = TestUtilities.GetMockLogger();
		}

		private Mock<IAgentHistoryRepository> agentHistoryRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<ILogger> loggerMock;

		[Test]
		public async Task AgentUptimeMetricLogic_ScoreMetric()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour { HourTimeStamp = DateTime.UtcNow } } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns(new AgentUptime { SuccessfulSamples = 75, TotalSamples = 100 });
			agentHistoryRepository.Setup(r => r.ReadEarliestAsync()).ReturnsAsync(new AgentHistory { TimeStamp = DateTime.UtcNow.AddYears(-10) });

			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.ScoreMetric(metric);

			//Assert
			Assert.That(result, Is.EqualTo(75.0m));
		}

		[Test]
		public async Task AgentUptimeMetricLogic_ScoreMetric_NoUptime()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour { HourTimeStamp = DateTime.UtcNow } } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns((AgentUptime)null);
			agentHistoryRepository.Setup(r => r.ReadEarliestAsync()).ReturnsAsync(new AgentHistory { TimeStamp = DateTime.UtcNow.AddYears(-10) });

			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.ScoreMetric(metric);

			//Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Uptime));
		}

		[Test]
		public async Task AgentUptimeMetricLogic_ScoreMetric_NoSamples()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour { HourTimeStamp = DateTime.UtcNow } } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns(new AgentUptime { SuccessfulSamples = 0, TotalSamples = 0 });
			agentHistoryRepository.Setup(r => r.ReadEarliestAsync()).ReturnsAsync(new AgentHistory { TimeStamp = DateTime.UtcNow.AddYears(-10) });

			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.ScoreMetric(metric);

			//Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Zero));
		}

		[Test]
		public async Task AgentUptimeMetricLogic_ScoreMetric_BeforeFirstAgentHistory()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour { HourTimeStamp = DateTime.UtcNow } } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns(new AgentUptime { SuccessfulSamples = 0, TotalSamples = 0 });
			agentHistoryRepository.Setup(r => r.ReadEarliestAsync()).ReturnsAsync(new AgentHistory { TimeStamp = DateTime.UtcNow.AddYears(10) });

			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.ScoreMetric(metric);

			//Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		public async Task AgentUptimeMetricLogic_CollectMetricData()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour() } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns(new AgentUptime { SuccessfulSamples = 10, TotalSamples = 100 });
			agentHistoryRepository.Setup(r => r.ReadByHourAsync(It.IsAny<Hour>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => new AgentHistory { Successful = i % 10 != 0 }).ToList());
			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.CollectMetricData(metric);

			//Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task AgentUptimeMetricLogic_CollectMetricData_NewAgentUptime()
		{
			//Arrange
			var metric = new MetricData { Metric = new Metric { Hour = new Hour() } };
			metricDataService.Setup(mds => mds.GetData<AgentUptime>(It.IsAny<MetricData>())).Returns((AgentUptime)null);
			agentHistoryRepository.Setup(r => r.ReadByHourAsync(It.IsAny<Hour>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => new AgentHistory { Successful = i % 10 != 0 }).ToList());
			var logic = new AgentUptimeMetricLogic(agentHistoryRepository.Object, metricDataService.Object, this.loggerMock.Object);

			//Act
			var result = await logic.CollectMetricData(metric);

			//Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
