namespace kCura.PDB.Service.Tests
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.MetricDataSources;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class UserExperienceSamplingTests
	{
		private Mock<IServerAuditService> serverAuditServiceMock;
		private Mock<IPoisonWaitRepository> poisonWaitRepositoryMock;
		private Mock<ISearchAuditBatchRepository> searchAuditBatchRepositoryMock;
		private Mock<ILogger> loggerMock;
		private Mock<IUserExperienceCacheRepository> userExperienceCacheRepositoryMock;
		private Mock<ISampleHistoryRepository> sampleHistoryRepositoryMock;
		private Mock<IServerRepository> serverRepositoryMock;
		private UserExperienceService userExperienceService;

		private Mock<IHourRepository> hourRepositoryMock;
		private UserExperienceSampleService userExperienceSampleService;

		[SetUp]
		public void SetUp()
		{
			this.serverAuditServiceMock = new Mock<IServerAuditService>();
			this.poisonWaitRepositoryMock = new Mock<IPoisonWaitRepository>();
			this.searchAuditBatchRepositoryMock = new Mock<ISearchAuditBatchRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.userExperienceService = new UserExperienceService(
				this.serverAuditServiceMock.Object,
				this.poisonWaitRepositoryMock.Object,
				this.searchAuditBatchRepositoryMock.Object,
				this.loggerMock.Object);
			this.userExperienceCacheRepositoryMock = new Mock<IUserExperienceCacheRepository>();
			this.hourRepositoryMock = new Mock<IHourRepository>();
			this.sampleHistoryRepositoryMock = new Mock<ISampleHistoryRepository>();
			this.serverRepositoryMock = new Mock<IServerRepository>();
			this.userExperienceSampleService = new UserExperienceSampleService(
				this.userExperienceService,
				this.hourRepositoryMock.Object,
				this.loggerMock.Object,
				this.userExperienceCacheRepositoryMock.Object,
				this.sampleHistoryRepositoryMock.Object,
				this.serverRepositoryMock.Object);
		}

		[Test]
		[TestCase(181, true, TestName = "Past threshold")]
		[TestCase(180, false, TestName = "At threshold")]
		public void DetermineEligibleArrivalRateSample(int totalQueriesForHour, bool expectedResult)
		{
			// Arrange
			var arrivalRate = this.userExperienceService.CalculateFinalArrivalRate(totalQueriesForHour);
			var userExperienceTest = new UserExperience
			{
				ActiveUsers = 2,
				ArrivalRate = arrivalRate,
				Concurrency = 1000,
				HasPoisonWaits = false
			};
			var userExperienceData = new List<UserExperience> { userExperienceTest };

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleArrivalRateSample(userExperienceData);

			// Assert
			Assert.That(result.Any(), Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(181, 1000, true, TestName = "Past threshold")]
		[TestCase(180, 1000, false, TestName = "At threshold")]
		public void DetermineEligibleConcurrencySample(int totalQueriesForHour, long totalExecutionTimeForAllAudits, bool expectedResult)
		{
			// Arrange
			var arrivalRate = this.userExperienceService.CalculateFinalArrivalRate(totalQueriesForHour);
			var concurrency = this.userExperienceService.CalculateFinalConcurrency(totalExecutionTimeForAllAudits);
			var userExperienceTest = new UserExperience
			{
				ActiveUsers = 2,
				ArrivalRate = arrivalRate,
				Concurrency = concurrency,
				HasPoisonWaits = false
			};
			var userExperienceData = new List<UserExperience> { userExperienceTest };

			// Act
			var result = this.userExperienceSampleService.DetermineEligibleConcurrencySample(userExperienceData);

			// Assert
			Assert.That(result.Any(), Is.EqualTo(expectedResult));
		}
	}
}
