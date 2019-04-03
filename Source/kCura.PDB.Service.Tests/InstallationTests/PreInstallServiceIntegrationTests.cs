namespace kCura.PDB.Service.Tests.InstallationTests
{
	using System;
	using System.Threading.Tasks;
	using global::Relativity.Services.Agent;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class PreInstallServiceIntegrationTests
	{
		private PreInstallService preInstallService;
		private IAgentManager agentManager;

		[SetUp]
		public void Setup()
		{
			var apiClientHelper = new ApiClientHelper();
			this.agentManager = apiClientHelper.GetKeplerServiceReference<IAgentManager>().Value;
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var agentRepository = new AgentRepository(connectionFactory);
			var logger = TestUtilities.GetMockLogger().Object;
			var agentManagerService = new AgentManagerService(this.agentManager, agentRepository, logger);
			this.preInstallService = new PreInstallService(agentManagerService);
		}

		[Test]
		public async Task RunEveryTimeAsync()
		{
			var result = await this.preInstallService.RunEveryTimeAsync();

			Assert.That(result.Success, Is.True);
		}

		[TearDown]
		public void TearDown()
		{
			this.agentManager?.Dispose();
		}
	}
}
