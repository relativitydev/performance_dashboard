namespace kCura.PDB.Agent.Tests
{
	using System.Collections.Generic;
	using global::Relativity.API;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using Ninject.Modules;

	public class AgentIntegrationModule : NinjectModule
	{
		public override void Load()
		{
			var agentService = new Mock<IAgentService>();
			var qosManager = new Mock<IQoSTaskManager>();
			agentService.SetupGet(s => s.AgentID).Returns(123456);
			this.Bind<IConnectionFactory>().ToConstant(TestUtilities.GetIntegrationConnectionFactory());
			this.Bind<IAgentService>().ToConstant(agentService.Object);
			this.Bind<IQoSTaskManager>().ToConstant(qosManager.Object);
			this.Bind<ISqlServerRepository>().To<SqlServerRepository>(); // Should resolve with the bound connectionFactory
			this.Bind<ILogger>().ToMethod(c =>
				new CompositeLogger(new List<ILogger>()
				{
						TestUtilities.GetMockLogger().Object,
						c.Kernel.Get<DatabaseLogger>()
				})).InTransientScope();
		}
	}
}
