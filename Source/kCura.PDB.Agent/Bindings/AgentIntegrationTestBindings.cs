namespace kCura.PDB.Agent.Bindings
{
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Service.Testing;

	using Ninject.Modules;

	public class AgentIntegrationTestBindings : NinjectModule
	{
		public override void Load()
		{
			// Agent Logic Execution Rebindings
			this.Rebind<IServerRepository>().To<IntegrationTestNewServerRepository>();
			this.Rebind<IHourRepository>().To<IntegrationTestNewHourRepository>();
			this.Rebind<IDatabaseRepository>().To<IntegrationTestNewDatabaseRepository>();
			this.Rebind<IMetricSystemManagerService>().To<IntegrationTestMetricManagerLogic>();
			this.Rebind<IEventSourceService>().To<IntegrationTestEventSourceService>();
			//this.Rebind<ITestEventTypeProvider>().To<BackupDbccTestEventTypeProvider>(); // todo - Re-evaluate how this gets bound up
			this.Rebind<IBackupProvider>().To<IntegrationTestBackupProvider>();
			this.Rebind<IDbccProvider>().To<IntegrationTestDbccProvider>();
		}
	}
}
