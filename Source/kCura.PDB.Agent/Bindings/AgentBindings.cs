namespace kCura.PDB.Agent.Bindings
{
	using global::Relativity.API;

	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Queuing;
	using Ninject.Modules;

	public class AgentBindings : NinjectModule
	{
		public AgentBindings(IAgentService agentService)
		{
			this.agentService = agentService;
		}

		private readonly IAgentService agentService;

		public override void Load()
		{
			this.Bind<IAgentService>().ToConstant(this.agentService);
			this.Bind<IConnectionFactory>().To<HelperConnectionFactory>();
			this.Bind<IPDDModelDataContext>().ToConstructor(c => new PDDModelDataContext()).InSingletonScope();
			this.Rebind<ILogger>().ToMethod(LoggerFactory.GetLogger);

			this.Bind<DatabaseLogger>()
				.ToConstructor(x => new DatabaseLogger(
						x.Inject<ILogRepository>(),
						x.Inject<ILogService>(),
						x.Inject<IAgentService>()));
		}
	}
}
