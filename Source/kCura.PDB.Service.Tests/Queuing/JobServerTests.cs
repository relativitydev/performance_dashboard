namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Hangfire;
	using Hangfire.Storage;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;
	using Service.Queuing;

	[TestFixture]
	[Category("Unit")]
	public class JobServerTests
	{
		[SetUp]
		public void Setup()
		{
			this.monitoringApi = new Mock<IMonitoringApi>();
			this.agentRepositoryMock = new Mock<IAgentRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.configurationRepositoryMock = new Mock<IConfigurationRepository>();
			this.agentServiceMock = new Mock<IAgentService>();
			this.jobServerOptionsFactoryMock = new Mock<IJobServerOptionsFactory>();
			this.jobServerOptionsFactoryMock.Setup(m => m.GetOptions()).Returns(new BackgroundJobServerOptions());
			this.timeService = TestUtilities.GetTimeService(60);
		}

		private Mock<IMonitoringApi> monitoringApi;
		private Mock<IAgentRepository> agentRepositoryMock;
		private Mock<ILogger> loggerMock;
		private Mock<IConfigurationRepository> configurationRepositoryMock;
		private Mock<IAgentService> agentServiceMock;
		private Mock<IJobServerOptionsFactory> jobServerOptionsFactoryMock;
		private ITimeService timeService;


		[Test]
		[Timeout(2000)]
		public async Task WaitTillProcessesAreDone()
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			JobStorage.Current = new TestJobStorage(monitoringApi.Object);

			var agentId = 123;
			this.agentServiceMock.Setup(m => m.AgentID).Returns(agentId);
			this.agentRepositoryMock.SetupSequence(m => m.ReadAgentEnabled(agentId))
				.Returns(true)
				.Returns(false);
			this.configurationRepositoryMock.Setup(
				m => m.ReadValue<int>(ConfigurationKeys.JobServerSleepTime)).Returns(10);
			this.configurationRepositoryMock.Setup(
				m => m.ReadValue<int>(ConfigurationKeys.JobServerMaxExecution)).Returns(900);

			// Act
			using (var server = new JobServer(this.agentRepositoryMock.Object, this.configurationRepositoryMock.Object, this.loggerMock.Object, this.agentServiceMock.Object, this.jobServerOptionsFactoryMock.Object, this.timeService))
			{
				await server.WaitTillProcessesAreDone(cancellationToken);

				// Assert
				this.configurationRepositoryMock.Verify(m => m.ReadValue<int>(ConfigurationKeys.JobServerSleepTime), Times.Once);
				this.configurationRepositoryMock.Verify(m => m.ReadValue<int>(ConfigurationKeys.JobServerSleepTime), Times.Once);
				this.agentRepositoryMock.Verify(m => m.ReadAgentEnabled(agentId), Times.Exactly(2));
			}
		}

		[Test]
		[Timeout(2000)]
		public async Task WaitTillProcessesAreDone_ReachedMaxTimeout()
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			JobStorage.Current = new TestJobStorage(monitoringApi.Object);

			var agentId = 123;
			this.agentServiceMock.Setup(m => m.AgentID).Returns(agentId);
			this.agentRepositoryMock.Setup(m => m.ReadAgentEnabled(agentId)).Returns(true);
			this.configurationRepositoryMock.Setup(
				m => m.ReadValue<int>(ConfigurationKeys.JobServerSleepTime)).Returns(31);
			this.configurationRepositoryMock.Setup(
				m => m.ReadValue<int>(ConfigurationKeys.JobServerMaxExecution)).Returns(30);

			using (var server = new JobServer(this.agentRepositoryMock.Object, this.configurationRepositoryMock.Object, this.loggerMock.Object, this.agentServiceMock.Object, this.jobServerOptionsFactoryMock.Object, this.timeService))
			{
				// Act
				await server.WaitTillProcessesAreDone(cancellationToken);

				// Assert
				this.configurationRepositoryMock.Verify(m => m.ReadValue<int>(ConfigurationKeys.JobServerSleepTime), Times.Once);
				this.configurationRepositoryMock.Verify(m => m.ReadValue<int>(ConfigurationKeys.JobServerMaxExecution), Times.Once);
				this.agentRepositoryMock.Verify(m => m.ReadAgentEnabled(agentId), Times.Once);
			}
		}

		private class TestJobStorage : JobStorage
		{
			public TestJobStorage(IMonitoringApi monitoringApi)
			{
				this.monitoringApi = monitoringApi;
			}

			private readonly IMonitoringApi monitoringApi;

			public override IMonitoringApi GetMonitoringApi()
			{
				return monitoringApi;
			}

			public override IStorageConnection GetConnection()
			{
				throw new NotImplementedException();
			}
		}
	}
}
