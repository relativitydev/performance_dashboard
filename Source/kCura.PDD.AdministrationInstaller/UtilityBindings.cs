namespace kCura.PDD.AdministrationInstaller
{
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using Ninject;
	using Ninject.Modules;

	public class UtilityBindings : NinjectModule
	{
		public override void Load()
		{
			Bind<ILogger>().To<TextLogger>().InSingletonScope();
			Bind<TextLogger>().ToMethod(c => c.Kernel.Get<ILogger>() as TextLogger).InSingletonScope();
			Bind<IConnectionFactory>().ToMethod(c => new ConfiguredConnectionFactory(new AppSettingsConfigurationService()));

			this.Bind<IMigrationResultHandler>().To<ExceptionMigrationResultHandler>();
		}
	}
}
