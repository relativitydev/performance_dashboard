namespace kCura.PDB.Data.Repositories.Testing
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class IntegrationTestNewServerRepository : IServerRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public IntegrationTestNewServerRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public int? GetWorkspaceServerId(int caseArtifactId)
		{
			throw new System.NotImplementedException();
		}

		public async Task<Server> ReadAsync(int serverId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Server>(Resources.Server_ReadByID, new { serverId });
			}
		}

		public async Task<IList<Server>> ReadAllActiveAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Server>(Resources.ServerTest_ReadAllActive)).ToList();
			}
		}

		public IList<Server> ReadAllActive()
		{
			throw new System.NotImplementedException();
		}

		public Task UpdateAsync(Server server)
		{
			throw new System.NotImplementedException();
		}

		public Task DeleteAsync(Server server)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<int>> ReadServerWorkspaceIdsAsync(int serverId)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<Server>> ReadServerPendingQosDeploymentAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task UpdateActiveServersPendingQosDeploymentAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<int?> ReadPrimaryStandaloneAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<bool> ReadWorkspaceExistsAsync(int workspaceId)
		{
			throw new System.NotImplementedException();
		}
	}
}
