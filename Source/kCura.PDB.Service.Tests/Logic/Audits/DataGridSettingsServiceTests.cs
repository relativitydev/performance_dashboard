namespace kCura.PDB.Service.Tests.Logic.Audits
{
	using System;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Service.Audits;

	[TestFixture, Category("Unit")]
	public class DataGridSettingsServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.logger = TestUtilities.GetMockLogger();
			this.artifactRepository = new Mock<IArtifactRepository>();
			this.dataGridSettingsService = new DataGridSettingsService(this.artifactRepository.Object, this.logger.Object);
		}

		private Mock<IArtifactRepository> artifactRepository;
		private Mock<ILogger> logger;
		private DataGridSettingsService dataGridSettingsService;

		[Test]
		public async Task ReadActionChoiceIds()
		{
			// Arrange
			var workspaceId = 12345;
			var fieldChoiceId = 5555;
			var auditActions = new[] { AuditActionId.DocumentQuery, AuditActionId.View };
			this.artifactRepository.Setup(r => r.ReadFieldChoiceIdByGuid(workspaceId, It.IsAny<Guid>()))
				.ReturnsAsync(fieldChoiceId);
			this.artifactRepository.Setup(r => r.ReadChoiceByChoiceType(workspaceId, fieldChoiceId))
				.ReturnsAsync(new[]
				{
					new Choice { ArtifactId = 777, Name = AuditActionId.DocumentQuery.GetDisplayName() },
					new Choice { ArtifactId = 888, Name = AuditActionId.View.GetDisplayName() },
				});

			// Act
			var result = await this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, auditActions);

			// Assert
			Assert.That(result.Count, Is.EqualTo(auditActions.Length));
		}
	}
}
