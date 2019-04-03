namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class EventOrphanService : IEventOrphanService
	{
		private readonly IEventLockRepository eventLockRepository;
		private readonly IEventRepository eventRepository;
		private readonly IEventWorkerRepository eventWorkerRepository;
		private readonly IAgentRepository agentRepository;
		private readonly ILogger logger;

		public EventOrphanService(
			IEventLockRepository eventLockRepository,
			IEventRepository eventRepository,
			IEventWorkerRepository eventWorkerRepository,
			IAgentRepository agentRepository,
			ILogger logger)
		{
			this.eventLockRepository = eventLockRepository;
			this.eventRepository = eventRepository;
			this.eventWorkerRepository = eventWorkerRepository;
			this.agentRepository = agentRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Event);
		}

		/// <inheritdoc />
		public async Task ResolveOrphanedEventLocks()
		{
			var agentsTask = this.agentRepository.ReadAllEnabledAgentsAsync();
			var workersTask = this.eventWorkerRepository.ReadAllWorkersAsync();
			await Task.WhenAll(agentsTask, workersTask);

            var agents = await agentsTask;
		    var workers = await workersTask;

			var orphanedAgentWorkers = workers
                .Where(w => w.Type == EventWorkerType.Agent && agents.All(a => a != w.Id)).ToList();

			var loggingTask = orphanedAgentWorkers
				.Select(w => this.logger.LogVerboseAsync($"Orphaned agent worker found: {w.Id} - {w.Name}"))
				.WhenAllStreamed();

			// Mark related orphaned events errored
			await orphanedAgentWorkers
				.Select(this.MarkOrphanedEventsErrored)
				.WhenAllStreamed();

			// Delete the orphaned workers
			await orphanedAgentWorkers
				.Select(w => this.eventWorkerRepository.DeleteAsync(w))
				.WhenAllStreamed();

			await loggingTask;
		}

		/// <inheritdoc />
		public async Task MarkOrphanedEventsErrored(EventWorker orphanedWorker)
		{
			// Read the event locks which each have an event id
			var orphanedEventLocks = await this.eventLockRepository.ReadByWorker(orphanedWorker);

			var loggingTask = orphanedEventLocks
				.Select(e => this.logger.LogVerboseAsync($"Orphaned events found from event locks: {e.EventId} from worker: {orphanedWorker.Id} - {orphanedWorker.Name}"))
				.WhenAllStreamed();

			// Get the event ids and read the events
			var orphanedEvents = await orphanedEventLocks
				.DistinctBy(l => l.EventId)
				.Select(l => this.eventRepository.ReadAsync(l.EventId))
				.WhenAllStreamed();

			// mark the event status errored and commit
			var updateTask = orphanedEvents
				.ForEach(e => e.Status = EventStatus.Error)
				.Select(e => this.eventRepository.UpdateAsync(e))
				.WhenAllStreamed();

			// Release all the locks for the worker
			var lockReleaseTask = this.eventLockRepository.Release(orphanedWorker);

			await Task.WhenAll(loggingTask, updateTask, lockReleaseTask);
		}

		/// <inheritdoc />
		public Task ResolveTimedOutEvents()
		{
			var timeout = DateTime.UtcNow.AddHours(-1);
			return this.eventRepository.UpdateTimedOutEventsAsync(timeout);
		}
	}
}
