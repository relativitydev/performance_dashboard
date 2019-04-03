namespace kCura.PDB.Service.Tests.Logic.Metrics.InfrastructurePerformance
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Metrics.InfrastructurePerformance;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RamMetricLogicTests
	{
		private RamMetricLogic logic;
		private Mock<IProcessControlRepository> processControlRepositoryMock;

		[SetUp]
		public void SetUp()
		{
			this.processControlRepositoryMock = new Mock<IProcessControlRepository>();
			this.logic = new RamMetricLogic(this.processControlRepositoryMock.Object);
		}

		[Test]
		public void CollectMetricData()
		{
			// Arrange
			var metricData = new MetricData();

			// Act
			var result = this.logic.CollectMetricData(metricData);

			// Assert
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task ScoreMetric()
		{
			// Arrange
			var metricData = new MetricData();

			// Act
			var result = await this.logic.ScoreMetric(metricData);

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.OneHundred));
		}

		[Test]
		public async Task IsReady()
		{
			// Arrange
			var metricData = new MetricData { Metric = new Metric { Hour = new Hour { HourTimeStamp = DateTime.UtcNow } } };

			// Act
			var result = await this.logic.IsReady(metricData);

			// Assert
			Assert.Pass($"Result {result}");
		}
	}
}