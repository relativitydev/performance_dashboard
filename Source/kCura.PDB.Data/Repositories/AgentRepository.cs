namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class AgentRepository : BaseDbRepository, IAgentRepository
	{
		public AgentRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<bool> ReadAgentWithTypeExists(Guid agentTypeGuid)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<bool>(Resources.Agent_CheckWithTypeExists, new { agentTypeGuid });
			}
		}

		public bool ReadAgentEnabled(int agentId)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.QueryFirstOrDefault<bool>(Resources.Agent_Enabled, new { agentId });
			}
		}

		public async Task<IList<int>> ReadAllEnabledAgentsAsync(/* IList<Guid> agentGuids = null */)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return (await conn.QueryAsync<int>(Resources.Agent_ReadAllEnabled, new { agentGuids = Guids.Agent.AllAgentGuids })).ToList();
			}
		}

		public async Task<IList<int>> ReadAllAgentsAsync(/* IList<Guid> agentGuids = null */)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return (await conn.QueryAsync<int>(Resources.Agent_ReadAll, new { agentGuids = Guids.Agent.AllAgentGuids })).ToList();
			}
		}

		public async Task<IList<int>> ReadAgentsAsync(IList<Guid> agentGuids)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return (await conn.QueryAsync<int>(Resources.Agent_ReadAll, new { agentGuids })).ToList();
			}
		}
	}
}
