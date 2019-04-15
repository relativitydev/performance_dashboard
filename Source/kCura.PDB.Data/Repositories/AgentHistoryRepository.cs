namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class AgentHistoryRepository : IAgentHistoryRepository
	{
		public AgentHistoryRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<AgentHistory> CreateAsync(AgentHistory history)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<AgentHistory>(Resources.AgentHistory_Create, history);
			}
		}

		public async Task<AgentHistory> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<AgentHistory>(Resources.AgentHistory_ReadByID, new { id });
			}
		}

		public async Task<AgentHistory> ReadEarliestAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<AgentHistory>(Resources.AgentHistory_ReadEarliest);
			}
		}

		public async Task<IList<AgentHistory>> ReadByHourAsync(Hour hour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<AgentHistory>(Resources.AgentHistory_ReadByHour, new { hourId = hour.Id })).ToList();
			}
		}

		public async Task UpdateAsync(AgentHistory history)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.AgentHistory_Update, history);
			}
		}

		public async Task DeleteAsync(AgentHistory history)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.AgentHistory_Delete, history);
			}
		}
	}
}
