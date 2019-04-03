namespace kCura.PDB.Service.Tests.Logic.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Reports;
	using kCura.PDB.Service.Audits;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ServerAuditReporterTests
	{
		[SetUp]
		public void Setup()
		{
			this.userExperienceReportRepository = new Mock<IUserExperienceReportRepository>();
			this.userExperienceReportRepository.Setup(r => r.CreateServerAuditRecord(It.IsAny<UserExperienceServer>())).Returns(Task.Delay(10));
			this.userExperienceReportRepository.Setup(r => r.CreateVarscatOutput(It.IsAny<Hour>(), It.IsAny<Server>())).Returns(Task.Delay(10));
			this.userExperienceReportRepository.Setup(r => r.CreateVarscatOutputDetails(It.IsAny<Hour>(), It.IsAny<Server>())).Returns(Task.Delay(10));
			this.userExperienceReportRepository.Setup(r => r.UpdateSearchAuditRecord(It.IsAny<Hour>(), It.IsAny<Server>())).Returns(Task.Delay(10));
			this.userExperienceReportRepository.Setup(r => r.DeleteTempReportData(It.IsAny<Hour>(), It.IsAny<Server>())).Returns(Task.Delay(10));
			this.workspaceService = new Mock<IWorkspaceService>();
			this.workspaceAuditService = new Mock<IWorkspaceAuditService>();
			this.workspaceAuditServiceFactory = new Mock<IWorkspaceAuditServiceFactory>();
			this.workspaceAuditServiceFactory.Setup(f => f.GetAuditService(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(workspaceAuditService.Object);
			this.searchAuditBatchRepositoryMock = new Mock<ISearchAuditBatchRepository>();
			this.reporter = new ServerAuditReporter(this.userExperienceReportRepository.Object, this.workspaceService.Object, this.workspaceAuditServiceFactory.Object, this.searchAuditBatchRepositoryMock.Object);
		}

		private IList<int> workspaceIdList;
		private Mock<IUserExperienceReportRepository> userExperienceReportRepository;
		private Mock<IWorkspaceService> workspaceService;
		private Mock<IWorkspaceAuditServiceFactory> workspaceAuditServiceFactory;
		private Mock<IWorkspaceAuditService> workspaceAuditService;
		private Mock<ISearchAuditBatchRepository> searchAuditBatchRepositoryMock;
		private ServerAuditReporter reporter;

		[Test]
		public async Task ServerAuditReporter_ReportServerAudits_EmptyBatchResults()
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();
			this.workspaceIdList = new[] { 1, 2, 3 };
			this.workspaceService.Setup(r => r.ReadAvailableWorkspaceIdsAsync(It.IsAny<int>())).ReturnsAsync(this.workspaceIdList);

			this.workspaceAuditService.Setup(a => a.ReadTotalUniqueUsersForHourAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(1234);
			this.workspaceAuditService.Setup(a => a.ReadTotalAuditsForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(4321);
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadByHourAndServer(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<SearchAuditBatch>());

			// Act
			await reporter.ReportServerAudits(server, hour, 80);

			// Assert
			this.userExperienceReportRepository.Verify(r => r.CreateServerAuditRecord(It.IsAny<UserExperienceServer>()), Times.Exactly(this.workspaceIdList.Count));
		}

		[Test]
		public async Task ServerAuditReporter_ReportServerAudits_NullBatchResult()
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();
			this.workspaceIdList = new[] { 1, 2, 3 };
			this.workspaceService.Setup(r => r.ReadAvailableWorkspaceIdsAsync(It.IsAny<int>())).ReturnsAsync(this.workspaceIdList);

			this.workspaceAuditService.Setup(a => a.ReadTotalUniqueUsersForHourAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(1234);
			this.workspaceAuditService.Setup(a => a.ReadTotalAuditsForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(4321);
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadByHourAndServer(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<SearchAuditBatch> {new SearchAuditBatch {WorkspaceId = 1} });

			// Act
			await reporter.ReportServerAudits(server, hour, 80);

			// Assert
			this.userExperienceReportRepository.Verify(r => r.CreateServerAuditRecord(It.IsAny<UserExperienceServer>()), Times.Exactly(this.workspaceIdList.Count));
		}

		[Test]
		public async Task ServerAuditReporter_ReportServerAudits_NotNull()
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();
			this.workspaceIdList = new[] { 1, 2, 3 };
			this.workspaceService.Setup(r => r.ReadAvailableWorkspaceIdsAsync(It.IsAny<int>())).ReturnsAsync(this.workspaceIdList);

			this.workspaceAuditService.Setup(a => a.ReadTotalUniqueUsersForHourAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(1234);
			this.workspaceAuditService.Setup(a => a.ReadTotalAuditsForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(4321);
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadByHourAndServer(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<SearchAuditBatch> { new SearchAuditBatch { WorkspaceId = 1, BatchResults = new List<SearchAuditBatchResult> {new SearchAuditBatchResult {TotalLongRunningQueries = 3} } } });

			// Act
			await reporter.ReportServerAudits(server, hour, 80);

			// Assert
			this.userExperienceReportRepository.Verify(r => r.CreateServerAuditRecord(It.IsAny<UserExperienceServer>()), Times.Exactly(this.workspaceIdList.Count));
		}

		[Test]
		public async Task ServerAuditReporter_FinalizeServerReports()
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();

			// Act
			await reporter.FinalizeServerReports(server, hour);

			// Assert
			this.userExperienceReportRepository.Verify(r => r.CreateVarscatOutput(It.IsAny<Hour>(), It.IsAny<Server>()), Times.Once);
			this.userExperienceReportRepository.Verify(r => r.CreateVarscatOutputDetails(It.IsAny<Hour>(), It.IsAny<Server>()), Times.Once);
			this.userExperienceReportRepository.Verify(r => r.UpdateSearchAuditRecord(It.IsAny<Hour>(), It.IsAny<Server>()), Times.Once);
		}

		[Test]
		public async Task ServerAuditReporter_DeleteServerTempReportData()
		{
			// Arrange
			var server = new Server();
			var hour = new Hour();

			// Act
			await reporter.DeleteServerTempReportData(server, hour);

			// Assert
			this.userExperienceReportRepository.Verify(r => r.DeleteTempReportData(It.IsAny<Hour>(), It.IsAny<Server>()), Times.Once);
		}
	}
}
