namespace kCura.PDB.Service.Testing
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;

	public class IntegrationTestEventSourceService : EventSourceService
	{
		private readonly ITestEventTypeProvider testEventTypeProvider;
		private readonly IEventRepository eventRepository;
		private readonly IQueuingService queuingService;
		
		public IntegrationTestEventSourceService(
			ITestEventTypeProvider testEventTypeProvider,
			IEventRepository eventRepository,
			IQueuingService queuingService,
			IConfigurationRepository configurationRepository,
			ICategoryRepository categoryRepository)
			: base(eventRepository, queuingService, configurationRepository, categoryRepository)
		{
			this.testEventTypeProvider = testEventTypeProvider;
			this.eventRepository = eventRepository;
			this.queuingService = queuingService;
		}

		public override async Task EnqueueTasksForPendingEvents()
		{
			// Don't care about system state

			// Don't care about configurable number to enqueue in a test context

			// How does the test determine which events to care about?  Inject a service that's in charge of that?
			var typesToEnqueue = this.testEventTypeProvider.GetEventTypesToEnqueue();
			
			// invert list
			var typesToExclude = EventConstants.AllEventTypes.Except(typesToEnqueue).ToList();

			// Enqueue the events
			var eventsIdsToUpdate = await this.eventRepository.ReadIdsByStatusAsync(EventStatus.Pending, 5000, typesToExclude);
			var result = await this.eventRepository.UpdateStatusAsync(eventsIdsToUpdate, EventStatus.PendingHangfire)
				             .PipeAsync(this.EnqueueTasksForPendingEvents);
			await result;
		}

		public override Task CreateHourProcessingEvents()
		{
			// Of the original 'HourProcessing' events, we only need
			// to create the event for getting the category scores scored
			return this.eventRepository.CreateAsync(new Event { SourceType = EventSourceType.FindNextCategoriesToScore });
		}

		internal new async Task EnqueueTasksForPendingEvents(IList<Event> previouslyPendingEvents)
		{
			var eventsToEnqueue = previouslyPendingEvents.DistinctBy(e => new { e.SourceType, e.SourceId }).ToList();
			var duplicateEventTasks = previouslyPendingEvents.Except(eventsToEnqueue)
				.Select(e =>
				{
					e.Status = EventStatus.Duplicate;
					return this.eventRepository.UpdateAsync(e);
				});
			eventsToEnqueue.ForEach(e =>
			{
				this.queuingService.Enqueue<IEventHandler>(t => t.HandleEvent(e.Id, e.SourceType), e.Delay);
			});
			await duplicateEventTasks.WhenAllStreamed();
		}
	}
}
