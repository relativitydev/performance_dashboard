namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AuditServiceTests
	{
		private ServerAuditService auditService;
		private Mock<IWorkspaceService> workspaceService;
		private Mock<IHourRepository> hourRepositoryMock;
		private Mock<IWorkspaceAuditServiceFactory> workspaceAuditServiceFactory;
		private Mock<IWorkspaceAuditService> workspaceAuditService;
		private Mock<ILogger> logger;
		private int serverId;

		[SetUp]
		public void SetUp()
		{
			this.workspaceService = new Mock<IWorkspaceService>();
			this.hourRepositoryMock = new Mock<IHourRepository>();
			this.workspaceAuditServiceFactory = new Mock<IWorkspaceAuditServiceFactory>();
			this.workspaceAuditService = new Mock<IWorkspaceAuditService>();
			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(this.workspaceAuditService.Object);
			this.logger = TestUtilities.GetMockLogger();
			this.auditService = new ServerAuditService(this.workspaceService.Object, this.workspaceAuditServiceFactory.Object, this.logger.Object);

			this.serverId = 1;
		}

		[Test]
		public async Task ReadTotalAuditsForHourAsync()
		{
			var hourId = 2;
			var actionTypes = new List<AuditActionId> {AuditActionId.DocumentQuery};

			var hourTimeStamp = DateTime.UtcNow;
			var hour = new Hour {HourTimeStamp = hourTimeStamp, Id = hourId};
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			var workspaceIdA = 3;
			var workspaceIdB = 4;
			var workspaceIds = new List<int> { workspaceIdA, workspaceIdB };
			this.workspaceService.Setup(m => m.ReadAvailableWorkspaceIdsAsync(this.serverId)).ReturnsAsync(workspaceIds);

			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdA, hourId)).ReturnsAsync(this.workspaceAuditService.Object);
			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdB, hourId)).ReturnsAsync(this.workspaceAuditService.Object);

			var auditsForWorkspaceA = 4;
			var auditsForWorkspaceB = 5;
			var totalAudits = auditsForWorkspaceA + auditsForWorkspaceB;

			this.workspaceAuditService.Setup(
					m => m.ReadTotalAuditsForHourAsync(workspaceIdA, hour.Id, actionTypes))
				.ReturnsAsync(auditsForWorkspaceA);
			this.workspaceAuditService.Setup(
					m => m.ReadTotalAuditsForHourAsync(workspaceIdB, hour.Id, actionTypes))
				.ReturnsAsync(auditsForWorkspaceB);
			
			var result = await this.auditService.ReadTotalAuditsForHourAsync(this.serverId, hourId, actionTypes);

			Assert.That(result, Is.EqualTo(totalAudits));
		}

		[Test]
		public async Task ReadTotalUniqueUsersForHourAuditsAsync()
		{
			var hourId = 2;
			var actionTypes = new List<AuditActionId> { AuditActionId.DocumentQuery };

			var hourTimeStamp = DateTime.UtcNow;
			var hour = new Hour { HourTimeStamp = hourTimeStamp, Id = hourId };
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			var workspaceIdA = 3;
			var workspaceIdB = 4;
			var workspaceIds = new List<int> { workspaceIdA, workspaceIdB };
			this.workspaceService.Setup(m => m.ReadAvailableWorkspaceIdsAsync(this.serverId)).ReturnsAsync(workspaceIds);

			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdA, hourId)).ReturnsAsync(this.workspaceAuditService.Object);
			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdB, hourId)).ReturnsAsync(this.workspaceAuditService.Object);


			var uniqueUsersA = new List<int> {1234, 123};
			var uniqueUsersB = new List<int> {1234, 567};
			var allUniqueUsers = new List<int>();
			allUniqueUsers.AddRange(uniqueUsersA);
			allUniqueUsers.AddRange(uniqueUsersB);
			var finalExpectedUniqueUsers = allUniqueUsers.Distinct().Count();
			
			this.workspaceAuditService.Setup(
					m => m.ReadUniqueUsersForHourAuditsAsync(workspaceIdA, hour.Id, actionTypes))
				.ReturnsAsync(uniqueUsersA);
			this.workspaceAuditService.Setup(
					m => m.ReadUniqueUsersForHourAuditsAsync(workspaceIdB, hour.Id, actionTypes))
				.ReturnsAsync(uniqueUsersB);

			var result = await this.auditService.ReadTotalUniqueUsersForHourAuditsAsync(this.serverId, hourId, actionTypes);

			Assert.That(result, Is.EqualTo(finalExpectedUniqueUsers));
		}

		[Test]
		[Ignore("Cannot use reliably until DataGridAudit API implementation exists")]
		[Category("Ignore")]
		public async Task ReadTotalAuditExectionTimeForHour()
		{
			var hourId = 2;
			var actionTypes = new List<AuditActionId> { AuditActionId.DocumentQuery };

			var hourTimeStamp = DateTime.UtcNow;
			var hour = new Hour { HourTimeStamp = hourTimeStamp, Id = hourId };
			this.hourRepositoryMock.Setup(m => m.ReadAsync(hourId)).ReturnsAsync(hour);

			var workspaceIdA = 3;
			var workspaceIdB = 4;
			var workspaceIds = new List<int> { workspaceIdA, workspaceIdB };
			this.workspaceService.Setup(m => m.ReadAvailableWorkspaceIdsAsync(this.serverId)).ReturnsAsync(workspaceIds);

			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdA, hourId)).ReturnsAsync(this.workspaceAuditService.Object);
			this.workspaceAuditServiceFactory.Setup(m => m.GetAuditService(workspaceIdB, hourId)).ReturnsAsync(this.workspaceAuditService.Object);

			var executionTimeA = 400;
			var executionTimeB = 300;
			var totalExecutionTime = executionTimeA + executionTimeB;
			
			this.workspaceAuditService.Setup(
					m => m.ReadTotalAuditExecutionTimeForHourAsync(workspaceIdA, hour.Id, actionTypes))
				.ReturnsAsync(executionTimeA);
			this.workspaceAuditService.Setup(
					m => m.ReadTotalAuditExecutionTimeForHourAsync(workspaceIdB, hour.Id, actionTypes))
				.ReturnsAsync(executionTimeB);

			var result = await this.auditService.ReadTotalAuditExecutionTimeForHourAsync(this.serverId, hourId, actionTypes);

			Assert.That(result, Is.EqualTo(totalExecutionTime));
		}
	}
}
