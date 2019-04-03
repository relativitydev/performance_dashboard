namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class EventWorkerRepository : IEventWorkerRepository
	{
		public EventWorkerRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<EventWorker> CreateAsync(EventWorker eventWorker)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var updateRecords = await conn.ExecuteAsync(Resources.EventWorker_Update, eventWorker);

				if (updateRecords == 0)
				{
					await conn.ExecuteAsync(Resources.EventWorker_Create, eventWorker);
				}

				return await conn.QueryFirstOrDefaultAsync<EventWorker>(Resources.EventWorker_Read, new{ eventWorker.Id });
			}
		}

		public async Task<IList<EventWorker>> ReadAllWorkersAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<EventWorker>(Resources.EventWorker_ReadAll)).ToList();
			}
		}

		public async Task DeleteAsync(EventWorker eventWorker)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.EventWorker_Delete, new { eventWorker.Id });
			}
		}

		public async Task<EventWorker> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<EventWorker>(Resources.EventWorker_Read, new { id });
			}
		}
	}
}
