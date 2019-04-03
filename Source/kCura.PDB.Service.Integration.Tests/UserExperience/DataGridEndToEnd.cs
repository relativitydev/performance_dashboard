namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.DataGrid;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;
	using global::Relativity.API;
	using global::Relativity.Services;
	using global::Relativity.Services.Field;
	using global::Relativity.Services.Proxy.Async;
	using kCura.AuditUI2.Services.ExternalAuditLog;
	using kCura.AuditUI2.Services.Interface.AuditLogManager.Models;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.DataGrid.Interfaces;

	[TestFixture]
	[Category("Integration")]
	public class DataGridEndToEnd
	{
		[Test]
		public async Task DataGrid_EndToEnd_ReadAudits()
		{
			var workspaceId = Config.WorkSpaceId;
			var auditQuery = new AuditQuery
			{
				ActionTypes = new List<AuditActionId> { AuditActionId.DocumentQuery },
				StartTime = DateTime.MinValue,
				EndTime = DateTime.UtcNow,
				WorkspaceId = workspaceId
			};

			var queryBatch = new AuditQueryBatch
			{
				Query = auditQuery,
				Size = 50,
				Start = 0
			};

			// However we grab this for testing...
			var apiClientHelper = new ApiClientHelper();
			var auditLogObjectManager = apiClientHelper.GetKeplerServiceReference<IExternalAuditLogObjectManager>();
			// Mock it up for the implementation
			var mockFactory = new Mock<IAuditLogObjectManagerFactory>();
			mockFactory.Setup(m => m.GetManager()).Returns(() => auditLogObjectManager.Value);
			var dataGridWorkspaceAuditService = DataGridEndToEnd.SetUpWorkspaceAuditService(mockFactory.Object);

			var result = await dataGridWorkspaceAuditService.ReadAuditsAsync(workspaceId, auditQuery.StartTime,
				auditQuery.EndTime,
				auditQuery.ActionTypes, queryBatch.Size, queryBatch.Start);

			var resultMessages =
				result.Select(
					r =>
							$"AuditId: {r.AuditID}, UserId {r.UserID}, Action: {r.Action}, ArtifactID: {r.ArtifactID}, ExecutionTime: {r.ExecutionTime}, WorkspaceId: {r.WorkspaceId}, TimeStamp: {r.TimeStamp}, Details: {r.Details}");
			var finalMessage = string.Join("||", resultMessages);

			Assert.Pass($"Test Passed - ResultCount {result.Count} -- {finalMessage}");
		}

		[Test]
		public async Task DataGrid_EndToEnd_ReadTotal()
		{
			var workspaceId = Config.WorkSpaceId;
			var auditQuery = new AuditQuery
			{
				ActionTypes = new List<AuditActionId> { AuditActionId.DocumentQuery },
				StartTime = DateTime.MinValue,
				EndTime = DateTime.UtcNow,
				WorkspaceId = workspaceId
			};

			// However we grab this for testing...
			var apiClientHelper = new ApiClientHelper();
			var auditLogObjectManager = apiClientHelper.GetKeplerServiceReference<IExternalAuditLogObjectManager>();
			//using (var auditLogObjectManager = TestUtilities.GetServicesManager().CreateProxy<IExternalAuditLogObjectManager>(ExecutionIdentity.System))

			// Mock it up for the implementation
			var mockFactory = new Mock<IAuditLogObjectManagerFactory>();
			mockFactory.Setup(m => m.GetManager()).Returns(() => auditLogObjectManager.Value);
			var dataGridWorkspaceAuditService = DataGridEndToEnd.SetUpWorkspaceAuditService(mockFactory.Object);

			var result =
				await
					dataGridWorkspaceAuditService.ReadTotalAuditsForHourAsync(workspaceId, auditQuery.StartTime, auditQuery.EndTime,
						auditQuery.ActionTypes);

			Assert.Pass($"Test Passed - ResultCount {result}");
		}

		[Test]
		public async Task PingAsync()
		{
			var apiClientHelper = new ApiClientHelper();
			var auditLogObjectManager = apiClientHelper.GetKeplerServiceReference<IExternalAuditLogObjectManager>();
			using (var proxy = auditLogObjectManager.Value)
			{
				var result = await proxy.PingAsync();
			}
		}

		[Test]
		public async Task ExecuteAsync()
		{
			var apiClientHelper = new ApiClientHelper();
			var auditLogObjectManager = apiClientHelper.GetKeplerServiceReference<IExternalAuditLogObjectManager>();
			using (var proxy = auditLogObjectManager.Value)
			{
				var pivotSettings = new PivotSettings()
				{
					ObjectSetQuery =
						new Query(
							"(('Action' IN CHOICE [1057247])) AND (('Timestamp' >= 0001-01-01T00:00:00.00Z)) AND (('Timestamp' <= 2018-07-24T18:47:21.59Z))",
							new List<Sort>()),
					ArtifactTypeID = 1057150,
					GroupBy = new FieldRef(1057179)
				};

				var result = await proxy.ExecuteAsync(1039923, pivotSettings, new CancellationToken(), new NonCapturingProgress<string>());
			}
		}

		[Test]
		public async Task ReadAnyAuditsAsync()
		{
			// Arrange
			var kernel = IntegrationSetupFixture.CreateNewKernel();
			kernel.Bind<IServicesMgr>().ToConstant(TestUtilities.GetKeplerServicesManager());
			kernel.Bind<IHelper>().ToConstant(TestUtilities.GetMockHelper().Object);

			var workspaceId = Config.WorkSpaceId;
			var dataGridCacheRepositoryMock = new Mock<IDataGridCacheRepository>();
			dataGridCacheRepositoryMock.Setup(m => m.ReadUseDataGrid(workspaceId, It.IsAny<int>())).ReturnsAsync(false);

			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var factory =
				new WorkspaceAuditServiceFactory(
					new RelativityApplicationRepository(connectionFactory),
					kernel.Get<IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService>>(),
					kernel.Get<IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>>(),
					new PdbSqlToggleProvider(connectionFactory),
					TestUtilities.GetMockLogger().Object,
					dataGridCacheRepositoryMock.Object);

			var dataGridAuditService = kernel.Get<IDataGridWorkspaceAuditService>();

			// Act
			var result = await dataGridAuditService.ReadAnyAuditsAsync(workspaceId, DateTime.MinValue, DateTime.UtcNow, AuditConstants.RelevantAuditActionIds);

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task HasDataGridAudits()
		{
			// Arrange
			var kernel = IntegrationSetupFixture.CreateNewKernel();
			kernel.Bind<IServicesMgr>().ToConstant(TestUtilities.GetKeplerServicesManager());
			kernel.Bind<IHelper>().ToConstant(TestUtilities.GetMockHelper().Object);

			var workspaceId = Config.WorkSpaceId;
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var hourRepository = new HourRepository(connectionFactory);
			var hour = await hourRepository.ReadLatestCompletedHourAsync();
			var dataGridCacheRepositoryMock = new Mock<IDataGridCacheRepository>();
			dataGridCacheRepositoryMock.Setup(m => m.ReadUseDataGrid(workspaceId, It.IsAny<int>())).ReturnsAsync(false);

			var factory =
				new WorkspaceAuditServiceFactory(
					new RelativityApplicationRepository(connectionFactory),
					kernel.Get<IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService>>(),
					kernel.Get<IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>>(),
					new PdbSqlToggleProvider(connectionFactory),
					TestUtilities.GetMockLogger().Object,
					dataGridCacheRepositoryMock.Object);

			// Act
			var result = await factory.HasDataGridAudits(workspaceId, hour.Id);

			// Assert
			Assert.That(result, Is.True);
		}

		public static DataGridWorkspaceAuditService SetUpWorkspaceAuditService(IAuditLogObjectManagerFactory auditLogObjectManagerFactory)
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var loggerMock = TestUtilities.GetMockLogger();
			var dataGridService = new DataGridService(auditLogObjectManagerFactory, new DataGridConditionBuilder(), new DataGridResponseAuditMapper(loggerMock.Object), loggerMock.Object);
			var artifactRepository = new ArtifactRepository(connectionFactory);
			var dataGridSettingsService = new DataGridSettingsService(artifactRepository, loggerMock.Object);
			var hourRepository = new HourRepository(connectionFactory);
			var dataGridWorkspaceAuditService = new DataGridWorkspaceAuditService(dataGridSettingsService, artifactRepository, dataGridService, hourRepository);

			return dataGridWorkspaceAuditService;
		}

		public static void SetUpDataGridOnlyWorkspaceAuditServiceFactory(IKernel kernel, string testServiceUrl = null, string testRestUrl = null, string testUsername = null, string testPassword = null)
		{
			var auditLogObjectManager =
				TestUtilities.GetKeplerServicesManager().CreateProxy<IExternalAuditLogObjectManager>(ExecutionIdentity.System);
			var mockFactory = new Mock<IAuditLogObjectManagerFactory>();
			mockFactory.Setup(m => m.GetManager()).Returns(auditLogObjectManager);
			var dataGridWorkspaceAuditService = SetUpWorkspaceAuditService(mockFactory.Object);
			kernel.Rebind<IDataGridWorkspaceAuditService>().ToConstant(dataGridWorkspaceAuditService);

			var helper = TestUtilities.GetMockHelper(testServiceUrl, testRestUrl, testUsername, testPassword);
			var mock = SetupEmptySqlAuditRepository();
			kernel.Rebind<ISqlAuditRepository>().ToConstant(mock.Object);
			kernel.Rebind<IHelper>().ToConstant(helper.Object);
		}

		public static Mock<ISqlAuditRepository> SetupEmptySqlAuditRepository()
		{
			var listOfAudits = new List<Audit>();
			var listOfUsers = new List<int>();
			var sqlAuditRepositoryMock = new Mock<ISqlAuditRepository>();
			sqlAuditRepositoryMock.Setup(
				m =>
					m.ReadAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>(),
						It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(listOfAudits);
			sqlAuditRepositoryMock.Setup(
					m =>
							m.ReadAnyAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(false);
			sqlAuditRepositoryMock.Setup(
				m =>
					m.ReadUniqueUsersForHourAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
						It.IsAny<IList<AuditActionId>>())).ReturnsAsync(listOfUsers);
			sqlAuditRepositoryMock.Setup(
				m =>
					m.ReadTotalAuditsForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
						It.IsAny<IList<AuditActionId>>())).ReturnsAsync(listOfAudits.Count);

			return sqlAuditRepositoryMock;
		}
	}
}
