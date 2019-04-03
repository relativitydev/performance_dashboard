namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Models;
	using Data.Repositories;
	using kCura.PDB.Core.Extensions;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class MetricDataRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			this.metricRepository = new MetricRepository(ConnectionFactorySetup.ConnectionFactory);
			var serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			this.server = await serverRepository.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.1",
				IgnoreServer = false,
				ResponsibleAgent = "",
				ArtifactId = 1234,
			});
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now.NormilizeToHour() });
			metric = await this.metricRepository.CreateAsync(new Metric { HourId = hour.Id, MetricType = MetricType.WebUptime });

			this.metricDataRepository = new MetricDataRepository(ConnectionFactorySetup.ConnectionFactory);
			metricData = await this.metricDataRepository.CreateAsync(new MetricData { MetricId = metric.Id, Score = 100.0m });
			metricData.Score = 75.0m;
			metricData.Data = "asdf";
			await this.metricDataRepository.UpdateAsync(metricData);
			metricData = await this.metricDataRepository.ReadAsync(metricData.Id);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			var serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			await serverRepository.DeleteAsync(this.server);
		}

		private Hour hour;
		private Server server;
		private Metric metric;
		private MetricData metricData;
		private MetricDataRepository metricDataRepository;
		private MetricRepository metricRepository;

		[Test]
		public void MetricData_CreateAsync_Success()
		{
			//Assert
			Assert.That(metricData, Is.Not.Null);
			Assert.That(metricData.Id, Is.GreaterThan(0));
			Assert.That(metricData.MetricId, Is.EqualTo(metric.Id));
			Assert.That(metricData.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public void MetricData_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(metricData, Is.Not.Null);
			Assert.That(metricData.Id, Is.GreaterThan(0));
			Assert.That(metricData.MetricId, Is.EqualTo(metric.Id));
			Assert.That(metricData.Score, Is.EqualTo(75.0m));
		}

		[Test]
		public void MetricData_UpdateAsync_Success()
		{
			//Assert
			Assert.That(metricData.Score, Is.EqualTo(75.0m));
			Assert.That(metricData.Data, Is.EqualTo("asdf"));
		}

		[Test]
		public async Task MetricData_ReadByCategoryScoreAsync_Success()
		{
			//Arrange
			var repo = new MetricDataRepository(ConnectionFactorySetup.ConnectionFactory);
			var categoryScoreRepo = new CategoryScoreRepository(ConnectionFactorySetup.ConnectionFactory);
			var categoryRepo = new CategoryRepository(ConnectionFactorySetup.ConnectionFactory);
			var category = await categoryRepo.CreateAsync(new Category { HourId = hour.Id, CategoryType = CategoryType.Uptime });
			var categoryScore = await categoryScoreRepo.CreateAsync(new CategoryScore { CategoryId = category.Id, Category = category });

			//Act
			var result = await repo.ReadByCategoryScoreAsync(categoryScore);

			//Assert
			Assert.That(result.Any(r => r.Id == metricData.Id), Is.True);
		}

		[Test]
		public async Task MetricData_ReadByHourAndMetricTypeAsync_Success()
		{
			//Arrange
			var backupGapsMetric = await this.metricRepository.CreateAsync(new Metric { HourId = hour.Id, MetricType = MetricType.BackupGaps });
			var backupGapsMetricData = await this.metricDataRepository.CreateAsync(new MetricData { MetricId = backupGapsMetric.Id, ServerId = this.server.ServerId });

			//Act
			var result = await this.metricDataRepository.ReadByHourAndMetricTypeAsync(this.hour, this.server, MetricType.BackupGaps);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(backupGapsMetricData.Id));
		}

		[Test]
		public async Task MetricData_ReadWorstScoreInDateRangeAsync_Success()
		{
			//Arrange
			var score = -5; // negative 5 is used to guarantee that this metric data is the worst
			var ropMetric = await this.metricRepository.CreateAsync(new Metric { HourId = hour.Id, MetricType = MetricType.Rpo });
			var rpoMetricData = await this.metricDataRepository.CreateAsync(new MetricData { MetricId = ropMetric.Id, ServerId = this.server.ServerId, Score = score });
			var startTime = hour.HourTimeStamp.AddHours(-1);
			var endTime = hour.HourTimeStamp.AddHours(1);

			//Act
			var result = await this.metricDataRepository.ReadWorstScoreInDateRangeAsync(startTime, endTime, this.server, MetricType.Rpo);

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(rpoMetricData.Id));
			Assert.That(result.Score, Is.EqualTo(score));
		}

		[Test]
		public async Task MetricData_ZDeleteAsync_Success()
		{
			//Arrange
			var repo = new MetricDataRepository(ConnectionFactorySetup.ConnectionFactory);

			//Act
			await repo.DeleteAsync(metricData);

			//Assert
			var readResult = await repo.ReadAsync(metricData.Id);
			Assert.That(readResult, Is.Null);
		}
	}
}
