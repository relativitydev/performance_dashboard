using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.PDB.Core.Constants;
using kCura.PDB.Core.Interfaces.Repositories;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Core.Models;
using kCura.PDB.Core.Models.MetricDataSources;
using kCura.PDB.Tests.Common;
using Moq;
using NUnit.Framework;

namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Metrics.UserExperience;

	[TestFixture, Category("Unit")]
	public class AuditAnalysisMetricLogicTests
	{
		[SetUp]
		public void Setup()
		{
			this.poisonWaitRepository = new Mock<IPoisonWaitRepository>();
			this.searchAuditBatchRepository = new Mock<ISearchAuditBatchRepository>();
			this.auditAnalysisRepository = new Mock<IAuditAnalysisRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.logic = new AuditAnalysisMetricLogic(searchAuditBatchRepository.Object, auditAnalysisRepository.Object, poisonWaitRepository.Object, logger.Object);
		}

		private Mock<IPoisonWaitRepository> poisonWaitRepository;
		private Mock<ISearchAuditBatchRepository> searchAuditBatchRepository;
		private Mock<IAuditAnalysisRepository> auditAnalysisRepository;
		private Mock<ILogger> logger;
		private AuditAnalysisMetricLogic logic;

		[Test, Explicit("TODO: Runs too long for TeamCity")]
		[TestCase(true, true, true)]
		[TestCase(false, true, false)]
		[TestCase(true, false, false)]
		[TestCase(false, false, false)]
		public async Task AuditAnalysisMetricLogic_IsReady(bool poisonWaitReady, bool searchAuditBatchReady, bool expectedResult)
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();
			var metricData = new MetricData { Metric = new Metric { Hour = hour }, Server = server, ServerId = 123 };

			this.poisonWaitRepository.Setup(r => r.ReadIfPoisonWaitsForHourAsync(hour))
				.ReturnsAsync(poisonWaitReady);
			this.searchAuditBatchRepository.Setup(r => r.ExistsForHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value))
				.ReturnsAsync(searchAuditBatchReady);

			// Act
			var result = await logic.IsReady(metricData);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task AuditAnalysisMetricLogic_CollectMetricData()
		{
			// Arrange
			var batchResults = Enumerable.Range(0, 4).Select(i => new SearchAuditBatchResult { UserId = i % 2, TotalComplexQueries = i + 1, TotalLongRunningQueries = i, TotalQueries = i + 2 });
			var metricData = new MetricData { Metric = new Metric { HourId = 123 }, ServerId = 234 };
			this.searchAuditBatchRepository.Setup(r => r.ReadByHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value))
				.Returns(new[]
				{
					new SearchAuditBatch { BatchResults = batchResults.Take(2).ToList()},
					new SearchAuditBatch { BatchResults = batchResults.Skip(2).ToList()}
				});
			this.auditAnalysisRepository.Setup(r => r.CreateAsync(It.IsAny<IList<AuditAnalysis>>())).Returns(Task.Delay(10));

			// Act
			var result = await logic.CollectMetricData(metricData);

			// Assert
			Assert.That(result, Is.Not.Null);
			this.auditAnalysisRepository.Verify(r => r.CreateAsync(It.Is<IList<AuditAnalysis>>(l =>
				l[0].UserId == 0 && l[0].TotalLongRunningQueries == 2 && l[0].TotalComplexQueries == 4 && l[0].TotalQueries == 6 &&
				l[1].UserId == 1 && l[1].TotalLongRunningQueries == 4 && l[1].TotalComplexQueries == 6 && l[1].TotalQueries == 8)));
		}

		[Test]
		public async Task AuditAnalysisMetricLogic_ScoreMetric()
		{
			// Arrange
			var metricData = new MetricData { Metric = new Metric { HourId = 123 }, ServerId = 234 };
			var analyses = new[] { new AuditAnalysis { TotalComplexQueries = 10, TotalLongRunningQueries = 5, TotalQueries = 20, TotalSimpleLongRunningQueries = 5 } };
			this.auditAnalysisRepository.Setup(r => r.ReadByMetricData(metricData))
				.ReturnsAsync(analyses.ToList());

			// Act
			var result = await logic.ScoreMetric(metricData);

			// Assert
			Assert.That(result, Is.EqualTo(50.0));
		}

		[Test]
		public async Task AuditAnalysisMetricLogic_ScoreMetric_NoMetricData()
		{
			// Arrange
			var metricData = new MetricData { Metric = new Metric { HourId = 123 }, ServerId = 234 };
			var analyses = new AuditAnalysis[] { };
			this.auditAnalysisRepository.Setup(r => r.ReadByMetricData(metricData))
				.ReturnsAsync(analyses.ToList());

			// Act
			var result = await logic.ScoreMetric(metricData);

			// Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.UserExperience));
		}

		[Test,
			TestCase(20, 10, 5, 0, 1),
			TestCase(0, 0, 0, 0, 1, TestName = "Zero data"),
			TestCase(1, 1, 1, 0, 1, TestName = "Single Complex Long Running"),
			TestCase(1, 0, 1, 1, 0, TestName = "Single Simple Long Running"),
			TestCase(2, 1, 1, 0, 1),
			TestCase(10, 6, 4, 3, 0.25),
			TestCase(10, 10, 0, 0, 1),
			TestCase(20, 10, 15, 5, 0.5)]
		public void AuditAnalysisMetricLogic_ScoreUserAuditAnalysis(long totalQueries, long totalComplexQueries, long totalLongRunningQueries, long totalSimpleLongRunningQueries, double expectedResult)
		{
			// Arrange
			var analysis = new AuditAnalysis
			{
				TotalQueries = totalQueries,
				TotalComplexQueries = totalComplexQueries,
				TotalLongRunningQueries = totalLongRunningQueries,
				TotalSimpleLongRunningQueries = totalSimpleLongRunningQueries
			};

			// Act
			var result = AuditAnalysisMetricLogic.ScoreUserAuditAnalysis(analysis);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
