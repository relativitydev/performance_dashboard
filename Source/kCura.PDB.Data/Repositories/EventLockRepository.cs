namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class EventLockRepository : IEventLockRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public EventLockRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		/// <inheritdoc />
		public async Task<EventLock> Claim(Event evnt, int workerId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				try
				{
					return await conn.QueryFirstAsync<EventLock>(Resources.EventLock_Claim,
								new { EventId = evnt.Id, evnt.SourceId, EventTypeId = evnt.SourceTypeId, workerId });
				}
				catch (SqlException exception)
				{
					// if we attempt to insert a record with the same sourceid and event type id (SourceTypeId)
					// then it will fail due to unique constraint meaning the lock can't be claimed.
					if (exception.Message.ComparisonContains("Violation of UNIQUE KEY constraint"))
					{
						return null;
					}
					else
					{
						throw;
					}
				}
			}
		}

		/// <inheritdoc />
		public async Task Release(EventLock eventLock)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.EventLock_Release, eventLock);
			}
		}

		/// <inheritdoc />
		public async Task Release(EventWorker eventWorker)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.EventLock_ReleaseByWorker, new { workerId = eventWorker.Id });
			}
		}

		/// <inheritdoc />
		public async Task<IList<EventLock>> ReadByWorker(EventWorker eventWorker)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<EventLock>(Resources.EventLock_ReadByWorker, new { workerId = eventWorker.Id })).ToList();
			}
		}
	}
}
