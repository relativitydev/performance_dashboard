namespace kCura.PDB.Service.Testing
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;

	public class IntegrationTestMetricManagerLogic : IMetricSystemManagerService
	{
		private readonly IEventSourceService eventSourceService;
		private readonly IQueuingConfiguration queuingConfiguration;

		public IntegrationTestMetricManagerLogic(
			IEventSourceService eventSourceService,
			IQueuingConfiguration queuingConfiguration)
		{
			this.eventSourceService = eventSourceService;
			this.queuingConfiguration = queuingConfiguration;
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			this.queuingConfiguration.ConfigureSystem();
			var enqueueEventsTimeDelay = 500;
			var enqueueEventsTimeout = this.GetTimeout(enqueueEventsTimeDelay);
			var createProcessingEventsTimeDelay = 500;
			var createProcessingEventsTimeout = this.GetTimeout(createProcessingEventsTimeDelay);

			while (cancellationToken.IsCancellationRequested == false)
			{
				if (enqueueEventsTimeout.IsAfterTimedOut)
				{
					// Enqueue tasks
					await this.eventSourceService.EnqueueTasksForPendingEvents();

					enqueueEventsTimeout.Reset();
				}

				if (createProcessingEventsTimeout.IsAfterTimedOut)
				{
					await this.eventSourceService.CreateHourProcessingEvents();

					createProcessingEventsTimeout.Reset();
				}

				// delay for a bit
				var delay = new[] { enqueueEventsTimeout.TimeRemaining, createProcessingEventsTimeout.TimeRemaining }.Min();
				if (delay.TotalMilliseconds > 0)
				{
					try
					{
						await Task.Delay(delay, cancellationToken);
					}
					catch (TaskCanceledException)
					{
						break;
					}
				}
			}
		}

		internal ITimeout GetTimeout(int interval)
		{
			return new Services.Timeout(TimeSpan.FromMilliseconds(interval));
		}
	}
}
