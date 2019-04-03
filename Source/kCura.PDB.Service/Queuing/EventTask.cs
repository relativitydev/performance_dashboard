namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class EventTask : IEventTask
	{
		private const int MaxRetries = 3;

		public EventTask(IEventRepository eventRepository, IEventRouter eventRouter, ILogger logger, IEventSourceService eventSourceService)
		{
			this.eventRepository = eventRepository;
			this.eventRouter = eventRouter;
			this.eventSourceService = eventSourceService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Event);
		}

		private readonly IEventRepository eventRepository;
		private readonly IEventRouter eventRouter;
		private readonly IEventSourceService eventSourceService;
		private readonly ILogger logger;

		/// <inheritdoc />
		public async Task<EventResult> ProcessEvent(Event evnt)
		{
			await this.logger.LogVerboseAsync($"Starting to process event: {evnt.Id} - {evnt?.SourceType}");

			EventResult result;
			var stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				result = await this.eventRouter.RouteEvent(evnt);
				stopwatch.Stop();
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				result = EventResult.Stop;
				result.Succeeded = false;

				if (this.ShouldRetryEvent(evnt))
				{
					await this.logger.LogWarningAsync($"Event run failed: {evnt.Id} - Type: {evnt.SourceType}", ex);
				}
				else
				{
					this.logger.LogError($"Event run failed: {evnt.Id} - Type: {evnt.SourceType}", ex);
				}
				
			}

			result.ExecutionTime = (int?)stopwatch.Elapsed.TotalMilliseconds;

			return result;
		}

		public async Task MarkEventResultAsync(EventResult result, Event evnt)
		{
			evnt.ExecutionTime = result.ExecutionTime;

			// Mark Error, Pending, or Completed
			if (result.Succeeded)
			{
				evnt.Status = EventStatus.Completed;
			}
			else
			{
				evnt.Status = this.ShouldRetryEvent(evnt) ? EventStatus.Pending : EventStatus.Error;
				evnt.Retries = (evnt.Retries ?? 0) + 1;
			}
			
			await this.eventRepository.UpdateAsync(evnt);

			await this.logger.LogVerboseAsync($"Completed to processing event: {evnt.Id} - {evnt?.SourceType} with {evnt?.Status}");
		}

		public async Task CreateNextEvents(EventResult result, Event previousEvent)
		{
			if (!result.Succeeded)
			{
				await this.logger.LogVerboseAsync("Event failed. No Next Events.");
				return;
			}

			// if the result is then we never create any next events
			if (!result.ShouldContinue) return;

			result.Types =
				(result.Types != null && result.Types.Any())
				? result.Types
				: EventConstants.GetNextEvents(previousEvent.SourceType).ToList();
			await this.logger.LogVerboseAsync($"Next event source ids: {result?.SourceIds?.Join("{0},{1}")} and with types {result?.Types?.Join("{0},{1}")}; Previous event: {previousEvent.Id}");
			await this.eventSourceService.CreateNextEvents(result.SourceIds, result.Types, result.Delay, previousEvent.Id, previousEvent.HourId);
		}

		internal bool ShouldRetryEvent(Event evnt) => (evnt.Retries ?? 0) < MaxRetries;
	}
}
