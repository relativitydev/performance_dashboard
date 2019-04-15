namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits.DataGrid;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class ArtifactRepositoryTests
	{
		private ArtifactRepository artifactRepository;
		private IConnectionFactory connectionFactory;

		[SetUp]
		public void SetUp()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.artifactRepository = new ArtifactRepository(connectionFactory);
		}

		[Test]
		[TestCase(DataGridGroupByEnum.Action)]
		[TestCase(DataGridGroupByEnum.Date)]
		[TestCase(DataGridGroupByEnum.ObjectType)]
		[TestCase(DataGridGroupByEnum.User)]
		public async Task ReadGroupByArtifactId(DataGridGroupByEnum groupBy)
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;

			// Act
			var result = await this.artifactRepository.ReadGroupByArtifactId(workspaceId, groupBy);

			// Assert
			Assert.Pass($"Passed initially {groupBy} - Result {result}");
		}

		[Test]
		public async Task ReadAuditArtifactTypeId()
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;

			// Act
			var result = await this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);

			// Assert
			Assert.Pass($"DataGridAudit ArtifactTypeId in workspace {workspaceId} - Result {result}");
		}

		[Test,
			TestCase("2BAACA72-790C-4B87-A7D8-C18C45CAC63D", 1000036)]
		public async Task ReadFieldChoiceIdByGuid(string guidStr, int expectedResult)
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;

			// Act
			var result = await this.artifactRepository.ReadFieldChoiceIdByGuid(workspaceId, Guid.Parse(guidStr));

			// Assert
			Assert.That(result.HasValue, Is.True);
			Assert.That(result.Value, Is.EqualTo(expectedResult));
		}

		[Test,
			TestCase("2BAACA72-790C-4B87-A7D8-C18C45CAC63D")]
		public async Task ReadChoiceByChoiceType(string guidStr)
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;
			var fieldChoiceId = await this.artifactRepository.ReadFieldChoiceIdByGuid(workspaceId, Guid.Parse(guidStr));

			// Act
			var result = await this.artifactRepository.ReadChoiceByChoiceType(workspaceId, fieldChoiceId.Value);

			// Assert
			Assert.That(result, Is.Not.Empty);
		}

		//[Test]
		//[TestCase(new[] {AuditActionId.DocumentQuery, AuditActionId.RelativityScriptExecution})]
		//[TestCase(new[]
		// {
		//	 AuditActionId.View,
		//	 AuditActionId.Update,
		//	 AuditActionId.UpdateMassEdit, 
		//	 AuditActionId.UpdateMassReplace, 
		//	 AuditActionId.UpdatePropagation, 
		//	 AuditActionId.DocumentQuery, 
		//	 AuditActionId.Query, 
		//	 AuditActionId.Import, 
		//	 AuditActionId.Export, 
		//	 AuditActionId.ReportQuery, 
		//	 AuditActionId.RelativityScriptExecution, 
		//	 AuditActionId.PivotQuery, 
		//	 AuditActionId.UpdateImport
		// }, TestName = "All")]
		//public void ReadActionChoiceIds(AuditActionId[] actions)
		//{
		//	var workspaceId = Config.WorkSpaceId;
		//	var result = this.artifactRepository..ReadActionChoiceIds(workspaceId, actions);

		//	Assert.That(actions.Length, Is.EqualTo(result.Count));
		//	Assert.Pass($"ActionIds Passed: {string.Join(",", actions)} - Results {string.Join(",", result)}");
		//}



		//[Test, Explicit("Doesn't trigger failure as expected.")]
		//public async Task HeavyLoadTest()
		//{
		//	// Arrange
		//	var workspaceId = Config.WorkSpaceId;

		//	// Act
		//	var tasks =
		//		Enumerable.Range(1, 500)
		//			.Select(a => Task.Run(() => this.dataGridSettingsRepository.ReadAuditArtifactTypeId(workspaceId)));
		//	var data = await Task.WhenAll(tasks);

		//	Assert.Pass($"DataGridAudit ArtifactTypeId in workspace {workspaceId} - Result {data}");
		//}

		[Test]
		public async Task IsWorkspaceUpgradeComplete()
		{
			var workspaceId = Config.WorkSpaceId;
			var result = await this.artifactRepository.IsWorkspaceUpgradeComplete(workspaceId);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task TestDatabaseAccessAsync()
		{
			var workspaceId = Config.WorkSpaceId;
			var result = await this.artifactRepository.TestDatabaseAccessAsync(workspaceId);

			Assert.That(result, Is.True);
		}
	}
}
