namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class CheckEventTask : IEventTask
	{
		public CheckEventTask(IEventRepository eventRepository, IEventRouter eventRouter, ILogger logger, IEventSourceService eventSourceService)
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

		public async Task<EventResult> ProcessEvent(Event evnt)
		{
			await this.logger.LogVerboseAsync($"Starting to process check event: {evnt.Id} - {evnt?.SourceType}");

			LoopEventResult result;
			var stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				result = await this.eventRouter.RouteEvent(evnt) as LoopEventResult;
				stopwatch.Stop();
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				result = new LoopEventResult(false) { Succeeded = false };
				this.logger.LogError($"Event run failed: {evnt.Id} - Type: {evnt.SourceType}", ex);
			}

			// Mark Error or Completed
			result.ExecutionTime = (int)stopwatch.Elapsed.TotalMilliseconds;		

			return result;
		}

		public async Task MarkEventResultAsync(EventResult result, Event evnt)
		{
			evnt.ExecutionTime = result.ExecutionTime;
			evnt.Status = result.Succeeded ? EventStatus.Completed : EventStatus.Error;
			evnt.Retries = result.Succeeded ? evnt.Retries : (evnt.Retries ?? 0) + 1;
			await this.eventRepository.UpdateAsync(evnt);

			await this.logger.LogVerboseAsync($"Completed to processing check event: {evnt.Id} - {evnt?.SourceType} with {evnt?.Status} and Success Result {{{result?.Succeeded}}}");
		}

		/// <summary>
		/// For the check events `ShouldContinue=true` indicates that the event should move onto the next task(s)
		/// and `ShouldContinue=false` indicates that we loop on the same event type.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="previousEvent"></param>
		/// <returns></returns>
		public async Task CreateNextEvents(EventResult result, Event previousEvent)
		{
			// result.Succeeded is false only when the event errors and in that case we don't mark it errored and don't continue.
			if (!result.Succeeded)
			{
				await this.logger.LogVerboseAsync("Event failed. No Next Events.");
				return;
			}

			// Grab next types if result should continue, else repeat event type
			var nextTypes = result.ShouldContinue ? EventConstants.GetNextEvents(previousEvent.SourceType) : new[] { previousEvent.SourceType };

			// Convert previous source id to array
			var nextSourceIds = previousEvent.SourceId.HasValue ? new[] { previousEvent.SourceId.Value } : null;
			await this.logger.LogVerboseAsync($"Next event with types {nextTypes.Join("{0},{1}")}");

			// Create next events
			await this.eventSourceService.CreateNextEvents(nextSourceIds, nextTypes, EventSourceService.GetIncreasingDelay(previousEvent), previousEvent.Id, previousEvent.HourId);
		}
	}
}
