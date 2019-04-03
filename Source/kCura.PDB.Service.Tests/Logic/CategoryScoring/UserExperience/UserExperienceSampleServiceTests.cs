namespace kCura.PDB.Service.Tests.Logic.CategoryScoring.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
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
	public class UserExperienceSampleServiceTests
	{
		[SetUp]
		public void SetUp()
		{
			this.server = new Server { ServerId = 4 };
			this.hour = new Hour { Id = 3, HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			this.userExperienceServiceMock = new Mock<IUserExperienceService>();
			this.hourRespositoryMock = new Mock<IHourRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.userExperienceCacheRepositoryMock = new Mock<IUserExperienceCacheRepository>();
			this.sampleHistoryRepositoryMock = new Mock<ISampleHistoryRepository>();
			this.serverRepositoryMock = new Mock<IServerRepository>();
			this.userExperienceSampleService = new UserExperienceSampleService(
				this.userExperienceServiceMock.Object,
				this.hourRespositoryMock.Object,
				this.loggerMock.Object,
				this.userExperienceCacheRepositoryMock.Object,
				this.sampleHistoryRepositoryMock.Object,
				this.serverRepositoryMock.Object);
		}

		private Mock<IUserExperienceService> userExperienceServiceMock;
		private Mock<IHourRepository> hourRespositoryMock;
		private Mock<ILogger> loggerMock;
		private Mock<IUserExperienceCacheRepository> userExperienceCacheRepositoryMock;
		private Mock<ISampleHistoryRepository> sampleHistoryRepositoryMock;
		private Mock<IServerRepository> serverRepositoryMock;
		private UserExperienceSampleService userExperienceSampleService;
		private Server server;
		private Hour hour;

		[Test]
		public async Task GetPastWeekUserExperienceMetricDataAsync()
		{
			// Arrange
			var pastWeekHours = new List<Hour>();
			this.hourRespositoryMock.Setup(m => m.ReadPastWeekHoursAsync(this.hour))
				.ReturnsAsync(pastWeekHours);

			foreach (var h in pastWeekHours)
			{
				var userExperience = new UserExperience
				{
					ActiveUsers = 3,
					ArrivalRate = 1.0m,
					Concurrency = 1.0m,
					HasPoisonWaits = false,
					ServerId = this.server.ServerId,
					HourId = h.Id,
				};
				this.userExperienceServiceMock.Setup(m => m.BuildUserExperienceModel(this.server.ServerId, h))
					.ReturnsAsync(userExperience); // Needs to define hour Id
			}

			// Act
			var results = await this.userExperienceSampleService.GetPastWeekUserExperienceMetricDataAsync(this.server.ServerId, this.hour);

			// Assert
			this.hourRespositoryMock.VerifyAll();
			this.userExperienceServiceMock.VerifyAll();
		}

		[Test]
		public void DetermineEligibleArrivalRateSample_AtThreshold()
		{
			// Arrange
			var count = Defaults.Scores.ArrivalRateCountThreshold;
			var pastWeekTestData = this.GenerateUserExperienceData(count);

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleArrivalRateSample(pastWeekTestData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(count));
		}

		[Test]
		public void DetermineEligibleArrivalRateSample_AboveThreshold()
		{
			// Arrange
			var count = Defaults.Scores.ArrivalRateCountThreshold + 1;
			var pastWeekTestData = this.GenerateUserExperienceData(count);

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleArrivalRateSample(pastWeekTestData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(count));
		}

		[Test]
		public void DetermineEligibleConcurrencySample_AtThreshold()
		{
			// Arrange
			var count = Defaults.Scores.ConcurrencyCountThreshold;
			var pastWeekTestData = this.GenerateUserExperienceData(count);

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleConcurrencySample(pastWeekTestData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(count));
		}

		[Test]
		public void DetermineEligibleConcurrencySample_AboveThreshold()
		{
			// Arrange
			var count = Defaults.Scores.ConcurrencyCountThreshold + 1;
			var pastWeekTestData = this.GenerateUserExperienceData(count);

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleConcurrencySample(pastWeekTestData);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(count));
		}

		[Test]
		public async Task CalculateSample_OneHour_WithCache()
		{
			var serverId = 3;
			var hourId = 2;
			var hour = new Hour { Id = hourId };
			var pastWeekHourId = 1;
			var pastWeekHour = new Hour { Id = pastWeekHourId };
			var pastWeekHours = new List<Hour> { pastWeekHour };

			this.hourRespositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);
			this.hourRespositoryMock.Setup(m => m.ReadPastWeekHoursAsync(hour)).ReturnsAsync(pastWeekHours);

			// read cache
			var userExperienceModel = new UserExperience() { HourId = pastWeekHourId };
			var userExperienceModels = new List<UserExperience> { userExperienceModel };
			this.userExperienceCacheRepositoryMock
				.Setup(m => m.ReadAsync(serverId, pastWeekHour.HourTimeStamp, pastWeekHour.HourTimeStamp))
				.ReturnsAsync(userExperienceModels);

			var result = await this.userExperienceSampleService.CalculateSample(serverId, hourId);
			Assert.That(result.HourId, Is.EqualTo(hourId));
			Assert.That(result.ServerId, Is.EqualTo(serverId));
			Assert.That(result.ArrivalRateHours, Is.Empty);
			Assert.That(result.ConcurrencyHours, Is.Empty);
		}

		[Test]
		public async Task CalculateSample_OneHour_NoCache()
		{
			var serverId = 3;
			var hourId = 2;

			var hour = new Hour();
			var pastWeekHour = new Hour();
			var pastWeekHours = new List<Hour> { pastWeekHour };

			// read hours
			this.hourRespositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);
			this.hourRespositoryMock.Setup(m => m.ReadPastWeekHoursAsync(hour)).ReturnsAsync(pastWeekHours);

			// read empty cache
			var userExperienceCacheModels = new List<UserExperience>();
			this.userExperienceCacheRepositoryMock
				.Setup(m => m.ReadAsync(serverId, pastWeekHour.HourTimeStamp, pastWeekHour.HourTimeStamp))
				.ReturnsAsync(userExperienceCacheModels);

			// Build ux models
			var userExperienceModel = new UserExperience();
			this.userExperienceServiceMock.Setup(m => m.BuildUserExperienceModel(serverId, pastWeekHour))
				.ReturnsAsync(userExperienceModel);
			this.userExperienceCacheRepositoryMock.Setup(m => m.CreateAsync(userExperienceModel))
				.ReturnsAsync(userExperienceModel);

			// Determine eligibility - None

			var result = await this.userExperienceSampleService.CalculateSample(serverId, hourId);
			Assert.That(result.HourId, Is.EqualTo(hourId));
			Assert.That(result.ServerId, Is.EqualTo(serverId));
			Assert.That(result.ArrivalRateHours, Is.Empty);
			Assert.That(result.ConcurrencyHours, Is.Empty);
		}

		[Test]
		public async Task UpdateCurrentSample_NoEddsStandalone()
		{
			// Arrange
			var sample = new PastWeekEligibleSample()
			{
				ServerId = 1,
				HourId = 2,
				ArrivalRateHours = new[] { new SampleHistory() },
				ConcurrencyHours = new[] { new SampleHistory() }
			};

			// No Standalone
			this.serverRepositoryMock.Setup(m => m.ReadPrimaryStandaloneAsync()).ReturnsAsync((int?)null);

			this.sampleHistoryRepositoryMock.Setup(m => m.ResetCurrentSampleAsync(sample.ServerId)).Returns(Task.Delay(1));
			this.sampleHistoryRepositoryMock.Setup(m => m.AddToCurrentSampleAsync(sample.ArrivalRateHours))
				.Returns(Task.Delay(1));
			this.sampleHistoryRepositoryMock.Setup(m => m.AddToCurrentSampleAsync(sample.ConcurrencyHours))
				.Returns(Task.Delay(1));

			// Act
			await this.userExperienceSampleService.UpdateCurrentSample(sample);

			// Assert
			this.serverRepositoryMock.VerifyAll();
			this.sampleHistoryRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task UpdateCurrentSample_EddsStandalone()
		{
			// Arrange
			var sample = new PastWeekEligibleSample()
			{
				ServerId = 1,
				HourId = 2,
				ArrivalRateHours = new[] { new SampleHistory() },
				ConcurrencyHours = new[] { new SampleHistory() }
			};

			// Edds is standalone
			var eddsStandaloneArtifactId = 123;
			this.serverRepositoryMock.Setup(m => m.ReadPrimaryStandaloneAsync()).ReturnsAsync(eddsStandaloneArtifactId);

			this.sampleHistoryRepositoryMock.Setup(m => m.ResetCurrentSampleAsync(sample.ServerId)).Returns(Task.Delay(1));
			this.sampleHistoryRepositoryMock.Setup(m => m.AddToCurrentSampleAsync(sample.ArrivalRateHours))
				.Returns(Task.Delay(1));
			this.sampleHistoryRepositoryMock.Setup(m => m.AddToCurrentSampleAsync(sample.ConcurrencyHours))
				.Returns(Task.Delay(1));
			this.sampleHistoryRepositoryMock.Setup(m =>
					m.AddToCurrentSampleAsync(It.Is<List<SampleHistory>>(sh =>
						sh.Exists(h => h.ServerId == eddsStandaloneArtifactId))))
				.Returns(Task.Delay(1));

			// Act
			await this.userExperienceSampleService.UpdateCurrentSample(sample);

			// Assert
			this.serverRepositoryMock.VerifyAll();
			this.sampleHistoryRepositoryMock.VerifyAll();
		}

		private List<UserExperience> GenerateUserExperienceData(int count)
		{
			return Enumerable.Range(1, count).Select(i => new UserExperience
			{
				ActiveUsers = 2,
				ArrivalRate = 1,
				Concurrency = 1,
				HasPoisonWaits = true,
				HourId = 4,
				ServerId = 4,
			}).ToList();
		}
	}
}
