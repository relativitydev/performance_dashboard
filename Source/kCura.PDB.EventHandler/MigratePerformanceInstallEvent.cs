namespace kCura.PDB.EventHandler
{
	using kCura.EventHandler;
	using kCura.EventHandler.CustomAttributes;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Interfaces;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;

	[kCura.EventHandler.CustomAttributes.RunOnce(false)]
	[kCura.EventHandler.CustomAttributes.Description("Deploys and manages updates for EDDSPerformance database during installation")]
	[System.Runtime.InteropServices.Guid("C26B5BBE-93DC-4B7A-B8EF-F82EEC983313")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.InstanceAndWorkspace)]
	public class MigratePerformanceInstallEvent : PostInstallEventHandler
	{
		private IParallelizationInstallationService parallelizationInstallationService;

		public MigratePerformanceInstallEvent()
		{
		}

		public MigratePerformanceInstallEvent(IParallelizationInstallationService parallelizationInstallationService)
		{
			this.parallelizationInstallationService = parallelizationInstallationService;
		}

		public override Response Execute()
		{
			// Initialize application install service -- TODO -- use DI to create kernel and get correct implementation
			if (this.parallelizationInstallationService == null)
			{
				// Build up application installation service
				var logger = new TextLogger();
				var connectionFactory = new HelperConnectionFactory(this.Helper);
				var sqlRepo = new SqlServerRepository(connectionFactory);
				var hashConverter = new LegacyHashConversionService();
				var tabRepository = new TabRepository(connectionFactory);
				var relativityOneService = new RelativityOneService(sqlRepo.ConfigurationRepository);
				var tabService = new TabService(tabRepository, relativityOneService);
				var databaseMigratorFactory = new DatabaseMigratorFactory(connectionFactory);
				var databaseDeployer = new DatabaseDeployer(databaseMigratorFactory, new ErrorOnlyMigrationResultHandler(logger));
				var applicationInstallationService = new ApplicationInstallationService(connectionFactory, sqlRepo, hashConverter, tabService, databaseDeployer, logger);

				var toggleProvider = new PdbSqlToggleProvider(connectionFactory);
				var forceDeparallelization = toggleProvider.IsEnabled<DatabaseDeploymentDeparallelizationToggle>();
				if (forceDeparallelization)
				{
					// Build up the service that checks for version
					var versionCheckService = new VersionCheckService(new PdbVersionRepository(connectionFactory), logger);
					this.parallelizationInstallationService = new SingletonDatabaseInstallationService(applicationInstallationService, versionCheckService, new TimeService(), logger);
				}
				else
				{
					// Build up the service that actually runs the install process
					this.parallelizationInstallationService = new DefaultDatabaseInstallationService(applicationInstallationService);
				}
			}

			// Install the application
			var appInstallResponse = this.parallelizationInstallationService.InstallApplication(this.Helper.GetActiveCaseID()).GetAwaiter().GetResult();

			// Return response
			return new Response { Message = appInstallResponse.Message, Success = appInstallResponse.Success };
		}
	}
}
