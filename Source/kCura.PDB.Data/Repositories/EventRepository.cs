namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Dapper;
	using Properties;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Transactions;
	using Core.Extensions;

	public class EventRepository : BaseDbRepository, IEventRepository
	{
		public EventRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<Event> CreateAsync(Event evnt)
		{
			evnt.Status = EventStatus.Pending;
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				try
				{
					return await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_Create, evnt);
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					// try again if it deadlocks
					if (ex.Message.Contains("deadlocked"))
					{
						return await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_Create, evnt);
					}
					else
					{
						throw;
					}
				}
			}
		}

		public async Task CreateAsync(IList<Event> events)
		{
			events.ForEach(evnt => evnt.Status = EventStatus.Pending);
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				try
				{
					await conn.ExecuteAsync(Resources.Event_CreateMany, events);
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					// try again if it deadlocks
					if (ex.Message.Contains("deadlocked"))
					{
						await conn.ExecuteAsync(Resources.Event_CreateMany, events);
					}
					else
					{
						throw;
					}
				}
			}
		}

		public async Task<Event> ReadAsync(long eventId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_ReadByID, new { id = eventId });
			}
		}

		public async Task<Event> ReadLastAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_ReadLast);
			}
		}

		public async Task<Event> ReadLastBySourceIdAndTypeAsync(EventSourceType type, int? sourceId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (sourceId.HasValue)
						? await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_ReadLastByEventTypeWithSourceId, new { eventType = type, sourceId })
						: await conn.QueryFirstOrDefaultAsync<Event>(Resources.Event_ReadLastByEventType, new { eventType = type });
			}
		}

		public async Task<int> ReadCountByStatusAsync(EventStatus status)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>(Resources.Event_ReadCountByStatus, new { status });
			}
		}

		public async Task<int> ReadCountByStatusAsync(IList<EventStatus> status)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>(Resources.Event_ReadCountByStatuses, new { status });
			}
		}

		/// <inheritdoc/>
		public async Task<bool> ReadAnyRemainingHourEventsAsync(int hourId, EventSourceType eventTypeToExclude, params EventStatus[] statuses)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>(Resources.Event_ReadCountByHourStatusTypes, new { hourId, eventTypeToExclude, statusIds = statuses.Select(s => (int)s).ToList() }) > 0;
			}
		}

		public async Task UpdateAsync(Event evnt)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				try
				{
					await conn.ExecuteAsync(Resources.Event_Update, evnt);
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					// try again if it deadlocks
					if (ex.Message.Contains("deadlocked"))
					{
						await conn.ExecuteAsync(Resources.Event_Update, evnt);
					}
					else
					{
						throw;
					}
				}
			}
		}

		public async Task UpdateTimedOutEventsAsync(DateTime timeout)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				try
				{
					await conn.ExecuteAsync(Resources.Event_UpdateTimedOutEvents, new { timeoutStatus = EventStatus.Error, inProgressStatus = EventStatus.InProgress, pendingHangfireStatus = EventStatus.PendingHangfire, timeout });
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					// try again if it deadlocks
					if (ex.Message.Contains("deadlocked"))
					{
						await conn.ExecuteAsync(Resources.Event_UpdateTimedOutEvents, new { timeoutStatus = EventStatus.Error, inProgressStatus = EventStatus.InProgress, pendingHangfireStatus = EventStatus.PendingHangfire, timeout });
					}
					else
					{
						throw;
					}
				}
			}
		}

		public async Task DeleteAsync(Event evnt)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Event_Delete, new { evnt.Id });
			}
		}

		public async Task DeleteAllByTypeAsync(EventSourceType eventType)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Event_DeleteAllByType, new { eventType });
			}
		}

		public async Task<EventSystemState> ReadEventSystemStateAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<EventSystemState>(Resources.Event_ReadEventSystemState);
			}
		}

		public async Task UpdateReadEventSystemStateAsync(EventSystemState state)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Event_UpdateEventSystemState, new { state });
			}
		}

		public async Task UpdateEventTypesAsync()
		{
			var eventTypes =
				Enum.GetValues(typeof(EventSourceType))
					.Cast<EventSourceType>()
					.Select(et => new { Id = (int)et, Name = et.ToString() })
					.ToList();
			using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var conn = connectionFactory.GetEddsPerformanceConnection())
				{
					await conn.ExecuteAsync(Resources.EventType_DeleteAll);
					await conn.ExecuteAsync(Resources.EventType_Create, eventTypes);
				}
				transaction.Complete();
			}
		}

		public async Task<bool> ExistsAsync(int? sourceId, int sourceTypeId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				if (sourceId.HasValue)
				{
					return await conn.QueryFirstOrDefaultAsync<int>(Resources.Event_ReadCountBySourceTypeAndId,
						new { sourceId, sourceTypeId }) > 0;
				}
				else
				{
					return await conn.QueryFirstOrDefaultAsync<int>(Resources.Event_ReadCountBySourceTypeAndNullId,
						new { sourceTypeId }) > 0;
				}
			}
		}

		/// <summary>
		/// Use for testing only, indexes not optimized
		/// </summary>
		/// <param name="hourId"></param>
		/// <param name="sourceType"></param>
		/// <param name="eventStatus"></param>
		/// <returns></returns>
		public async Task<bool> ExistsAsync(int hourId, EventSourceType sourceType, EventStatus eventStatus)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<bool>(
							Resources.Event_ExistsForHourTypeStatus,
							new { hourId, sourceTypeId = sourceType, statusId = (int)eventStatus });
			}
		}

		public void CreateEventLog(long eventId, int logId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.EventLog_Create, new { eventId, logId });
			}
		}

		public async Task CreateEventLogAsync(long eventId, int logId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.EventLog_Create, new { eventId, logId });
			}
		}

		public async Task CancelEventsAsync(IList<EventStatus> statusesToCancel, IList<EventSourceType> typesToExclude)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Event_CancelEvents, new { cancelStatus = EventStatus.Cancelled, statusesToCancel, typesToExclude });
			}
		}

		public async Task<IList<long>> ReadIdsByStatusAsync(EventStatus eventStatus, long count, IList<EventSourceType> excludeEventTypes)
		{
			// If there is nothing to exclude then use the other method
			if (!excludeEventTypes.Any())
			{
				return await this.ReadIdsByStatusAsync(eventStatus, count);
			}

			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<long>(
					Resources.Event_ReadIdsByStatusAndType, new { eventStatus, count, excludeEventTypes })).ToList();
			}
		}

		public async Task<IList<long>> ReadIdsByStatusAsync(EventStatus eventStatus, long count)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<long>(
							Resources.Event_ReadIdsByStatus,
							new { eventStatus, count })).ToList();
			}
		}

		public async Task<IList<Event>> UpdateStatusAsync(IList<long> eventIds, EventStatus updateStatus)
		{
			if (!eventIds.Any())
			{
				return new List<Event>();
			}

			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Event>(
							Resources.Event_UpdateStatusById,
							new { ids = eventIds, updateStatus })).ToList();
			}
		}

		public async Task<IList<EventSourceType>> ReadTypesByTypeAndStatusAsync(
			IList<EventSourceType> eventTypes,
			IList<EventStatus> eventStatuses)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<EventSourceType>(
					Resources.Event_ReadTypesByTypeAndStatus,
					new { eventTypes, eventStatuses }).ToListAsync();
			}
		}

		public async Task<IList<long>> ReadSingleByTypeAndStatusAsync(IList<EventSourceType> eventTypes, EventStatus status, IList<EventStatus> negativeStatuses)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<long>(Resources.Event_ReadSingleByTypeAndStatus, new { eventTypes, status, negativeStatuses })
					.ToListAsync();
			}
		}

		public async Task UpdateStatusAndRetriesAsync(IList<long> eventIds, EventStatus updateStatus)
		{
			if (!eventIds.Any())
			{
				return;
			}

			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(
							Resources.Event_UpdateStatusAndRetriesById,
							new { ids = eventIds, updateStatus });
			}
		}

	}
}
