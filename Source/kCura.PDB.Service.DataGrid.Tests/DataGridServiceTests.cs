namespace kCura.PDB.Service.DataGrid.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.AuditUI2.Services.ExternalAuditLog;
	using kCura.AuditUI2.Services.Interface.AuditLogManager.Models;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Service.DataGrid;
	using kCura.PDB.Service.DataGrid.Interfaces;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Services.Pivot;
	using PivotSettings = kCura.AuditUI2.Services.Interface.AuditLogManager.Models.PivotSettings;

	[TestFixture, Category("Unit")]
	public class DataGridServiceTests
	{
		private DataGridService dataGridService;

		private Mock<IAuditLogObjectManagerFactory> auditLogObjectManagerFactoryMock;
		private Mock<IDataGridConditionBuilder> dataGridConditionBuilderMock;
		private Mock<IDataGridResponseAuditMapper> dataGridResponseAuditMapperMock;
		private Mock<IExternalAuditLogObjectManager> auditManagerMock;
		private Mock<ILogger> loggerMock;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = TestUtilities.GetMockLogger();
			this.auditManagerMock = new Mock<IExternalAuditLogObjectManager>();
			this.auditLogObjectManagerFactoryMock = new Mock<IAuditLogObjectManagerFactory>(MockBehavior.Strict);
			this.auditLogObjectManagerFactoryMock.Setup(m => m.GetManager()).Returns(auditManagerMock.Object);
			this.dataGridConditionBuilderMock = new Mock<IDataGridConditionBuilder>(MockBehavior.Strict);
			this.dataGridResponseAuditMapperMock = new Mock<IDataGridResponseAuditMapper>(MockBehavior.Strict);
			this.dataGridService = new DataGridService(auditLogObjectManagerFactoryMock.Object, this.dataGridConditionBuilderMock.Object, this.dataGridResponseAuditMapperMock.Object, this.loggerMock.Object);
		}

		[Test]
		[Explicit("TODO -- Fix execution time for TeamCity, takes over 1 second")]
		public async Task ReadAuditsAsync_Success()
		{
			// Arrange
			var queryBatch = new AuditQueryBatch
			{
				Query = new AuditQuery
				{
					WorkspaceId = 123
				}
			};
			var queryStart = Convert.ToInt32(queryBatch.Start + 1);
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var testCondition = "TEST";
			this.dataGridConditionBuilderMock.Setup(
					m => m.BuildActionTimeframeCondition(choiceIds, queryBatch.Query.StartTime, queryBatch.Query.EndTime))
				.Returns(testCondition);

			var testResponse = new QueryResult();
			this.auditManagerMock.Setup(
				m =>
					m.QueryAsync(queryBatch.Query.WorkspaceId,
						It.Is<QueryRequest>(r => r.ObjectType.ArtifactTypeID == auditArtifactTypeId && r.Condition == testCondition),
						queryStart,
						queryBatch.Size,
						It.Is<AuditQueryOptions>(o => o.ReturnRawDetails == true)))
							.ReturnsAsync(testResponse);

			var audits = new List<Audit>();
			this.dataGridResponseAuditMapperMock.Setup(m => m.ResponseToAudits(testResponse, queryBatch.Query.WorkspaceId)).Returns(audits);

			// Act
			var result = await this.dataGridService.ReadAuditsAsync(queryBatch, auditArtifactTypeId, choiceIds);

			// Assert
			Assert.That(result, Is.EqualTo(audits));
			this.auditLogObjectManagerFactoryMock.VerifyAll();
			this.auditManagerMock.VerifyAll();
			this.dataGridConditionBuilderMock.VerifyAll();
			this.dataGridResponseAuditMapperMock.VerifyAll();
		}

		[Test]
		[Explicit("TODO -- Fix execution time for TeamCity, takes over 1 second")]
		public void ReadAuditsAsync_Failure()
		{
			// Arrange
			var queryBatch = new AuditQueryBatch
			{
				Query = new AuditQuery
				{
					WorkspaceId = 123
				}
			};
			var queryStart = Convert.ToInt32(queryBatch.Start + 1);
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var testCondition = "TEST";
			this.dataGridConditionBuilderMock.Setup(
					m => m.BuildActionTimeframeCondition(choiceIds, queryBatch.Query.StartTime, queryBatch.Query.EndTime))
				.Returns(testCondition);

			var testResponse = new QueryResult();
			this.auditManagerMock.Setup(
					m =>
						m.QueryAsync(queryBatch.Query.WorkspaceId,
							It.Is<QueryRequest>(r => r.ObjectType.ArtifactTypeID == auditArtifactTypeId && r.Condition == testCondition),
							queryStart,
							queryBatch.Size,
							It.Is<AuditQueryOptions>(o => o.ReturnRawDetails == true)))
								.ThrowsAsync(new Exception("TestException"));

			// Act
			// Assert
			var exception =
				Assert.ThrowsAsync<Exception>(() => this.dataGridService.ReadAuditsAsync(queryBatch, auditArtifactTypeId, choiceIds));
			this.auditLogObjectManagerFactoryMock.VerifyAll();
			this.auditManagerMock.VerifyAll();
			this.dataGridConditionBuilderMock.VerifyAll();
			this.dataGridResponseAuditMapperMock.VerifyAll();
		}

		[Test]
		[Explicit("TODO -- Fix execution time for TeamCity, takes over 1 second")]
		public async Task ReadUniqueUsersForHourAuditsAsync()
		{
			// Arrange
			var query = new AuditQuery
			{

			};
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var userGroupById = 3;
			var testCondition = "TEST";
			this.dataGridConditionBuilderMock.Setup(
					m => m.BuildActionTimeframeCondition(choiceIds, query.StartTime, query.EndTime))
				.Returns(testCondition);

			var users = new[] { 12, 34, 56 };

			var expectedResult = new PivotResultSet
			{
				Results = new DataTable("TestTable")
				{
					Columns = { new DataColumn(DataGridResourceConstants.DataGridUserName, typeof(string)) },
				},
				Success = true
			};

			users.ForEach(u =>
			{
				var row = expectedResult.Results.NewRow();
				row[DataGridResourceConstants.DataGridUserName] = u.ToString();
				expectedResult.Results.Rows.Add(row);
			});

			this.auditManagerMock.Setup(m => m.ExecuteAsync(
					query.WorkspaceId,
					It.Is<PivotSettings>(s =>
						s.ArtifactTypeID == auditArtifactTypeId 
						&& s.GroupBy.ArtifactID == userGroupById 
						&& s.ObjectSetQuery.Condition == testCondition),
					It.IsAny<CancellationToken>(),
					It.IsAny<IProgress<string>>()))
				.ReturnsAsync(expectedResult);


			var result = await this.dataGridService.ReadUniqueUsersForHourAuditsAsync(query, auditArtifactTypeId, choiceIds, userGroupById);

			// Assert
			Assert.That(result, Is.EquivalentTo(users));
		}

		[Test]
		[Explicit("TODO -- Fix execution time for TeamCity, takes over 1 second")]
		public async Task ReadTotalAuditsForHourAsync()
		{
			// Arrange
			var query = new AuditQuery
			{

			};
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var userGroupById = 3;
			var testCondition = "TEST";
			this.dataGridConditionBuilderMock.Setup(
					m => m.BuildActionTimeframeCondition(choiceIds, query.StartTime, query.EndTime))
				.Returns(testCondition);

			var totalAudits = new [] {34, 34, 1, 0};

			var expectedResult = new PivotResultSet
			{
				Results = new DataTable("TestTable")
				{
					Columns = { new DataColumn(DataGridResourceConstants.DataGridGrandTotal, typeof(string)) },
				},
				Success = true
			};

			totalAudits.ForEach(t =>
			{
				var row = expectedResult.Results.NewRow();
				row[DataGridResourceConstants.DataGridGrandTotal] = t.ToString();
				expectedResult.Results.Rows.Add(row);
			});

			this.auditManagerMock.Setup(m => m.ExecuteAsync(
					query.WorkspaceId,
					It.Is<PivotSettings>(s =>
						s.ArtifactTypeID == auditArtifactTypeId
						&& s.GroupBy.ArtifactID == userGroupById 
						&& s.ObjectSetQuery.Condition == testCondition),
					It.IsAny<CancellationToken>(),
					It.IsAny<IProgress<string>>()))
				.ReturnsAsync(expectedResult);

			var result = await this.dataGridService.ReadTotalAuditsForHourAsync(query, auditArtifactTypeId, choiceIds, userGroupById);

			Assert.That(result, Is.EqualTo(totalAudits.Sum()));
		}

		[Test]
		[Explicit("TODO -- Fix execution time for TeamCity, takes over 1 second")]
		public async Task ReadTotalLongRunningQueriesForHourAsync()
		{
			// Arrange
			var query = new AuditQuery
			{

			};
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var userGroupById = 3;
			var testCondition = "TEST";
			this.dataGridConditionBuilderMock.Setup(
					m => m.BuildActionTimeframeLongRunningCondition(choiceIds, query.StartTime, query.EndTime))
				.Returns(testCondition);
			var totalAudits = new [] {34};

			var expectedResult = new PivotResultSet
			{
				Results = new DataTable("TestTable")
				{
					Columns = { new DataColumn(DataGridResourceConstants.DataGridGrandTotal, typeof(string)) },
				},
				Success = true
			};

			totalAudits.ForEach(t =>
			{
				var row = expectedResult.Results.NewRow();
				row[DataGridResourceConstants.DataGridGrandTotal] = t.ToString();
				expectedResult.Results.Rows.Add(row);
			});

			this.auditManagerMock.Setup(m => m.ExecuteAsync(
					query.WorkspaceId,
					It.Is<PivotSettings>(s =>
						s.ArtifactTypeID == auditArtifactTypeId && s.GroupBy.ArtifactID == userGroupById &&
						s.ObjectSetQuery.Condition == testCondition),
					It.IsAny<CancellationToken>(),
					It.IsAny<IProgress<string>>()))
				.ReturnsAsync(expectedResult);

			var result = await this.dataGridService.ReadTotalLongRunningQueriesForHourAsync(query, auditArtifactTypeId, choiceIds, userGroupById);

			Assert.That(result, Is.EqualTo(totalAudits.Sum()));
		}

		[Test]
		[Category("Ignore")]
		[Ignore("Not supported currently by Data Grid API")]
		public async Task ReadTotalAuditExecutionTimeForHourAsync()
		{
			// Arrange
			var query = new AuditQuery
			{

			};
			var auditArtifactTypeId = 2;
			var choiceIds = new[] { 1, 2 };
			var userGroupById = 3;

			var result = await this.dataGridService.ReadTotalAuditExecutionTimeForHourAsync(query, auditArtifactTypeId, choiceIds, userGroupById);
		}
	}
}