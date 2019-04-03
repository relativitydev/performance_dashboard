namespace kCura.PDB.Service
{
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.CategoryScoring;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Metrics.InfrastructurePerformance;
	using kCura.PDB.Service.Metrics.RecoverabilityIntegrity;
	using kCura.PDB.Service.Metrics.Uptime;
	using kCura.PDB.Service.Metrics.UserExperience;
	using kCura.PDB.Service.Services;
	using Ninject.Modules;

	public class ServiceBindings : NinjectModule
	{
		public override void Load()
		{
			// Bind a list of all the metric logics
			this.Bind<IMetricLogic>().To<AgentUptimeMetricLogic>();
			this.Bind<IMetricLogic>().To<WebUptimeMetricLogic>();
			this.Bind<IMetricLogic>().To<AuditAnalysisMetricLogic>();
			this.Bind<IMetricLogic>().To<RamMetricLogic>();
			this.Bind<IMetricLogic>().To<VirtualLogFileCountMetricLogic>();
			this.Bind<IMetricLogic>().To<WaitsMetricLogic>();
			this.Bind<IMetricLogic>().To<BackupGapMetricLogic>();
			this.Bind<IMetricLogic>().To<BackupCoverageMetricLogic>();
			this.Bind<IMetricLogic>().To<BackupFrequencyMetricLogic>();
			this.Bind<IMetricLogic>().To<DbccGapMetricLogic>();
			this.Bind<IMetricLogic>().To<DbccCoverageMetricLogic>();
			this.Bind<IMetricLogic>().To<DbccFrequencyMetricLogic>();
			this.Bind<IMetricLogic>().To<RpoMetricLogic>();
			this.Bind<IMetricLogic>().To<RtoMetricLogic>();

			// Bind a list of all the category score logics
			this.Bind<ICategoryScoringLogic>().To<UptimeScoringLogic>();
			this.Bind<ICategoryScoringLogic>().To<UserExperienceScoringLogic>();
			this.Bind<ICategoryScoringLogic>().To<InfrastructurePerformanceScoringLogic>();
			this.Bind<ICategoryScoringLogic>().To<RecoverabilityIntegrityScoringLogic>();

			// Bind a list of all the category complete logics
			this.Bind<ICategoryCompleteLogic>().To<UptimeCategoryComplete>();
			this.Bind<ICategoryCompleteLogic>().To<UserExperienceCategoryComplete>();
			this.Bind<ICategoryCompleteLogic>().To<InfrastructurePerformanceCategoryComplete>();
			this.Bind<ICategoryCompleteLogic>().To<RecoverabilityIntegrityCategoryComplete>();

			// Service mapping bindings
			this.Bind<IServiceFactory<IMetricLogic, MetricType>>()
				.To<AttributeServiceFactory<IMetricLogic, MetricType>>();
			this.Bind<IServiceFactory<IMetricReadyForDataCollectionLogic, MetricType>>()
				.To<AttributeServiceFactory<IMetricReadyForDataCollectionLogic, MetricType>>();
			this.Bind<IServiceFactory<ICategoryScoringLogic, CategoryType>>()
				.To<AttributeServiceFactory<ICategoryScoringLogic, CategoryType>>();
			this.Bind<IServiceFactory<ICategoryCompleteLogic, CategoryType>>()
				.To<AttributeServiceFactory<ICategoryCompleteLogic, CategoryType>>();

			// Other bindings
			this.Bind<IToggleProvider>().To<PdbSqlToggleProvider>();
			this.Bind<IMigrationResultHandler>().To<ExceptionMigrationResultHandler>();
			this.Bind<IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService>>().To<SqlWorkspaceAuditServiceProvider>();
		}
	}
}
