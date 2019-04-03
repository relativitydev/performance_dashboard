namespace kCura.PDB.EventHandler.Test
{
	using System;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using Moq;
	using NUnit.Framework;
	using Relativity.API;

	[TestFixture]
	[Category("Unit")]
	public class RemoveOldResourcesPostInstallEventHandlerTests
	{
		[SetUp]
		public void Setup()
		{
			helper = new Mock<IEHHelper>();
			repo = new Mock<ISqlServerRepository>();
			helper.Setup(h => h.GetActiveCaseID()).Returns(1234);
			this.removeOldResourcesPostInstallEventHandler = new RemoveOldResourcesPostInstallEventHandler(repo.Object);
			this.removeOldResourcesPostInstallEventHandler.Helper = this.helper.Object;
		}

		private Mock<IEHHelper> helper;
		private Mock<ISqlServerRepository> repo;
		private RemoveOldResourcesPostInstallEventHandler removeOldResourcesPostInstallEventHandler;

		[Test]
		public void InstallRequirementsPreInstall_Execute_Fails()
		{
			//Arrange
			repo.Setup(r => r.DeploymentRepository.RemoveOldApplicationReferences());
			repo.Setup(r => r.DeploymentRepository.RemoveOldApplicationReferencesFromWorkspace(It.IsAny<Guid>(), 1234));
			repo.Setup(r => r.DeploymentRepository.RemoveOldResourceFiles());

			//Act
			var response = this.removeOldResourcesPostInstallEventHandler.Execute();

			//Assert
			repo.Verify(r => r.DeploymentRepository.RemoveOldApplicationReferences());
			repo.Verify(r => r.DeploymentRepository.RemoveOldApplicationReferencesFromWorkspace(It.IsAny<Guid>(), 1234), Times.Exactly(Guids.Agent.AgentGuidsToRemove.Count));
			repo.Verify(r => r.DeploymentRepository.RemoveOldResourceFiles());
			Assert.That(response.Success, Is.True);
			Assert.That(response.Message, Is.EqualTo("Removed old resource files."));
		}
	}
}
