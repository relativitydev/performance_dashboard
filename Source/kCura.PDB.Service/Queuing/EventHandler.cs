namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class EventHandler : IEventHandler
	{

		private readonly IEventTaskFactory eventTaskFactory;
		private readonly IEventRepository eventRepository;
		private readonly IEventLockRepository eventLockRepository;
		private readonly IEventWorkerService eventWorkerService;
		private readonly IEventChildKernelFactory eventChildKernelFactory;
		private readonly ILogger logger;

		public EventHandler(
			IEventTaskFactory eventTaskFactory,
			IEventRepository eventRepository,
			IEventLockRepository eventLockRepository,
			IEventWorkerService eventWorkerService,
			IEventChildKernelFactory eventChildKernelFactory,
			ILogger logger)
		{
			this.eventTaskFactory = eventTaskFactory;
			this.eventRepository = eventRepository;
			this.eventLockRepository = eventLockRepository;
			this.eventWorkerService = eventWorkerService;
			this.eventChildKernelFactory = eventChildKernelFactory;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Event);
		}

		public async Task HandleEvent(long eventId, EventSourceType eventType)
		{
			EventLock eventLock = null;
			try
			{
				// Grab the event
				var evnt = await this.eventRepository.ReadAsync(eventId);

				// if the event is not pending hangire then we wont process it. This can happen because of event migration or orphaned events.
				if (evnt.Status != EventStatus.PendingHangfire)
				{
					this.logger.LogError($"Cannot process event {eventId} with status: {evnt.Status}");

					// Mark the event errored (unless it's already errored, cancelled, or expired)
					if (evnt.Status != EventStatus.Cancelled
						&& evnt.Status != EventStatus.Error
						&& evnt.Status != EventStatus.Expired)
					{
						await this.MarkEventErrored(eventId);
					}

					return;
				}

				// Add the event to the kernel context
				using (var eventKernel = this.eventChildKernelFactory.CreateChildKernel(evnt))
				{
					// Try to put a lock on it
					var currentWorker = await this.eventWorkerService.GetCurrentWorker();
					ThrowOn.IsNull(currentWorker, "Event worker");
					eventLock = await this.eventLockRepository.Claim(evnt, currentWorker.Id);
					if (eventLock != null)
					{
						// If successful...
						// Update to In Progress (from Hangfire Pending) and save
						evnt.Status = EventStatus.InProgress;
						await this.eventRepository.UpdateAsync(evnt);

						// Grab the event task
						var eventTask = this.eventTaskFactory.GetEventTask(eventKernel.Kernel, eventType);

						// Process the event
						var eventResult = await eventTask.ProcessEvent(evnt);

						// Release the lock
						await this.eventLockRepository.Release(eventLock);

						await eventTask.MarkEventResultAsync(eventResult, evnt);

						// Create the next events to call after this
						await eventTask.CreateNextEvents(eventResult, evnt);
					}
					else
					{
						// If not successful, mark event as a duplicate and not execute
						evnt.Status = EventStatus.Duplicate;
						await this.eventRepository.UpdateAsync(evnt);
					}
				}

			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to run handleEvent: {eventId}", ex);

				// Attempt to mark the event errored
				await this.MarkEventErrored(eventId);

				// If we have a lock that we made, but failed after event execution
				if (eventLock != null && eventLock.EventId == eventId)
				{
					// Attempt to release the lock
					await this.eventLockRepository.Release(eventLock);
				}
			}
		}

		private async Task MarkEventErrored(long eventId)
		{
			try
			{
				// Re-query the event since updates may have been made since the error
				var evnt = await this.eventRepository.ReadAsync(eventId);
				evnt.Status = EventStatus.Error;
				await this.eventRepository.UpdateAsync(evnt);
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to mark event errored during handleEvent: {eventId}", ex);
			}
		}
	}
}
