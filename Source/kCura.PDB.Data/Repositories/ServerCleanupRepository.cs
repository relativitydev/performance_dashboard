namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class ServerCleanupRepository : IServerCleanupRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ServerCleanupRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<ServerCleanup> ReadAsync(int serverCleanupId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<ServerCleanup>(Resources.ServerCleanup_Read, new { serverCleanupId });
			}
		}

		public async Task<ServerCleanup> CreateAsync(ServerCleanup serverCleanup)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<ServerCleanup>(Resources.ServerCleanup_Create, serverCleanup);
			}
		}

		public async Task UpdateAsync(ServerCleanup serverCleanup)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(Resources.ServerCleanup_Update, serverCleanup);
			}
		}
	}
}
