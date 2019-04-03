namespace kCura.PDB.EventHandler.Bindings
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Logging;
	using Ninject.Modules;

	public class EventHandlerBindings : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IConnectionFactory>().To<HelperConnectionFactory>();
			this.Bind<IPDDModelDataContext>().ToConstructor(c => new PDDModelDataContext()).InSingletonScope();
			this.Rebind<ILogger>().ToMethod(LoggerFactory.GetEventHandlerLogger);

			this.Bind<DatabaseLogger>()
				.ToConstructor(x => new DatabaseLogger(
					x.Inject<ILogRepository>(),
					x.Inject<ILogService>()));
		}
	}
}