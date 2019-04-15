namespace kCura.PDB.Service.Tests.Testing
{
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Service.Testing;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class OldBackupDbccServiceExecutionTests
	{
		private OldBackupDbccServiceExecution oldBackupDbccServiceExecution;
		private Mock<ISqlServerRepository> sqlServerRepositoryMock;
		private Mock<IAgentService> agentServiceMock;

		[SetUp]
		public void Setup()
		{
			this.sqlServerRepositoryMock = new Mock<ISqlServerRepository>();
			this.agentServiceMock = new Mock<IAgentService>();
			this.oldBackupDbccServiceExecution = new OldBackupDbccServiceExecution(this.sqlServerRepositoryMock.Object, this.agentServiceMock.Object);
		}

		[Test]
		public async Task ExecuteAsync()
		{
			var testCancellationToken = new CancellationToken();
			var agentId = 57713;
			this.agentServiceMock.Setup(m => m.AgentID).Returns(agentId);
			this.sqlServerRepositoryMock.Setup(
					m => m.ExecuteBackupDBCCMonitor(agentId, It.Is<string>(s => s == Names.Database.BackupAndDBCCMonLauncherSproc_Test)))
				.Verifiable();

			await this.oldBackupDbccServiceExecution.ExecuteAsync(testCancellationToken);

			this.agentServiceMock.VerifyAll();
			this.sqlServerRepositoryMock.VerifyAll();
		}
	}
}
