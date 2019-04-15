namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface IEventRepository : IDbRepository
	{
		Task<Event> CreateAsync(Event evnt);

		Task CreateAsync(IList<Event> events);

		Task<Event> ReadAsync(long eventId);

		/// <summary>
		/// Read all events with given status and sourceType (capped at numToEnqueue limit)
		/// </summary>
		/// <param name="eventStatus">Filters on events with this status</param>
		/// <param name="count">The max count of events to return</param>
		/// <param name="excludeEventTypes">Event types that are excluded from the results</param>
		/// <returns>Task of event ids</returns>
		Task<IList<long>> ReadIdsByStatusAsync(
			EventStatus eventStatus,
			long count,
			IList<EventSourceType> excludeEventTypes);

		/// <summary>
		/// Read all events with given status (capped at numToEnqueue limit)
		/// </summary>
		/// <param name="eventStatus">Filters on events with this status</param>
		/// <param name="count">The max count of events to return</param>
		/// <returns>Task of event ids</returns>
		Task<IList<long>> ReadIdsByStatusAsync(EventStatus eventStatus, long count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventIds"></param>
		/// <param name="updateStatus"></param>
		/// <returns></returns>
		Task<IList<Event>> UpdateStatusAsync(IList<long> eventIds, EventStatus updateStatus);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventIds"></param>
		/// <param name="updateStatus"></param>
		/// <returns>Task</returns>
		Task UpdateStatusAndRetriesAsync(IList<long> eventIds, EventStatus updateStatus);

		Task UpdateAsync(Event evnt);

		Task DeleteAsync(Event evnt);

		Task UpdateTimedOutEventsAsync(DateTime timeout);

		Task<EventSystemState> ReadEventSystemStateAsync();

		Task UpdateReadEventSystemStateAsync(EventSystemState state);

		Task<Event> ReadLastAsync();

		/// <summary>
		/// Reads the last event by event source type
		/// </summary>
		/// <param name="type">the type to filter on</param>
		/// <param name="sourceId">the source id to filter on</param>
		/// <returns>Task with the found event or null if none</returns>
		Task<Event> ReadLastBySourceIdAndTypeAsync(EventSourceType type, int? sourceId);

		/// <summary>
		/// Queries if there are any remaining hour events that are not complete, excluding the event type from eventTypeToExclude.
		/// </summary>
		/// <param name="hourId">the hour</param>
		/// <param name="eventTypeToExclude">will query for all event types BUT this event type</param>
		/// <param name="statuses">The status to query events by</param>
		/// <returns>True if there are any remaining hour events that are not complete, excluding the event type from eventTypeToExclude</returns>
		Task<bool> ReadAnyRemainingHourEventsAsync(int hourId, EventSourceType eventTypeToExclude, params EventStatus[] statuses);

		Task<int> ReadCountByStatusAsync(EventStatus status);

		Task<int> ReadCountByStatusAsync(IList<EventStatus> status);

		Task UpdateEventTypesAsync();

		Task CreateEventLogAsync(long eventId, int logId);

		Task<bool> ExistsAsync(int? sourceId, int sourceTypeId);

	    Task<bool> ExistsAsync(int hourId, EventSourceType sourceType, EventStatus eventStatus);

		void CreateEventLog(long eventId, int logId);

		Task CancelEventsAsync(IList<EventStatus> statusesToCancel, IList<EventSourceType> typesToExclude);

		/// <summary>
		/// Reads a distinct list of event types that match the event types and event source.
		/// A use case of this would be to query the event types that currently have active events.
		/// </summary>
		/// <param name="eventTypes">The event types to filter on</param>
		/// <param name="eventStatuses">the event statuses to filter on</param>
		/// <returns>Task of list of event types matching the types and statuses</returns>
		Task<IList<EventSourceType>> ReadTypesByTypeAndStatusAsync(
			IList<EventSourceType> eventTypes,
			IList<EventStatus> eventStatuses);

		/// <summary>
		/// Reads a single event for each type that is filtered on the status.
		/// </summary>
		/// <param name="eventTypes">the event types to read by</param>
		/// <param name="status">the event status to read by</param>
		/// <param name="negativeStatuses">will not return any events for a type if there are any events in a negative status</param>
		/// <returns>Task of event ids</returns>
		Task<IList<long>> ReadSingleByTypeAndStatusAsync(IList<EventSourceType> eventTypes, EventStatus status, IList<EventStatus> negativeStatuses);
	}
}
