namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class MetricRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var hourRepo = new HourRepository(ConnectionFactorySetup.ConnectionFactory);
			hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });

			metricRepository = new MetricRepository(ConnectionFactorySetup.ConnectionFactory);
			metric = await metricRepository.CreateAsync(new Metric { HourId = hour.Id, MetricType = MetricType.AgentUptime });
			metric = await metricRepository.ReadAsync(metric.Id);
			metric.MetricType = MetricType.WebUptime;
			await metricRepository.UpdateAsync(metric);
		}

		private Hour hour;
		private Metric metric;
		private MetricRepository metricRepository;

		[Test]
		public void Metric_CreateAsync_Success()
		{
			//Assert
			Assert.That(metric, Is.Not.Null);
			Assert.That(metric.Id, Is.GreaterThan(0));
		}

		[Test]
		public void Metric_ReadAsync_ByID_Success()
		{
			//Assert
			Assert.That(metric, Is.Not.Null);
			Assert.That(metric.Id, Is.GreaterThan(0));
			Assert.That(metric.MetricType, Is.EqualTo(MetricType.WebUptime));
			Assert.That(metric.HourId, Is.EqualTo(hour.Id));
		}

		[Test]
		public void Metric_UpdateAsync_Success()
		{
			// Assert
			Assert.That(metric.MetricType, Is.EqualTo(MetricType.WebUptime));
		}

		[Test]
		public async Task Metric_ZDeleteAsync_Success()
		{
			// Act
			await metricRepository.DeleteAsync(metric);

			// Assert
			var readResult = await metricRepository.ReadAsync(metric.Id);
			Assert.That(readResult, Is.Null);
		}
	}
}
