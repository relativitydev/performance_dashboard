namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class WorkspaceAuditServiceFactoryTests
	{
		private Mock<IRelativityApplicationRepository> relativityAppliationRepository;
		private Mock<IDataGridWorkspaceAuditService> dataGridWorkspaceAuditService;
		private Mock<IDataGridCacheRepository> dataGridCacheRepository;
		private Mock<IToggleProvider> toggleProvider;
		private Mock<ILogger> logger;
		private Mock<IDataGridCacheRepository> dataGridCacheRepositoryMock;
		private Mock<IHourRepository> hourRepositoryMock;
		private WorkspaceAuditServiceFactory workspaceAuditServiceFactory;

		private int workspaceId;
		private int startHourId;

		private Mock<ISqlWorkspaceAuditService> sqlWorkspaceAuditServiceMock;

		[SetUp]
		public void SetUp()
		{
			workspaceId = 3;
			startHourId = 666;

			this.relativityAppliationRepository = new Mock<IRelativityApplicationRepository>();
			this.dataGridCacheRepository = new Mock<IDataGridCacheRepository>();

			var sqlProvider = new Mock<IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService>>();
			var dataGridProvider = new Mock<IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>>();
			this.dataGridWorkspaceAuditService = new Mock<IDataGridWorkspaceAuditService>();
			this.sqlWorkspaceAuditServiceMock = new Mock<ISqlWorkspaceAuditService>();
			sqlProvider.Setup(p => p.GetService()).Returns(this.sqlWorkspaceAuditServiceMock.Object);
			dataGridProvider.Setup(p => p.GetService()).Returns(this.dataGridWorkspaceAuditService.Object);

			this.toggleProvider = new Mock<IToggleProvider>();

			this.logger = TestUtilities.GetMockLogger();

			this.workspaceAuditServiceFactory = new WorkspaceAuditServiceFactory(
				this.relativityAppliationRepository.Object,
				sqlProvider.Object,
				dataGridProvider.Object,
				this.toggleProvider.Object,
				this.logger.Object,
				this.dataGridCacheRepository.Object);
		}

		[Test]
		[TestCase(null, true, true, false, TestName = "Data Grid Audit not installed, yes cache")]
		[TestCase(null, true, false, false, TestName = "Data Grid Audit not installed, no cache")]
		[TestCase("1.1.1.1", true, true, false, TestName = "Data Grid Audit old version, yes cache")]
		[TestCase("1.1.1.1", true, false, false, TestName = "Data Grid Audit old version, no cache")]
		[TestCase("1.1.1.1", false, true, false, TestName = "Data Grid Audit old version 2, yes cache")]
		[TestCase("1.1.1.1", false, false, false, TestName = "Data Grid Audit old version 2, no cache")]
		[TestCase("100.100.100.100", true, true, true, TestName = "Data Grid Audit has audits, yes cache")]
		[TestCase("100.100.100.100", true, false, true, TestName = "Data Grid Audit has audits, no cache")]
		[TestCase("100.100.100.100", false, true, true, TestName = "Data Grid Audit no audits, yes cache")]
		[TestCase("100.100.100.100", false, false, false, TestName = "Data Grid Audit no audits, no cache")]
		public async Task MeetsDataGridRequirements(string dataGridAuditVersionInstalled, bool auditsInDataGrid, bool useDataGridCache, bool expectedResult)
		{
			// Arrange
			this.relativityAppliationRepository.Setup(
					m => m.GetApplicationVersion(workspaceId, Guids.Application.DataGridForAudit))
				.Returns(dataGridAuditVersionInstalled);
			this.dataGridWorkspaceAuditService.Setup(
					m => m.ReadAnyAuditsAsync(workspaceId, startHourId, AuditConstants.RelevantAuditActionIds))
				.ReturnsAsync(auditsInDataGrid);
			this.dataGridCacheRepository.Setup(m => m.ReadUseDataGrid(workspaceId, startHourId))
				.ReturnsAsync(useDataGridCache);

			// Act
			var result = await this.workspaceAuditServiceFactory.MeetsDataGridRequirements(workspaceId, startHourId);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task GetAuditService_DataGrid()
		{
			// Arrange
			this.relativityAppliationRepository.Setup(
					m => m.GetApplicationVersion(workspaceId, Guids.Application.DataGridForAudit))
				.Returns(DataGridResourceConstants.DataGridAuditMinVersion);
			this.dataGridWorkspaceAuditService.Setup(
				m => m.ReadAnyAuditsAsync(workspaceId, startHourId, AuditConstants.RelevantAuditActionIds)).ReturnsAsync(true);

			this.toggleProvider.Setup(r => r.IsEnabledAsync<DataGridAuditAnalysisToggle>())
				.ReturnsAsync(true);

			// Act
			var result = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, startHourId);

			// Assert
			Assert.That(result, Is.EqualTo(dataGridWorkspaceAuditService.Object));
			this.logger.Verify(l => l.LogVerboseAsync(It.Is<string>(s => !s.Contains("from toggle")), It.IsAny<List<string>>())); // ensure the value is not coming from the configuration
		}

		[Test]
		public async Task GetAuditService_Sql()
		{
			// Arrange
			this.relativityAppliationRepository.Setup(
					m => m.GetApplicationVersion(workspaceId, Guids.Application.DataGridForAudit))
				.Returns("");
			this.toggleProvider.Setup(r => r.IsEnabledAsync<DataGridAuditAnalysisToggle>())
				.ReturnsAsync(true);

			// Act
			var result = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, startHourId);

			// Assert
			Assert.That(result, Is.EqualTo(sqlWorkspaceAuditServiceMock.Object));
		}

		[Test]
		public async Task GetAuditService_Sql_FromConfig()
		{
			// Arrange
			this.toggleProvider.Setup(r => r.IsEnabledAsync<DataGridAuditAnalysisToggle>())
				.ReturnsAsync(false);

			// Act
			var result = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, startHourId);

			// Assert
			Assert.That(result, Is.EqualTo(sqlWorkspaceAuditServiceMock.Object));
			this.logger.Verify(l => l.LogVerboseAsync(It.Is<string>(s => s.Contains("from toggle")), It.IsAny<List<string>>())); // ensure the value came from the configuration
		}
	}
}
