namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class EventSourceService : IEventSourceService
	{
		private readonly IEventRepository eventRepository;
		private readonly IQueuingService queuingService;
		private readonly IConfigurationRepository configurationRepository;
		private readonly ICategoryRepository categoryRepository;

		public EventSourceService(
			IEventRepository eventRepository,
			IQueuingService queuingService,
			IConfigurationRepository configurationRepository,
			ICategoryRepository categoryRepository)
		{
			this.eventRepository = eventRepository;
			this.queuingService = queuingService;
			this.configurationRepository = configurationRepository;
			this.categoryRepository = categoryRepository;
		}

		public virtual async Task CreateHourProcessingEvents()
		{
			// Reads if there are any pending or active hour bootstrap events
			var pendingOrActiveEventTypes = EventConstants.ActiveEventStatuses.Concat(new[] { EventStatus.Pending }).ToList();

			var pendingOrActiveHourBootstrapEventTypes = await this.eventRepository.ReadTypesByTypeAndStatusAsync(
				EventConstants.HourBootstrapEvents,
				pendingOrActiveEventTypes);

			// Only creates hour bootstrap events if there aren't any pending or active ones already
			if (!pendingOrActiveHourBootstrapEventTypes.Any())
			{
				await EventConstants.HourBootstrapEvents
					.Select(et => this.eventRepository.CreateAsync(new Event { SourceType = et }))
					.WhenAllStreamed();
			}
		}

		public virtual async Task EnqueueTasksForPendingEvents()
		{
			// there are any pending servers then we skip all tasks except hour prerequisites
			var eventSystemState = await this.eventRepository.ReadEventSystemStateAsync();
			if (eventSystemState == EventSystemState.Paused)
			{
				return;
			}

			// Get enqueue configuration value
			var numToEnqueue =
				Math.Max(
					await this.configurationRepository.ReadValueAsync<long>(ConfigurationKeys.NumberOfEventsToEnqueue) ??
					EventConstants.DefaultEventsToEnqueue, EventConstants.MinEventsToEnqueue);

			// Update to InProgress (marked as picked up)

			// Read first
			IList<long> eventsToEnqueue;
			if (eventSystemState == EventSystemState.Prerequisites)
			{
				var nonSingletonEventsToEnqueue = await this.eventRepository.ReadIdsByStatusAsync(
					EventStatus.Pending, numToEnqueue, EventConstants.SingletonEventTypes);

				var singletonPrereqEventTypes =
					EventConstants.PrerequisiteEvents.Intersect(EventConstants.SingletonEventTypes).ToList();

				var singletonEventsToEnqueue =
					await this.eventRepository.ReadSingleByTypeAndStatusAsync(
						singletonPrereqEventTypes, EventStatus.Pending, EventConstants.ActiveEventStatuses);

				eventsToEnqueue = nonSingletonEventsToEnqueue.Union(singletonEventsToEnqueue).ToList();
			}
			else
			{
				var singletonEventsToEnqueue =
					await this.eventRepository.ReadSingleByTypeAndStatusAsync(EventConstants.SingletonEventTypes, EventStatus.Pending, EventConstants.ActiveEventStatuses);

				var nonSingletonEventsToEnqueue = await this.eventRepository.ReadIdsByStatusAsync(EventStatus.Pending, numToEnqueue, EventConstants.SingletonEventTypes);

				eventsToEnqueue = nonSingletonEventsToEnqueue.Union(singletonEventsToEnqueue).ToList();
			}

			if (eventsToEnqueue.Any())
			{
				// Then enqueue the read
				await this.eventRepository.UpdateStatusAsync(eventsToEnqueue, EventStatus.PendingHangfire)
					.PipeAsync(this.EnqueueTasksForPendingEvents);
			}
		}

		// Only one source should be doing this at a time.  If we grab duplicate events at once, we can cause issues
		internal async Task EnqueueTasksForPendingEvents(IList<Event> previouslyPendingEvents)
		{
			var eventsToEnqueue = previouslyPendingEvents.DistinctBy(e => new { e.SourceType, e.SourceId }).ToList();
			var duplicateEventTasks = previouslyPendingEvents.Except(eventsToEnqueue)
				.Select(e =>
					{
						e.Status = EventStatus.Duplicate;
						return this.eventRepository.UpdateAsync(e);
					});
			eventsToEnqueue.ForEach(e => this.queuingService.Enqueue<IEventHandler>(t => t.HandleEvent(e.Id, e.SourceType), e.Delay));
			await duplicateEventTasks.WhenAllStreamed();
		}

		public async Task CreateNextEvents(IList<int> sourceIds, IList<EventSourceType> eventSourceTypes, int? delay, long? previousEventId, int? hourId)
		{
			var nextTasks =
					sourceIds?.SelectMany(id => eventSourceTypes.Select(eventType =>
						new Event
						{
							SourceId = id,
							SourceType = eventType,
							PreviousEventId = previousEventId,
							Delay = delay,
							HourId = hourId
						})).ToList()
					?? eventSourceTypes.Select(eventType =>
						new Event
						{
							SourceType = eventType,
							PreviousEventId = previousEventId,
							Delay = delay,
							HourId = hourId
						}).ToList();

			await SetInitialHourIdForEvents(nextTasks);

			if (nextTasks.Any())
			{
				await this.eventRepository.CreateAsync(nextTasks);
			}
		}

		public static int GetIncreasingDelay(Event evnt) =>
			(int)Math.Min(30 * Math.Max(Math.Log(evnt?.Retries ?? 0) / Math.Log(1.75), 1), 15 * 60);

		internal async Task SetInitialHourIdForEvents(IList<Event> nextEvents)
		{
			nextEvents
				.Where(t => t.SourceType == EventSourceType.StartHour)
				.ForEach(t => t.HourId = t.SourceId);

			await nextEvents
				.Where(t => t.SourceType == EventSourceType.ScoreCategoryScore)
				.Select(async e => e.HourId = (await this.categoryRepository.ReadByCategoryScoreAsync(e.SourceId.Value)).HourId)
				.WhenAllStreamed();

		}
	}
}
