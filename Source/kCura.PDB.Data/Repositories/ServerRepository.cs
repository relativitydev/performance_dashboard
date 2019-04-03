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

	public class ServerRepository : BaseDbRepository, IServerRepository
	{
		public ServerRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
		
		internal async Task<Server> CreateAsync(Server server)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Server>(Resources.Server_Create, server);
			}
		}

		public async Task<Server> ReadAsync(int serverId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Server>(Resources.Server_ReadByID, new { serverId });
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method.")]
		public async Task<IList<Server>> ReadAllActiveAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Server>(Resources.Server_ReadAllActive)).ToList();
			}
		}

		public IList<Server> ReadAllActive()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.Query<Server>(Resources.Server_ReadAllActive).ToList();
			}
		}

		public async Task UpdateAsync(Server server)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Server_Update, server);
			}
		}

		public async Task DeleteAsync(Server server)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Server_Delete, new { server.ServerId });
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method.")]
		public async Task<IList<int>> ReadServerWorkspaceIdsAsync(int serverId)
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return (await conn.QueryAsync<int>(Resources.Server_ReadWorkspaceIds, new { serverId })).ToList();
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method.")]
		public async Task<IList<Server>> ReadServerPendingQosDeploymentAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Server>(Resources.Server_ReadServerPendingQosDeployment, new { databaseServerTypeId = (int)ServerType.Database })).ToList();
			}
		}

		public async Task UpdateActiveServersPendingQosDeploymentAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Server_UpdateActiveServersPendingQosDeployment);
			}
		}

		public async Task<int?> ReadPrimaryStandaloneAsync()
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int?>(Resources.Server_ReadPrimaryStandalone);
			}
		}

		public async Task<bool> ReadWorkspaceExistsAsync(int workspaceId)
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<bool>(Resources.RelativityArtifact_Exists, new {artifactId = workspaceId});
			}
		}
	}
}
