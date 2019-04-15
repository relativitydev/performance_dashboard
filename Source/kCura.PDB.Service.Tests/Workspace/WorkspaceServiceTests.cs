namespace kCura.PDB.Service.Tests.Workspace
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Workspace;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class WorkspaceServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.artifactRepository = new Mock<IArtifactRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.workspaceService = new WorkspaceService(this.serverRepository.Object, this.artifactRepository.Object, this.logger.Object);
		}

		private Mock<IServerRepository> serverRepository;
		private Mock<IArtifactRepository> artifactRepository;
		private Mock<ILogger> logger;
		private WorkspaceService workspaceService;

		[Test]
		public async Task ReadAvailableWorkspaceIdsAsync()
		{
			// Assert
			this.serverRepository.Setup(r => r.ReadServerWorkspaceIdsAsync(1234))
				.ReturnsAsync(new[] { 1, 2, 3, 4 });

			this.artifactRepository.Setup(r => r.TestDatabaseAccessAsync(It.IsAny<int>()))
				.ReturnsAsync(true);
			this.artifactRepository.Setup(r => r.IsWorkspaceUpgradeComplete(It.IsAny<int>())).ReturnsAsync(true);

			// Act
			var result = await this.workspaceService.ReadAvailableWorkspaceIdsAsync(1234);

			// Arrange
			Assert.That(result, Is.EqualTo(new[] { 1, 2, 3, 4 }));
		}

		[Test]
		public async Task WorkspaceIsAvailableAsync_Success()
		{
			// Assert
			this.artifactRepository.Setup(r => r.TestDatabaseAccessAsync(1234))
				.ReturnsAsync(true);
			this.artifactRepository.Setup(r => r.IsWorkspaceUpgradeComplete(1234))
				.ReturnsAsync(true);

			// Act
			var result = await this.workspaceService.WorkspaceIsAvailableAsync(1234);

			// Arrange
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task WorkspaceIsAvailableAsync_Errors()
		{
			// Assert
			this.artifactRepository.Setup(r => r.TestDatabaseAccessAsync(1234))
				.ThrowsAsync(new Exception("test error"));

			// Act
			var result = await this.workspaceService.WorkspaceIsAvailableAsync(1234);

			// Arrange
			Assert.That(result, Is.False);
			this.logger.Verify(l => l.LogWarningAsync(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<List<string>>()), Times.AtLeastOnce);
		}

		[Test]
		public async Task WorkspaceIsAvailableAsync_FailedToAccess()
		{
			// Assert
			this.artifactRepository.Setup(r => r.TestDatabaseAccessAsync(1234)).ReturnsAsync(false);
			this.artifactRepository.Setup(r => r.IsWorkspaceUpgradeComplete(1234))
				.ReturnsAsync(true);

			// Act
			var result = await this.workspaceService.WorkspaceIsAvailableAsync(1234);

			// Arrange
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task WorkspaceIsAvailableAsync_FailedUpgrading()
		{
			// Assert
			this.artifactRepository.Setup(r => r.TestDatabaseAccessAsync(1234)).ReturnsAsync(true);
			this.artifactRepository.Setup(r => r.IsWorkspaceUpgradeComplete(1234))
				.ReturnsAsync(false);

			// Act
			var result = await this.workspaceService.WorkspaceIsAvailableAsync(1234);

			// Arrange
			Assert.That(result, Is.False);
		}
	}
}
