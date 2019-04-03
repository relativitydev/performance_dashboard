namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AuditWorkspaceBatcherTests
	{
		private AuditWorkspaceBatcher auditWorkspaceBatcher;
		private Mock<IHourRepository> hourRepositoryMock;
		private Mock<IWorkspaceAuditServiceFactory> workspaceAuditServiceFactoryMock;
		private Mock<IWorkspaceAuditService> workspaceAuditServiceMock;
		private Mock<ILogger> loggerMock;
		private int serverId;
		private int workspaceId;
		private int hourId;
		private Hour hour;

		[SetUp]
		public void SetUp()
		{
			serverId = 1;
			workspaceId = 1;
			hourId = 2;
			var hourTimeStamp = DateTime.UtcNow;
			hour = new Hour { Id = hourId, HourTimeStamp = hourTimeStamp };
			this.hourRepositoryMock = new Mock<IHourRepository>();
			this.workspaceAuditServiceFactoryMock = new Mock<IWorkspaceAuditServiceFactory>();
			this.workspaceAuditServiceMock = new Mock<IWorkspaceAuditService>();
			this.workspaceAuditServiceFactoryMock.Setup(m => m.GetAuditService(workspaceId, hourId)).ReturnsAsync(this.workspaceAuditServiceMock.Object);
			this.loggerMock = TestUtilities.GetMockLogger();

			this.auditWorkspaceBatcher = new AuditWorkspaceBatcher(this.workspaceAuditServiceFactoryMock.Object, this.loggerMock.Object);
		}

		[Test]
		[TestCase(6666, 2)]
		[TestCase(1000, 1)]
		[TestCase(1, 1)]
		public async Task CreateWorkspaceBatches(long totalAudits, int expectedTotalBatches)
		{
			// Current batchSize defaults to 5000.
			Assert.That(AuditConstants.BatchSize, Is.EqualTo(5000));

			this.workspaceAuditServiceMock.Setup(
				m =>
					m.ReadTotalAuditsForHourAsync(workspaceId, hour.Id,
						new List<AuditActionId> { AuditActionId.DocumentQuery })).ReturnsAsync(totalAudits);

			var result = await this.auditWorkspaceBatcher.CreateWorkspaceBatches(serverId, workspaceId, hourId);

			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Count, Is.EqualTo(expectedTotalBatches));
		}

		[Test]
		[TestCase(100, 10, 10)]
		[TestCase(10, 3, 4)]
		[TestCase(1, 1, 1)]
		public void CreateBatches(long totalAudits, int batchSize, int expectedTotalBatches)
		{
			var serverId = 1;
			var workspaceId = 1;
			var hourId = 2;

			var result = AuditWorkspaceBatcher.CreateBatches(totalAudits, batchSize, serverId, workspaceId, hourId);

			Assert.That(result.Count, Is.EqualTo(expectedTotalBatches));
		}

		[Test]
		public void CreateBatches_DivideByZeroException()
		{
			var serverId = 1;
			var workspaceId = 1;
			var hourId = 2;
			var totalAudits = 0;
			var batchSize = 0;

			Assert.Throws<DivideByZeroException>(() => AuditWorkspaceBatcher.CreateBatches(totalAudits, batchSize, serverId, workspaceId, hourId));
		}
	}
}
