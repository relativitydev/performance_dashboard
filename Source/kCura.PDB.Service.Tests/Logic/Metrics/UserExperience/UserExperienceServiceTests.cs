namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class UserExperienceServiceTests
	{
		private UserExperienceService userExperienceService;
		private Mock<IServerAuditService> auditServiceMock;
		private Mock<IPoisonWaitRepository> poisonWaitRepositoryMock;
		private Mock<ISearchAuditBatchRepository> searchAuditBatchRepositoryMock;
		private Mock<ILogger> loggerMock;

		[SetUp]
		public void SetUp()
		{
			this.auditServiceMock = new Mock<IServerAuditService>();
			this.poisonWaitRepositoryMock = new Mock<IPoisonWaitRepository>();
			this.searchAuditBatchRepositoryMock = new Mock<ISearchAuditBatchRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.userExperienceService = new UserExperienceService(
				this.auditServiceMock.Object,
				this.poisonWaitRepositoryMock.Object,
				this.searchAuditBatchRepositoryMock.Object,
				this.loggerMock.Object);
		}

		[Test]
		[TestCase(2, true, true, 3600, 1, 0, 0, 3600000, 1)]
		public async Task BuildUserExperienceModel(
			int totalUniqueUsers,
			bool hasPoisonWait,
			bool poisonWaitValue,
			long totalQueries,
			decimal expectedArrivalRate,
			long totalAudits3456,
			long totalAudits47,
			long totalExecutionTimeAuditsOther,
			decimal expectedConcurrency)
		{
			// Arrange
			var serverId = 1;
			var hour = new Hour { Id = 2 };
			this.auditServiceMock.Setup(
					m => m.ReadTotalUniqueUsersForHourAuditsAsync(serverId, hour.Id, AuditConstants.RelevantAuditActionIds))
				.ReturnsAsync(totalUniqueUsers);
			this.poisonWaitRepositoryMock.Setup(m => m.ReadIfPoisonWaitsForHourAsync(It.IsAny<Hour>())).ReturnsAsync(hasPoisonWait);
			this.poisonWaitRepositoryMock.Setup(m => m.ReadPoisonWaitsForHourAsync(It.IsAny<Hour>(), serverId))
				.ReturnsAsync(poisonWaitValue);
			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hour.Id, AuditConstants.RelevantAuditActionIds))
				.ReturnsAsync(totalQueries);
			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hour.Id, AuditConstants.Audits3456))
				.ReturnsAsync(totalAudits3456);

			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hour.Id, new List<AuditActionId> { AuditActionId.UpdateImport }))
				.ReturnsAsync(totalAudits47);
			var searchAuditBatchResults = new List<SearchAuditBatch>
			{
				new SearchAuditBatch
				{
					BatchResults =
						new List<SearchAuditBatchResult>()
						{
							new SearchAuditBatchResult {TotalExecutionTime = totalExecutionTimeAuditsOther}
						}
				}
			};

			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadByHourAndServer(hour.Id, serverId))
				.Returns(searchAuditBatchResults);

			// Act
			var result = await this.userExperienceService.BuildUserExperienceModel(serverId, hour);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ActiveUsers, Is.EqualTo(totalUniqueUsers));
			Assert.That(result.HasPoisonWaits, Is.EqualTo(hasPoisonWait));
			Assert.That(result.ArrivalRate, Is.EqualTo(expectedArrivalRate));
			Assert.That(result.Concurrency, Is.EqualTo(expectedConcurrency).Within(0.0000001m));
			Assert.That(result.HourId, Is.EqualTo(hour.Id));
			Assert.That(result.ServerId, Is.EqualTo(serverId));
		}

		[Test]
		[TestCase(3600, 1)]
		public async Task CalculateArrivalRate(long totalQueries, decimal expectedResult)
		{
			var serverId = 1;
			var hourId = 2;
			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hourId, AuditConstants.RelevantAuditActionIds))
				.ReturnsAsync(totalQueries);

			var result = await this.userExperienceService.CalculateArrivalRate(serverId, hourId);

			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(3600, 1)]
		[TestCase(180, 0.05)] // Minimum threshold for arrival rate
		public void CalculateFinalArrivalRate(long totalQueries, decimal expectedResult)
		{
			var result = this.userExperienceService.CalculateFinalArrivalRate(totalQueries);
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(1000, 1000, 1000, 0.00361111111)]
		[TestCase(0, 0, 3600000, 1)]
		public async Task CalculateConcurrency(long totalAudits3456, long totalAudits47, long totalExecutionTimeAuditsOther, decimal expectedResult)
		{
			// Arrange
			var serverId = 1;
			var hourId = 2;

			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hourId, AuditConstants.Audits3456))
				.ReturnsAsync(totalAudits3456);
			this.auditServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(serverId, hourId, new List<AuditActionId> { AuditActionId.UpdateImport }))
				.ReturnsAsync(totalAudits47);

			var searchAuditBatchResults = new List<SearchAuditBatch>
			{
				new SearchAuditBatch
				{
					BatchResults =
						new List<SearchAuditBatchResult>()
						{
							new SearchAuditBatchResult {TotalExecutionTime = totalExecutionTimeAuditsOther}
						}
				}
			};

			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadByHourAndServer(hourId, serverId))
				.Returns(searchAuditBatchResults);

			// Act
			var result = await this.userExperienceService.CalculateConcurrency(serverId, hourId);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult).Within((0.0000001m)));
		}

		[Test]
		[TestCase(3600000, 1)]
		public void CalculateFinalConcurrency(long totalExecutionTime, decimal expectedResult)
		{
			// Arrange
			// Act
			var result = this.userExperienceService.CalculateFinalConcurrency(totalExecutionTime);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
