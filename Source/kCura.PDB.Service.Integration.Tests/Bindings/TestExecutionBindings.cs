namespace kCura.PDB.Service.Integration.Tests.Bindings
{
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;

	using Moq;

	using Ninject.Modules;

	public class TestExecutionBindings : NinjectModule
	{
		public override void Load()
		{
			// Connection Factory
			this.Bind<IConnectionFactory>().ToMethod(c => new ConfiguredConnectionFactory(new AppSettingsConfigurationService()));

			// Logger (like Agent)
			this.Rebind<ILogger>().ToConstant(TestUtilities.GetMockLogger().Object);

			// Test Execution Agent
			var testAgentId = 999999;
			var agentService = new Mock<IAgentService>();
			agentService.SetupGet(m => m.AgentID).Returns(testAgentId);
			this.Rebind<IAgentService>().ToConstant(agentService.Object);

			var agentRepository = new Mock<IAgentRepository>();
			agentRepository.Setup(m => m.ReadAgentEnabled(testAgentId)).Returns(true);
			this.Rebind<IAgentRepository>().ToConstant(agentRepository.Object);
		}
	}
}
