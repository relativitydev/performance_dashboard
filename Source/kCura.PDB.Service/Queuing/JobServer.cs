namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Hangfire;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class JobServer : BackgroundJobServer, IJobServer
	{
		private readonly IAgentRepository agentRepository;
		private readonly IConfigurationRepository configurationRepository;
		private readonly ILogger logger;
		private readonly IAgentService agentService;
		private readonly ITimeService timeService;

		public JobServer(
			IAgentRepository agentRepository,
			IConfigurationRepository configurationRepository,
			ILogger logger,
			IAgentService agentService,
			IJobServerOptionsFactory jobServerOptionsFactory,
			ITimeService timeService)
			: base(jobServerOptionsFactory.GetOptions())
		{
			this.agentRepository = agentRepository;
			this.logger = logger.WithTypeName(this); //.WithClassName(); -- doesn't work
			this.agentService = agentService;
			this.configurationRepository = configurationRepository;
			this.timeService = timeService;
		}

		public async Task WaitTillProcessesAreDone(CancellationToken cancellationToken)
		{
			await this.logger.LogVerboseAsync($"Started Hangfire Server on agent: {this.agentService.Name} -- id: {this.agentService.AgentID}");

			// Get configuration variables and clamp them to min value
			var timeToSleep =
				Math.Max(
					this.configurationRepository.ReadValue<int>(ConfigurationKeys.JobServerSleepTime) ?? Defaults.Queuing.DefaultServerSleep,
					Defaults.Queuing.MinServerSleep);

			var jobServerMaxExecutionTime =
				Math.Max(
					this.configurationRepository.ReadValue<int>(ConfigurationKeys.JobServerMaxExecution) ?? Defaults.Queuing.DefaultServerExecution,
					Defaults.Queuing.MinServerExecution);

			await this.logger.LogVerboseAsync(
				$"Configuration for agent {this.agentService.Name} -- Sleep time seconds {timeToSleep}, Max uptime {jobServerMaxExecutionTime}");

			// Initialize max execution time
			var maxTimeout = this.timeService.GetUtcNow().AddSeconds(jobServerMaxExecutionTime);
			do
			{
				// Sleep
				await this.timeService.Delay(TimeSpan.FromSeconds(timeToSleep), cancellationToken);

				// Check if agent is disabled or timeout has been reached
			    var agentEnabled = this.agentRepository.ReadAgentEnabled(this.agentService.AgentID);

				if ((agentEnabled == false) || maxTimeout < this.timeService.GetUtcNow() || cancellationToken.IsCancellationRequested)
				{
					break;
				}

				// TODO - get count of in-progress jobs from hangfire or events table
				await this.logger.LogVerboseAsync($"Currently processing jobs on agent {this.agentService.Name} -- id {this.agentService.AgentID}");
			}
			while (true);
			await this.logger.LogVerboseAsync($"Stopping Hangfire Server on agent {this.agentService.Name} -- id {this.agentService.AgentID}");
		}
	}
}