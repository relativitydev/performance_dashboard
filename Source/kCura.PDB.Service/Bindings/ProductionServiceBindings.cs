namespace kCura.PDB.Service.Bindings
{
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Queuing;
	using Ninject.Modules;

	public class ProductionServiceBindings : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IEventSourceService>().To<EventSourceService>();
			this.Bind<IServerRepository>().To<ServerRepository>();
			this.Bind<IHourRepository>().To<HourRepository>();
			this.Bind<IDatabaseRepository>().To<DatabaseRepository>();
			this.Bind<IMetricSystemManagerService>().To<MetricSystemManagerAgentLogic>();
			this.Bind<IBackupProvider>().To<BackupRepository>();
			this.Bind<IDbccProvider>().To<DbccRepository>();
		}
	}
}
