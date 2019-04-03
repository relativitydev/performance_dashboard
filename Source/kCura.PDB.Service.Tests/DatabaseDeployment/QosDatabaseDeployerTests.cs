namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class QosDatabaseDeployerTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepo = new Mock<IServerRepository>();
			this.deployer = new Mock<IDatabaseDeployer>();
			this.qosDatabaseDeployer = new QosDatabaseDeployer(this.serverRepo.Object, this.deployer.Object);
		}

		private Mock<IServerRepository> serverRepo;
		private Mock<IDatabaseDeployer> deployer;
		private QosDatabaseDeployer qosDatabaseDeployer;

		[Test]
		public async Task ServerDatabaseDeployment()
		{
			// Arrange
			this.serverRepo.Setup(r => r.ReadAsync(1234)).ReturnsAsync(new Server { ServerId = 1234, ServerName = "abc" });
			this.serverRepo.Setup(r => r.UpdateAsync(It.IsAny<Server>())).Returns(Task.Delay(12));
			this.deployer.Setup(d => d.DeployQos(It.IsAny<Server>()));

			// Act
			await this.qosDatabaseDeployer.ServerDatabaseDeployment(1234);

			// Assert
			this.serverRepo.Verify(r => r.UpdateAsync(It.IsAny<Server>()));
		}

		[Test]
		public async Task StartQosDatabaseDeployment()
		{
			// Arrange
			this.serverRepo.Setup(r => r.ReadServerPendingQosDeploymentAsync())
				.ReturnsAsync(new[] { new Server { ServerId = 1234 }, new Server { ServerId = 2345 } });

			// Act
			var result = await this.qosDatabaseDeployer.StartQosDatabaseDeployment();

			// Assert
			Assert.That(result, Is.EqualTo(new[] { 1234, 2345 }));
		}
	}
}
