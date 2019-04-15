namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Threading.Tasks;
	using Hangfire;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;

	public class JobServerOptionsFactory : IJobServerOptionsFactory
	{
		private readonly IConfigurationRepository configurationRepository;

		public JobServerOptionsFactory(IConfigurationRepository configurationRepository)
		{
			this.configurationRepository = configurationRepository;
		}

		public BackgroundJobServerOptions GetOptions()
		{
			var shutdownTimeout = this.configurationRepository.ReadValue<int>(ConfigurationKeys.JobServerShutdownTimeoutSeconds);
			var workerCount = this.configurationRepository.ReadValue<int>(ConfigurationKeys.JobServerWorkerCount);

			return new BackgroundJobServerOptions
			{
				ServerName = $"{Environment.MachineName}",
				ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeout ?? Defaults.Queuing.DefaultServerTimeout),
				WorkerCount = workerCount ?? Defaults.Queuing.DefaultWorkerCount,
				Queues = new[] { Names.Queuing.Worker, Names.Queuing.DefaultQueue }
			};
		}
	}
}
