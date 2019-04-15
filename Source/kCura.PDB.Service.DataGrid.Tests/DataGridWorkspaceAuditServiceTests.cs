namespace kCura.PDB.Service.DataGrid.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Audits.DataGrid;
	using kCura.PDB.Service.DataGrid;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class DataGridWorkspaceAuditServiceTests
	{
		private DataGridWorkspaceAuditService dataGridWorkspaceAuditService;
		private Mock<IDataGridSettingsService> dataGridSettingsRepositoryMock;
		private Mock<IArtifactRepository> artifactRepository;
		private Mock<IDataGridService> dataGridServiceMock;
		private Mock<IHourRepository> hourRepositoryMock;

		private int workspaceId;
		private DateTime start;
		private DateTime end;
		private List<AuditActionId> actions;

		[SetUp]
		public void SetUp()
		{
			this.dataGridSettingsRepositoryMock = new Mock<IDataGridSettingsService>();
			this.artifactRepository = new Mock<IArtifactRepository>();
			this.dataGridServiceMock = new Mock<IDataGridService>();
			this.hourRepositoryMock = new Mock<IHourRepository>();

			this.dataGridWorkspaceAuditService = new DataGridWorkspaceAuditService(this.dataGridSettingsRepositoryMock.Object, this.artifactRepository.Object, this.dataGridServiceMock.Object, this.hourRepositoryMock.Object);

			this.workspaceId = 1;
			this.start = DateTime.UtcNow.AddDays(-180);
			this.end = start.AddHours(1);
			this.actions = new List<AuditActionId> { };
		}
		
		[Test]
		public async Task ReadAuditsAsync()
		{
			// Arrange
			var batchSize = 3;
			var batchStart = 1;

			var auditArtifactTypeId = 2;
			var choiceIds = new List<int> { 1, 2, 3 };

			var audits = new List<Audit>();

			this.artifactRepository.Setup(m => m.ReadAuditArtifactTypeId(this.workspaceId)).ReturnsAsync(auditArtifactTypeId);
			this.dataGridSettingsRepositoryMock.Setup(m => m.ReadActionChoiceIds(this.workspaceId, this.actions)).ReturnsAsync(choiceIds);
			this.dataGridServiceMock.Setup(m => m.ReadAuditsAsync(It.IsAny<AuditQueryBatch>(), auditArtifactTypeId, choiceIds))
				.ReturnsAsync(audits);

			// Act
			var result = await this.dataGridWorkspaceAuditService.ReadAuditsAsync(this.workspaceId, this.start, this.end, this.actions, batchSize, batchStart);

			// Assert
			Assert.That(result, Is.EquivalentTo(audits));
			this.dataGridSettingsRepositoryMock.VerifyAll();
			this.dataGridServiceMock.VerifyAll();
		}

		[Test]
		public async Task ReadTotalAuditsForHourAsync()
		{
			// Arrange
			var auditArtifactTypeId = 2;
			var userGroupById = 4;
			var choiceIds = new List<int> { 1, 2, 3 };

			var audits = new List<Audit>();

			this.artifactRepository.Setup(m => m.ReadAuditArtifactTypeId(this.workspaceId)).ReturnsAsync(auditArtifactTypeId);
			this.dataGridSettingsRepositoryMock.Setup(m => m.ReadActionChoiceIds(this.workspaceId, this.actions)).ReturnsAsync(choiceIds);
			this.artifactRepository.Setup(m => m.ReadGroupByArtifactId(this.workspaceId, DataGridGroupByEnum.User))
				.ReturnsAsync(userGroupById);
			this.dataGridServiceMock.Setup(m => m.ReadTotalAuditsForHourAsync(It.IsAny<AuditQuery>(), auditArtifactTypeId, choiceIds, userGroupById))
				.ReturnsAsync(audits.Count);

			// Act
			var result = await this.dataGridWorkspaceAuditService.ReadTotalAuditsForHourAsync(this.workspaceId, this.start, this.end, this.actions);

			// Assert
			Assert.That(result, Is.EqualTo(audits.Count));
			this.dataGridSettingsRepositoryMock.VerifyAll();
			this.dataGridServiceMock.VerifyAll();
		}

		[Test]
		public async Task ReadUniqueUsersForHourAuditsAsync()
		{
			// Arrange
			var auditArtifactTypeId = 2;
			var userGroupById = 4;
			var choiceIds = new List<int> { 1, 2, 3 };
			var userIds = new List<int> { 12, 34, 56 };

			this.artifactRepository.Setup(m => m.ReadAuditArtifactTypeId(this.workspaceId)).ReturnsAsync(auditArtifactTypeId);
			this.dataGridSettingsRepositoryMock.Setup(m => m.ReadActionChoiceIds(this.workspaceId, this.actions)).ReturnsAsync(choiceIds);
			this.artifactRepository.Setup(m => m.ReadGroupByArtifactId(this.workspaceId, DataGridGroupByEnum.User))
				.ReturnsAsync(userGroupById);
			this.dataGridServiceMock.Setup(m => m.ReadUniqueUsersForHourAuditsAsync(It.IsAny<AuditQuery>(), auditArtifactTypeId, choiceIds, userGroupById))
				.ReturnsAsync(userIds);

			// Act
			var result = await this.dataGridWorkspaceAuditService.ReadUniqueUsersForHourAuditsAsync(this.workspaceId, this.start, this.end, this.actions);

			// Assert
			Assert.That(result, Is.EquivalentTo(userIds));
			this.dataGridSettingsRepositoryMock.VerifyAll();
			this.dataGridServiceMock.VerifyAll();
		}

		[Test, Category("Ignore"), Ignore("Not supported in Data Grid APIs")]
		public void ReadTotalAuditExecutionTimeForHourAsync()
		{
			// Arrange
			// Act
			var result = this.dataGridWorkspaceAuditService.ReadTotalAuditExecutionTimeForHourAsync(this.workspaceId, this.start, this.end, this.actions);

			// Assert
		}

		[Test]
		public async Task ReadTotalLongRunningQueriesForHourAsync()
		{
			// Arrange
			var auditArtifactTypeId = 2;
			var userGroupById = 4;
			var choiceIds = new List<int> { 1, 2, 3 };
			var longRunningQueries = 10;

			this.artifactRepository.Setup(m => m.ReadAuditArtifactTypeId(this.workspaceId)).ReturnsAsync(auditArtifactTypeId);
			this.dataGridSettingsRepositoryMock.Setup(m => m.ReadActionChoiceIds(this.workspaceId, this.actions)).ReturnsAsync(choiceIds);
			this.artifactRepository.Setup(m => m.ReadGroupByArtifactId(this.workspaceId, DataGridGroupByEnum.User))
				.ReturnsAsync(userGroupById);
			this.dataGridServiceMock.Setup(m => m.ReadTotalLongRunningQueriesForHourAsync(It.IsAny<AuditQuery>(), auditArtifactTypeId, choiceIds, userGroupById))
				.ReturnsAsync(longRunningQueries);

			// Act
			var result = await this.dataGridWorkspaceAuditService.ReadTotalLongRunningQueriesForHourAsync(this.workspaceId, this.start, this.end, this.actions);

			// Assert
			Assert.That(result, Is.EqualTo(longRunningQueries));
			this.dataGridSettingsRepositoryMock.VerifyAll();
			this.dataGridServiceMock.VerifyAll();
		}
	}
}
