namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class AuditBatchProcessorTests
	{
		private AuditBatchProcessor auditBatchProcessor;
		private Mock<ISearchAuditBatchRepository> searchAuditBatchRepositoryMock;
		private Mock<IHourRepository> hourRepositoryMock;
		private Mock<ISearchAnalysisService> searchAnalysisServiceMock;
		private Mock<IAuditBatchAnalyzer> auditBatchAnalyzerMock;
		private Mock<IWorkspaceAuditServiceFactory> workspaceAuditServiceFactoryMock;
		private Mock<IWorkspaceAuditService> workspaceAuditServiceMock;
		private Mock<IWorkspaceAuditReporter> workspaceAuditReporter;
		private Mock<IServerRepository> serverRepositoryMock;
		private Mock<IAuditParsingService> auditParsingServiceMock;
		private Mock<IWorkspaceService> workspaceService;
		private Mock<ILogger> loggerMock;

		private int workspaceId;
		private int hourId;
		private Hour hour;

		[SetUp]
		public void Setup()
		{
			workspaceId = 0;
			hourId = 2;
			hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };

			this.searchAuditBatchRepositoryMock = new Mock<ISearchAuditBatchRepository>();
			this.hourRepositoryMock = new Mock<IHourRepository>();
			this.searchAnalysisServiceMock = new Mock<ISearchAnalysisService>();
			this.auditBatchAnalyzerMock = new Mock<IAuditBatchAnalyzer>();
			this.workspaceAuditServiceFactoryMock = new Mock<IWorkspaceAuditServiceFactory>();
			this.workspaceAuditServiceMock = new Mock<IWorkspaceAuditService>();
			this.workspaceAuditServiceFactoryMock.Setup(m => m.GetAuditService(workspaceId, hour.Id))
				.ReturnsAsync(this.workspaceAuditServiceMock.Object);
			this.workspaceAuditReporter = new Mock<IWorkspaceAuditReporter>();
			this.serverRepositoryMock = new Mock<IServerRepository>();
			this.auditParsingServiceMock = new Mock<IAuditParsingService>();
			this.workspaceService = new Mock<IWorkspaceService>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.auditBatchProcessor = new AuditBatchProcessor(
				this.searchAuditBatchRepositoryMock.Object,
				this.hourRepositoryMock.Object,
				this.searchAnalysisServiceMock.Object,
				this.auditBatchAnalyzerMock.Object,
				this.workspaceAuditServiceFactoryMock.Object,
				this.workspaceAuditReporter.Object,
				this.serverRepositoryMock.Object,
				this.auditParsingServiceMock.Object,
				this.workspaceService.Object,
				this.loggerMock.Object);
		}

		[Test]
		public async Task ProcessBatch()
		{
			// Arrange
			var batchId = 1;
			var batchSize = 50;
			var batchStart = 0;
			var batch = new SearchAuditBatch
			{
				Id = batchId,
				WorkspaceId = workspaceId,
				BatchSize = batchSize,
				BatchStart = batchStart,
				ServerId = 123,
				HourId = this.hourId
			};
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadBatch(batch.Id))
				.Returns(batch);

			this.serverRepositoryMock.Setup(m => m.ReadWorkspaceExistsAsync(workspaceId)).ReturnsAsync(true);
			this.hourRepositoryMock.Setup(m => m.ReadAsync(batch.HourId)).ReturnsAsync(hour);

			var audit = new Audit();
			var audits = new List<Audit> { audit };
			this.workspaceAuditServiceMock.Setup(
				m => m.ReadAuditsAsync(
					batch.WorkspaceId,
					hour.Id,
					It.Is<IList<AuditActionId>>(a => a.Contains(AuditActionId.DocumentQuery)),
					batch.BatchSize,
					batch.BatchStart))
				.ReturnsAsync(audits);

			var queryId = (string)null;
			this.auditParsingServiceMock.Setup(m => m.ParseQueryId(audit.Details)).Returns(queryId);

			var searchAudit = new SearchAudit { Search = new Search(), IsComplex = true };
			var startSearchAudits = new SearchAuditGroup { Audits = new List<SearchAudit> { searchAudit } };
			var endSearchAudits = new SearchAuditGroup { Audits = new List<SearchAudit> { searchAudit } };
			var expectedSearchAudits = new List<SearchAudit> { searchAudit };
			var expectedSearchAuditGroups = new List<SearchAuditGroup> { new SearchAuditGroup { Audits = expectedSearchAudits } };
			this.searchAnalysisServiceMock.Setup(m => m.AnalyzeSearchAudit(startSearchAudits)).ReturnsAsync(endSearchAudits);
			expectedSearchAudits[0].IsComplex = searchAudit.IsComplex;

			var auditResult = new SearchAuditBatchResult { BatchId = batchId };
			var auditResults = new List<SearchAuditBatchResult> { auditResult };
			this.auditBatchAnalyzerMock.Setup(m => m.GetBatchResults(It.IsAny<IList<SearchAuditGroup>>(), batchId)).Returns(auditResults);

			this.workspaceAuditReporter.Setup(r => r.ReportWorkspaceAudits(It.IsAny<IList<SearchAuditGroup>>(), It.IsAny<Hour>(), It.IsAny<int>()))
				.Returns(Task.FromResult(1));

			this.searchAuditBatchRepositoryMock.Setup(r => r.CreateBatchResults(It.IsAny<IList<SearchAuditBatchResult>>()))
				.Returns(new[] { 123456 });

			this.searchAuditBatchRepositoryMock.Setup(r => r.UpdateAsync(batch))
				.Returns(Task.FromResult(1));

			this.workspaceService.Setup(s => s.WorkspaceIsAvailableAsync(workspaceId))
				.ReturnsAsync(true);

			// Act
			var result = await this.auditBatchProcessor.ProcessBatch(batchId);

			// Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Count, Is.EqualTo(auditResults.Count));

		}

		[Test]
		public async Task ProcessBatch_DeletedWorkspace()
		{
			// Arrange
			var batchId = 1;
			var batchSize = 50;
			var batchStart = 0;
			var batch = new SearchAuditBatch
			{
				Id = batchId,
				WorkspaceId = workspaceId,
				BatchSize = batchSize,
				BatchStart = batchStart,
				ServerId = 123,
				HourId = this.hourId
			};
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadBatch(batch.Id)).Returns(batch);
			this.serverRepositoryMock.Setup(m => m.ReadWorkspaceExistsAsync(workspaceId)).ReturnsAsync(false);
			this.searchAuditBatchRepositoryMock.Setup(r => r.UpdateAsync(batch))
				.Returns(Task.FromResult(1));

			// Act
			var result = await this.auditBatchProcessor.ProcessBatch(batchId);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task ProcessBatch_NullQuery()
		{
			// Arrange
			var batchId = 1;
			var batchSize = 50;
			var batchStart = 0;
			var batch = new SearchAuditBatch
			{
				Id = batchId,
				WorkspaceId = workspaceId,
				BatchSize = batchSize,
				BatchStart = batchStart,
				ServerId = 123,
				HourId = this.hourId
			};
			this.searchAuditBatchRepositoryMock.Setup(m => m.ReadBatch(batch.Id)).Returns(batch);

			this.serverRepositoryMock.Setup(m => m.ReadWorkspaceExistsAsync(workspaceId)).ReturnsAsync(true);
			this.hourRepositoryMock.Setup(m => m.ReadAsync(batch.HourId)).ReturnsAsync(hour);

			var audit = new Audit();
			var audits = new List<Audit> { audit };
			this.workspaceAuditServiceMock.Setup(
				m =>
					m.ReadAuditsAsync(batch.WorkspaceId, hour.Id,
						It.Is<IList<AuditActionId>>(a => a.Contains(AuditActionId.DocumentQuery)), batch.BatchSize, batch.BatchStart)).ReturnsAsync(audits);

			var queryId = (string)null;
			this.auditParsingServiceMock.Setup(m => m.ParseQueryId(audit.Details)).Returns(queryId);

			var searchAudit = new SearchAudit { Search = new Search(), IsComplex = false };
			var startSearchAudits = new SearchAuditGroup { Audits = new List<SearchAudit> { searchAudit } };
			var endSearchAudits = new SearchAuditGroup { Audits = new List<SearchAudit> { searchAudit } };
			var expectedSearchAudits = new List<SearchAudit> { searchAudit };
			var expectedSearchAuditGroups = new List<SearchAuditGroup> { new SearchAuditGroup { Audits = expectedSearchAudits } };
			this.searchAnalysisServiceMock.Setup(m => m.AnalyzeSearchAudit(startSearchAudits)).ReturnsAsync(endSearchAudits);
			expectedSearchAudits[0].IsComplex = searchAudit.IsComplex;

			var auditResult = new SearchAuditBatchResult { BatchId = batchId };
			var auditResults = new List<SearchAuditBatchResult> { auditResult };
			this.auditBatchAnalyzerMock.Setup(m => m.GetBatchResults(It.IsAny<IList<SearchAuditGroup>>(), batchId)).Returns(auditResults);

			this.workspaceAuditReporter.Setup(r => r.ReportWorkspaceAudits(It.IsAny<IList<SearchAuditGroup>>(), It.IsAny<Hour>(), It.IsAny<int>()))
				.Returns(Task.FromResult(1));

			this.searchAuditBatchRepositoryMock.Setup(r => r.CreateBatchResults(It.IsAny<IList<SearchAuditBatchResult>>()))
				.Returns(new[] { 123456 });

			this.searchAuditBatchRepositoryMock.Setup(r => r.UpdateAsync(batch))
				.Returns(Task.FromResult(1));

			this.workspaceService.Setup(s => s.WorkspaceIsAvailableAsync(workspaceId))
				.ReturnsAsync(true);

			// Act
			var result = await this.auditBatchProcessor.ProcessBatch(batchId);

			// Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Count, Is.EqualTo(auditResults.Count));
		}
	}
}
