namespace kCura.PDB.Data.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;
	using kCura.PDB.Data.Repositories;

	using NUnit.Framework;

	using PDB.Tests.Common;

	[TestFixture]
	[Category("Integration")]
	public class AuditAnalysisRepositoryTests
	{
		[OneTimeSetUp]
		public async Task SetUp()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();

			var hourRepo = new HourRepository(connectionFactory);
			var metricRepo = new MetricRepository(connectionFactory);
			var hour = await hourRepo.CreateAsync(new Hour { HourTimeStamp = DateTime.Now });
			var metric = await metricRepo.CreateAsync(new Metric { HourId = hour.Id, MetricType = MetricType.WebUptime });

			var metricDataRepo = new MetricDataRepository(connectionFactory);
			metricData = await metricDataRepo.CreateAsync(new MetricData { MetricId = metric.Id, Score = 100.0m });

			var repo = new AuditAnalysisRepository(connectionFactory);
			await repo.CreateAsync(Enumerable.Range(0, 4).Select(i =>
			new AuditAnalysis
			{
				MetricDataId = metricData.Id,
				TotalQueries = 123,
				TotalLongRunningQueries = 234,
				TotalComplexQueries = 345,
				UserId = 456
			}).ToList());
			auditAnalyses = await repo.ReadByMetricData(metricData);
		}

		private IConnectionFactory connectionFactory;
		private MetricData metricData;
		private IList<AuditAnalysis> auditAnalyses;

		[Test]
		public void AuditAnalysis_Create()
		{
			// Assert
			Assert.That(auditAnalyses.All(aa=>aa.Id > 0), Is.True);
		}

		[Test]
		public void AuditAnalysis_ReadByMetricData()
		{
			// Assert
			Assert.That(auditAnalyses.All(aa => aa.Id > 0), Is.True);
			Assert.That(auditAnalyses.All(aa => aa.TotalQueries == 123), Is.True);
			Assert.That(auditAnalyses.All(aa => aa.TotalLongRunningQueries == 234), Is.True);
			Assert.That(auditAnalyses.All(aa => aa.TotalComplexQueries == 345), Is.True);
			Assert.That(auditAnalyses.All(aa => aa.UserId == 456), Is.True);
			Assert.That(auditAnalyses.All(aa => aa.MetricDataId == this.metricData.Id), Is.True);
		}
	}
}
