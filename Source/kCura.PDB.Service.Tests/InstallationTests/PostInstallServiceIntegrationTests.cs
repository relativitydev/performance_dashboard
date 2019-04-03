namespace kCura.PDB.Service.Tests.InstallationTests
{
	using System.Threading.Tasks;
	using global::Relativity.Services.Agent;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class PostInstallServiceIntegrationTests
	{
		private PostInstallService postInstallService;
		private IAgentManager agentManager;

		[SetUp]
		public void Setup()
		{
			this.agentManager = new ApiClientHelper().GetKeplerServiceReference<IAgentManager>().Value;
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var agentRepository = new AgentRepository(connectionFactory);
			var logger = TestUtilities.GetMockLogger().Object;
			var agentManagerService = new AgentManagerService(this.agentManager, agentRepository, logger);

			
			var resourceServerRepository = new ResourceServerRepository(connectionFactory);
			var refreshServerService = new RefreshServerService(logger, resourceServerRepository);
			
			var serverRepository = new ServerRepository(connectionFactory);
			var configurationRepository = new ConfigurationRepository(connectionFactory);

			this.postInstallService = new PostInstallService(agentManagerService, refreshServerService, serverRepository, configurationRepository, logger);
		}

		[Test]
		public async Task RunEveryTimeAsync()
		{
			var result = await this.postInstallService.RunEveryTimeAsync();

			Assert.That(result.Success, Is.True);
		}

		[TearDown]
		public void TearDown()
		{
			this.agentManager?.Dispose();
		}
	}
}
