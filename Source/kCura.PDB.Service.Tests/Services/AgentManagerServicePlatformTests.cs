namespace kCura.PDB.Service.Tests.Services
{
	using System.Threading.Tasks;

	using global::Relativity.Services.Agent;

	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Tests.Common;

	using NUnit.Framework;

	[TestFixture]
	[Category("Platform")]
	[Category("Integration")]
	//[Explicit("Non-functional")]
	public class AgentManagerServicePlatformTests
	{
		private AgentManagerService agentManagerService;
		private IAgentManager agentManagerProxy;

		[SetUp]
		public void Setup()
		{
			var apiClientHelper = new ApiClientHelper();
			this.agentManagerProxy = apiClientHelper.GetKeplerServiceReference<IAgentManager>().Value;
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var agentRepository = new AgentRepository(connectionFactory);
			var logger = TestUtilities.GetMockLogger();

			this.agentManagerService = new AgentManagerService(this.agentManagerProxy, agentRepository, logger.Object);
		}

		[Test]
		public async Task StopPerformanceDashboardAgents()
		{
			// Arrange
			// Act
			var results = await this.agentManagerService.StopPerformanceDashboardAgentsAsync();

			// Assert
		}

		[Test]
		public async Task StartPerformanceDashboardAgentsAsync()
		{
			await this.agentManagerService.StartPerformanceDashboardAgentsAsync();
		}

		[Test]
		public async Task StopAndStartPerformanceDashboardAgents()
		{
			var results = await this.agentManagerService.StopPerformanceDashboardAgentsAsync();
			await this.agentManagerService.StartPerformanceDashboardAgentsAsync(results);
		}

		[Test]
		public async Task CreatePerformanceDashboardAgents()
		{
			var machineName = Config.Server;
			var results = await this.agentManagerService.CreatePerformanceDashboardAgents(machineName);
		}

		[TearDown]
		public void TearDown()
		{
			this.agentManagerProxy?.Dispose();
		}
	}
}
