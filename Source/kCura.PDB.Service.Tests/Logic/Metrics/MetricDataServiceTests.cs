namespace kCura.PDB.Service.Tests.Logic.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using kCura.PDB.Service.Metrics;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class MetricDataServiceTests
	{
		[SetUp]
		public void Setup()
		{
			metricRepository = new Mock<IMetricRepository>();
			metricDataRepository = new Mock<IMetricDataRepository>();
			hourRepository = new Mock<IHourRepository>();
			serverRepository = new Mock<IServerRepository>();
		}

		private Mock<IMetricRepository> metricRepository;
		private Mock<IMetricDataRepository> metricDataRepository;
		private Mock<IHourRepository> hourRepository;
		private Mock<IServerRepository> serverRepository;

		[Test]
		public async Task ReadMetricDatasByCategoryScore()
		{
			// Arrange
			var categoryScore = new CategoryScore();
			var metricData1 = new MetricData { MetricId = 10001, ServerId = 400 };
			var metricData2 = new MetricData { MetricId = 10002, ServerId = 400 };
			var metric1 = new Metric { Id = 10001, HourId = 500 };
			var metric2 = new Metric { Id = 10002, HourId = 500 };
			var server = new Server { ServerId = 400 };
			var hour = new Hour { Id = 500 };
			var metricDatas = new List<MetricData>() { metricData1, metricData2 };

			metricDataRepository.Setup(r => r.ReadByCategoryScoreAsync(categoryScore)).ReturnsAsync(metricDatas);
			metricRepository.Setup(r => r.ReadAsync(10001)).ReturnsAsync(metric1);
			metricRepository.Setup(r => r.ReadAsync(10002)).ReturnsAsync(metric2);
			hourRepository.Setup(r => r.ReadAsync(500)).ReturnsAsync(hour);
			serverRepository.Setup(r => r.ReadAsync(400)).ReturnsAsync(server);

			// Act
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var result = await service.ReadMetricDatasByCategoryScoreAsync(categoryScore);

			// Assert
			Assert.That(result[0].Metric.Id, Is.EqualTo(metric1.Id));
			Assert.That(result[0].Metric.Hour.Id, Is.EqualTo(hour.Id));
			Assert.That(result[0].Server.ServerId, Is.EqualTo(server.ServerId));
			Assert.That(result[1].Metric.Id, Is.EqualTo(metric2.Id));
			Assert.That(result[1].Metric.Hour.Id, Is.EqualTo(hour.Id));
			Assert.That(result[1].Server.ServerId, Is.EqualTo(server.ServerId));
		}

		private class TestData
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public DateTime Time { get; set; }
		}

		[Test]
		public void GetData_TestData_Success()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var metricData = new MetricData
			{
				Data = "{\"ID\":12345,\"Name\":\"Steve\",\"Time\":\"0001-01-01T00:00:00\"}"
			};

			// Act
			var result = service.GetData<TestData>(metricData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(12345));
			Assert.That(result.Name, Is.EqualTo("Steve"));
			Assert.That(result.Time, Is.EqualTo(DateTime.MinValue));
		}

		[Test]
		public void GetData_TestData_NullDataSource()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);

			var metricData = new MetricData
			{
				Data = null
			};

			// Act
			var result = service.GetData<TestData>(metricData);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void SetData_TestData_Success()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var data = new TestData
			{
				Id = 3,
				Name = "Tom",
				Time = DateTime.MinValue
			};
			var metricData = new MetricData();

			// Act
			service.SetData(metricData, data);

			// Assert
			Assert.That(metricData.Data, Is.Not.Null);
			Assert.That(metricData.Data, Is.Not.Empty);
		}

		[Test]
		public void SetData_TestData_NullData()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var metricData = new MetricData();

			// Act
			service.SetData(metricData, (object)null);

			// Assert
			Assert.That(metricData.Data, Is.Null);
		}

		[Test]
		public async Task GetMetricData_MetricDataId()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var metricDataId = 4;
			var metricId = 3;
			var hourId = 2;
			var serverId = 1;
			var metricData = new MetricData {MetricId = metricId, Id = metricDataId, ServerId = serverId};
			var metric = new Metric {Id = metricId, HourId = hourId};
			var hour = new Hour {Id = hourId};
			var server = new Server {ServerId = serverId};

			this.metricDataRepository.Setup(m => m.ReadAsync(metricDataId)).ReturnsAsync(metricData);
			this.metricRepository.Setup(m => m.ReadAsync(metricData.MetricId)).ReturnsAsync(metric);
			this.hourRepository.Setup(m => m.ReadAsync(metric.HourId)).ReturnsAsync(hour);
			this.serverRepository.Setup(m => m.ReadAsync(metricData.ServerId.Value)).ReturnsAsync(server);

			// Act
			var result = await service.GetMetricData(metricDataId);

			// Assert
			Assert.That(metricData.Data, Is.Null);
			this.metricDataRepository.VerifyAll();
			this.metricRepository.VerifyAll();
			this.hourRepository.VerifyAll();
			this.serverRepository.VerifyAll();
		}

		[Test, Explicit("TODO - Figure out how to make test functional"), Category("Explicit")]
		public async Task GetMetricData_HourIdsServerCategoryType()
		{
			// Arrange
			var service = new MetricDataService(metricRepository.Object, metricDataRepository.Object, hourRepository.Object, serverRepository.Object);
			var metricData = new MetricData();
			var metricDataId = 4;

			this.metricDataRepository.Setup(m => m.ReadAsync(metricDataId)).ReturnsAsync(metricData);


			var serverId = 3;
			var hourIds = new []{3, 4, 5};
			var categoryType = CategoryType.UserExperience;

			// Act
			var result = await service.GetMetricData(hourIds, serverId, categoryType);

			// Assert
			Assert.That(metricData.Data, Is.Null);
		}
	}
}
