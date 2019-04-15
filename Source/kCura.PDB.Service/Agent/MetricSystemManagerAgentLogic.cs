namespace kCura.PDB.Service.Agent
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;
	using Timeout = kCura.PDB.Service.Services.Timeout;

	public class MetricSystemManagerAgentLogic : IMetricSystemManagerService
	{
		private readonly IQueuingConfiguration queuingConfiguration;
		private readonly IEventSourceService eventSourceService;
		private readonly IConfigurationRepository configurationRepository;
		private readonly ILogger logger;
		private readonly IAgentService agentService;
		private readonly IAgentRepository agentRepository;
		private readonly IEventOrphanService eventOrphanService;
		private readonly IMetricManagerStatsRepository metricManagerStatsRepository;

		public MetricSystemManagerAgentLogic(
			IQueuingConfiguration queuingConfiguration,
			IEventSourceService eventSourceService,
			IConfigurationRepository configurationRepository,
			ILogger logger,
			IAgentService agentService,
			IAgentRepository agentRepository,
			IEventOrphanService eventOrphanService,
			IMetricManagerStatsRepository metricManagerStatsRepository)
		{
			this.queuingConfiguration = queuingConfiguration;
			this.eventSourceService = eventSourceService;
			this.configurationRepository = configurationRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.System);
			this.agentService = agentService;
			this.agentRepository = agentRepository;
			this.eventOrphanService = eventOrphanService;
			this.metricManagerStatsRepository = metricManagerStatsRepository;
		}

		// TODO We need to abstract this better but that can wait till next release
		public async Task Execute(CancellationToken cancellationToken)
		{
			await this.logger.LogVerboseAsync("Starting metric manager logic.");
			var executionInfo = new MetricManagerExecutionInfo();

			cancellationToken.ThrowIfCancellationRequested();

			// TODO: add logic to get ensure this is first manager agent
			DataSetup.Setup();
			this.queuingConfiguration.ConfigureSystem();

			await this.logger.LogVerboseAsync("Configured metric manager.");

			// explicitly call the first time
			using (executionInfo.AsMeter(e => e.InitialWork))
			{
				using (var stopwatch = Stopwatch.StartNew().AsMeter(executionInfo, e => e.ResolveOrphanedEventLocks))
				{
					await this.eventOrphanService.ResolveOrphanedEventLocks();
					await this.logger.LogVerboseAsync(
						$"Completed initial resolving orphaned event locks. {stopwatch.Elapsed.TotalSeconds:N1}s");
				}

				using (var stopwatch = Stopwatch.StartNew().AsMeter(executionInfo, e => e.ResolveTimedOutEvents))
				{
					await this.eventOrphanService.ResolveTimedOutEvents();
					await this.logger.LogVerboseAsync(
						$"Completed initial resolving timed out events. {stopwatch.Elapsed.TotalSeconds:N1}s");
				}

				using (var stopwatch = Stopwatch.StartNew().AsMeter(executionInfo, e => e.CreateHourProcessingEvents))
				{
					await this.eventSourceService.CreateHourProcessingEvents();
					await this.logger.LogVerboseAsync(
						$"Completed creating hour processing events. {stopwatch.Elapsed.TotalSeconds:N1}s");
				}

				using (var stopwatch = Stopwatch.StartNew().AsMeter(executionInfo, e => e.EnqueueTasksForPendingEvents))
				{
					await this.eventSourceService.EnqueueTasksForPendingEvents();
					await this.logger.LogVerboseAsync(
						$"Completed enqueuing task for pending events. {stopwatch.Elapsed.TotalSeconds:N1}s");
				}
			}

			await this.logger.LogVerboseAsync($"Completed initial tasks: {executionInfo.ToString()}");
			await this.metricManagerStatsRepository.CreateAsync(executionInfo.ToStats());

			// Create timeouts
			var createBootstrapEventsTimeout =
				await this.GetTimeout(ConfigurationKeys.CreateBootstrapEventsInterval, EventConstants.DefaultCreateBootstrapEventsInterval, EventConstants.MinCreateBoostrapEventsInterval);
			var resolveOrphanedEventsTimeout =
				await this.GetTimeout(ConfigurationKeys.ResolveOrphanedEventsInterval, EventConstants.DefaultResolveOrphanedEventsInterval, EventConstants.MinResolveOrphanedEventsInterval);
			var enqueueTasksTimeout =
				await this.GetTimeout(ConfigurationKeys.EnqueueTasksInterval, EventConstants.DefaultEnqueueTasksInterval, EventConstants.MinEnqueueTasksInterval, false);
			var managerRunTimeout =
				await this.GetTimeout(ConfigurationKeys.EventManagerRunInterval, EventConstants.DefaultManagerRunInterval, EventConstants.MinManagerRunInterval);
			var meterReportTimeout =
				await this.GetTimeout(ConfigurationKeys.ManagerMeterReportTimeout, EventConstants.DefaultManagerMeterReportTimeout, EventConstants.MinManagerMeterReportTimeout);

			await this.logger.LogVerboseAsync($"Starting main manager loop");
			using (var managerLoopMeter = executionInfo.AsMeter(e => e.ManagerMainLoops))
			{
				while (!managerRunTimeout.IsAfterTimedOut && !cancellationToken.IsCancellationRequested)
				{
					managerLoopMeter.Increment();

					if (meterReportTimeout.IsAfterTimedOut)
					{
						await this.logger.LogVerboseAsync($"Main manager loop: {executionInfo.ToString()}");
						await this.metricManagerStatsRepository.CreateAsync(executionInfo.ToStats());
						meterReportTimeout.Reset();
					}

					if (createBootstrapEventsTimeout.IsAfterTimedOut)
					{
						using (executionInfo.AsMeter(e => e.CreateHourProcessingEvents))
						{
							await this.eventSourceService.CreateHourProcessingEvents();
						}

						createBootstrapEventsTimeout.Reset();
					}

					if (resolveOrphanedEventsTimeout.IsAfterTimedOut)
					{
						using (executionInfo.AsMeter(e => e.ResolveOrphanedEventLocks))
						{
							await this.eventOrphanService.ResolveOrphanedEventLocks();
						}

						using (executionInfo.AsMeter(e => e.ResolveTimedOutEvents))
						{
							await this.eventOrphanService.ResolveTimedOutEvents();
						}

						resolveOrphanedEventsTimeout.Reset();
					}

					if (enqueueTasksTimeout.IsAfterTimedOut)
					{
						using (executionInfo.AsMeter(e => e.EnqueueTasksForPendingEvents))
						{
							await this.eventSourceService.EnqueueTasksForPendingEvents();
						}

						enqueueTasksTimeout.Reset();
					}



					using (executionInfo.AsMeter(e => e.CheckIfAgentIsDisabled))
					{
					    var agentExists = this.agentRepository.ReadAgentEnabled(this.agentService.AgentID);

                        if (!agentExists)
						{
							await this.logger.LogVerboseAsync($"Read manager agent as disabled. AgentId {this.agentService.AgentID}");
							break;
						}
					}

					var delay = new[]
					{
						createBootstrapEventsTimeout.TimeRemaining,
						resolveOrphanedEventsTimeout.TimeRemaining,
						enqueueTasksTimeout.TimeRemaining,
						managerRunTimeout.TimeRemaining
					}.Min();
					executionInfo.TimeoutDelays.Increment(delay);
					if (delay.TotalMilliseconds > 0)
					{
						await Task.Delay(delay, cancellationToken);
					}
				}
			}

			await this.metricManagerStatsRepository.CreateAsync(executionInfo.ToStats());
			await this.logger.LogVerboseAsync($"Stopping manager. {executionInfo.ToString()}");
		}

		internal async Task<ITimeout> GetTimeout(string configKey, int defaultInterval, int minInterval, bool useSeconds = true)
		{
			var configuredTimeout = await this.configurationRepository.ReadValueAsync<int>(configKey);
			var interval = Math.Max(configuredTimeout ?? defaultInterval, minInterval);
			return new Timeout(useSeconds ? TimeSpan.FromSeconds(interval) : TimeSpan.FromMilliseconds(interval));
		}
	}
}