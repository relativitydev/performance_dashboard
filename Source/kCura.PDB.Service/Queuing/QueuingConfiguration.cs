namespace kCura.PDB.Service.Queuing
{
	using System;
	using Core.Interfaces.Queuing;
	using Core.Interfaces.Services;
	using Hangfire;
	using Hangfire.Logging;
	using Hangfire.Logging.LogProviders;
	using Hangfire.SqlServer;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using Ninject;

	public class QueuingConfiguration : IQueuingConfiguration
	{
		public QueuingConfiguration(IConnectionFactory connectionFactory, IKernel kernel, IConfigurationRepository configurationRepository)
		{
			this.connectionFactory = connectionFactory;
			this.kernel = kernel;
			this.configurationRepository = configurationRepository;
		}

		public QueuingConfiguration(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;
		private readonly IKernel kernel;
		private readonly IConfigurationRepository configurationRepository;

		public void ConfigureSystem()
		{
			var queuePollInterval =
				Math.Max(
					this.configurationRepository?.ReadValue<int>(ConfigurationKeys.QueuePollInterval) ?? Defaults.Queuing.DefaultQueuePollInterval,
					Defaults.Queuing.MinQueuePollInterval);
			var connectionString = this.connectionFactory.GetEddsPerformanceConnection().ConnectionString;
			LogProvider.SetCurrentLogProvider(new ColouredConsoleLogProvider());
			var sqlHangfireOptions = new SqlServerStorageOptions
			{
				QueuePollInterval = TimeSpan.FromSeconds(queuePollInterval)
			};
			GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString, sqlHangfireOptions);
			GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
			if (this.kernel != null)
				GlobalConfiguration.Configuration.UseNinjectActivator(this.kernel);
		}
	}
}
