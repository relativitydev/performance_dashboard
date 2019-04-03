namespace kCura.PDB.Service.Tests.Logic.CategoryScoring.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.MetricDataSources;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class UserExperienceScoringLogicTests
	{
		[SetUp]
		public void SetUp()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);
			this.userExperienceRatingsRepositoryMock = this.mockRepository.Create<IUserExperienceRatingsRepository>();
			this.hourRepositoryMock = this.mockRepository.Create<IHourRepository>();
			this.serverAuditReporter = this.mockRepository.Create<IServerAuditReporter>();
			this.userExperienceSampleServiceMock = this.mockRepository.Create<IUserExperienceSampleService>();
			this.metricDataServiceMock = this.mockRepository.Create<IMetricDataService>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.userExperienceScoringLogic = new UserExperienceScoringLogic(
				this.userExperienceRatingsRepositoryMock.Object,
				this.hourRepositoryMock.Object,
				this.serverAuditReporter.Object,
				this.userExperienceSampleServiceMock.Object,
				this.metricDataServiceMock.Object,
				this.loggerMock.Object);
		}

		private MockRepository mockRepository;
		private Mock<IUserExperienceRatingsRepository> userExperienceRatingsRepositoryMock;
		private Mock<IHourRepository> hourRepositoryMock;
		private Mock<IServerAuditReporter> serverAuditReporter;
		private Mock<IUserExperienceSampleService> userExperienceSampleServiceMock;
		private Mock<IMetricDataService> metricDataServiceMock;
		private Mock<ILogger> loggerMock;
		private UserExperienceScoringLogic userExperienceScoringLogic;

		[Test]
		public async Task ScoreMetrics_Empty()
		{
			// Arrange
			// server
			var serverId = 1;
			var serverArtifactId = 2;

			// hour
			var hourId = 9;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			// parameters
			var category = new Category { HourId = hourId };
			var categoryScore = new CategoryScore
			{
				ServerId = serverId,
				Category = category,
				Server = new Server { ArtifactId = serverArtifactId, ServerId = serverId }
			};
			var metricDatas = new List<MetricData>();

			// scores
			var UXArrivalRateScore = Defaults.Scores.UserExperience;
			var UXConcurrencyScore = Defaults.Scores.UserExperience;
			var expectedResult = (UXArrivalRateScore + UXConcurrencyScore) / 2;

			// sample
			var sample = new PastWeekEligibleSample { ServerId = serverId, HourId = hourId, ArrivalRateHours = new List<SampleHistory>(), ConcurrencyHours = new List<SampleHistory>() };
			this.userExperienceSampleServiceMock.Setup(m => m.CalculateSample(serverId, hourId)).ReturnsAsync(sample);
			this.userExperienceSampleServiceMock.Setup(m => m.UpdateCurrentSample(sample)).Returns(Task.Delay(1));

			// writing to userExperienceRatings
			this.userExperienceRatingsRepositoryMock.Setup(
					m => m.CreateAsync(serverArtifactId, UXArrivalRateScore, UXConcurrencyScore, hour.Id))
				.Returns(Task.CompletedTask);

			// report data
			this.serverAuditReporter.Setup(r => r.ReportServerAudits(categoryScore.Server, hour, It.Is<decimal>(d => d == expectedResult)))
				.Returns(Task.Delay(1));
			this.serverAuditReporter.Setup(r => r.FinalizeServerReports(categoryScore.Server, hour))
				.Returns(Task.Delay(1));
			this.serverAuditReporter.Setup(r => r.DeleteServerTempReportData(categoryScore.Server, hour)).Returns(Task.Delay(1));

			// Act
			var result = await this.userExperienceScoringLogic.ScoreMetrics(categoryScore, metricDatas);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
			this.mockRepository.VerifyAll();
		}

		[Test]
		[TestCase(100, 100)]
		[TestCase(0, 0)]
		[TestCase(98, 98)]
		[TestCase(100, 50)]
		public async Task ScoreMetrics(decimal uxArrivalRateScore, decimal uxConcurrencyScore)
		{
			// scores
			var UXArrivalRateScore = 98;
			var UXConcurrencyScore = 98;
			var expectedResult = (UXArrivalRateScore + UXConcurrencyScore) / 2;

			// Arrange
			// server
			var serverId = 1;
			var serverArtifactId = 2;

			// hour
			var hourId = 9;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);
			var concurrencySampleHistoryHourId = 8;
			var arrivalRateSampleHistoryHourId = 7;
			var sampleConcurrencyHours = new List<SampleHistory>
			{
				new SampleHistory
				{
					HourId = concurrencySampleHistoryHourId,
					IsActiveConcurrencySample = true,
					IsActiveArrivalRateSample = true
				}
			};
			var sampleArrivalRateHours = new List<SampleHistory>
			{
				new SampleHistory {HourId = arrivalRateSampleHistoryHourId, IsActiveArrivalRateSample = true}
			};

			var concurrencyMetricData = new List<MetricData> { new MetricData { Score = UXConcurrencyScore } };
			var arrivalRateMetricData = new List<MetricData> { new MetricData { Score = UXArrivalRateScore } };
			this.metricDataServiceMock
				.Setup(m => m.GetMetricData(It.Is<IList<int>>(list => list.Contains(concurrencySampleHistoryHourId)), serverId,
					CategoryType.UserExperience)).ReturnsAsync(concurrencyMetricData);
			this.metricDataServiceMock
				.Setup(m => m.GetMetricData(It.Is<IList<int>>(list => list.Contains(arrivalRateSampleHistoryHourId)), serverId,
					CategoryType.UserExperience)).ReturnsAsync(arrivalRateMetricData);


			var sample = new PastWeekEligibleSample
			{
				ServerId = serverId,
				HourId = hourId,
				ArrivalRateHours = sampleArrivalRateHours,
				ConcurrencyHours = sampleConcurrencyHours
			};

			// parameters
			var category = new Category { HourId = hourId };
			var categoryScore = new CategoryScore
			{
				ServerId = serverId,
				Category = category,
				Server = new Server { ArtifactId = serverArtifactId, ServerId = serverId }
			};
			var metricDatas = new List<MetricData>();

			this.userExperienceSampleServiceMock.Setup(m => m.CalculateSample(serverId, hourId)).ReturnsAsync(sample);
			this.userExperienceSampleServiceMock.Setup(m => m.UpdateCurrentSample(sample)).Returns(Task.Delay(1));

			// writing to userExperienceRatings
			this.userExperienceRatingsRepositoryMock.Setup(
					m => m.CreateAsync(serverArtifactId, UXArrivalRateScore, UXConcurrencyScore, hour.Id))
				.Returns(Task.CompletedTask);

			// report data
			this.serverAuditReporter.Setup(r => r.ReportServerAudits(categoryScore.Server, hour, It.IsAny<decimal>()))
				.Returns(Task.Delay(1));
			this.serverAuditReporter.Setup(r => r.FinalizeServerReports(categoryScore.Server, hour))
				.Returns(Task.Delay(1));
			this.serverAuditReporter.Setup(r => r.DeleteServerTempReportData(categoryScore.Server, hour)).Returns(Task.Delay(1));

			// Act
			var result = await this.userExperienceScoringLogic.ScoreMetrics(categoryScore, metricDatas);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
			this.hourRepositoryMock.VerifyAll();
			this.userExperienceSampleServiceMock.VerifyAll();
			this.userExperienceRatingsRepositoryMock.VerifyAll();
		}

		[Test]
		[TestCase(3.0, 1.0, 3.0, 5.0, TestName = "Varied values")]
		[TestCase(1.0, 1.0, 1.0, 1.0, TestName = "Constant values")]
		[TestCase(100.0, 100.0, null, 100.0, TestName = "Supports Null")]
		[TestCase(100.0, TestName = "No metric scores")]
		public async Task ScoreSampleData(double expectedScore, params double?[] metricDataScores)
		{
			var serverId = 3;
			var metricDataDecimals = metricDataScores.Select(s => (decimal?)s);
			var expectedDecimal = (decimal)expectedScore;
			var metricDatas = metricDataDecimals?.Select((score, i) => new MetricData
			{
				Score = score,
				Id = i
			}).ToList();
			var hourIds = Enumerable.Range(1, metricDatas.Count).ToList();

			this.metricDataServiceMock.Setup(m => m.GetMetricData(hourIds, serverId, CategoryType.UserExperience))
				.ReturnsAsync(metricDatas);

			var result = await this.userExperienceScoringLogic.ScoreSampleData(hourIds, serverId);

			Assert.That(result, Is.EqualTo(expectedScore));

		}

		public SampleHistory BuildSampleHistory(int i, int hourId, int serverId)
		{
			return new SampleHistory
			{
				HourId = hourId,
				ServerId = serverId,
				Id = i,
				IsActiveArrivalRateSample = false,
				IsActiveConcurrencySample = false
			};
		}

		public UserExperience BuildMockUserExperience(int i, int hourId, int serverId)
		{
			var pastMetricArrivalRate = 0.1m;
			var pastMetricActiveUsers = 2;
			var pastMetricHasPoisonWaits = false;
			var pastMetricConcurrency = 0.1m;
			return new UserExperience
			{
				ArrivalRate = pastMetricArrivalRate,
				ActiveUsers = pastMetricActiveUsers,
				Concurrency = pastMetricConcurrency,
				HasPoisonWaits = pastMetricHasPoisonWaits
			};
		}
	}
}
