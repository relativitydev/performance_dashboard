namespace kCura.PDB.Data.Repositories
{
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class DataGridCacheRepository : IDataGridCacheRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public DataGridCacheRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}
		
		public async Task<bool> ReadUseDataGrid(int workspaceId, int hourId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstAsync<bool>(Resources.DataGridCache_ReadUseDataGrid, new { workspaceId, hourId });
			}
		}

		public async Task UpdateDataGridCache(int workspaceId, int earliestDataGridHourId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.DataGridCache_UpdateOrCreate, new { workspaceId, hourId = earliestDataGridHourId });
			}
		}

		public async Task Clear(int workspaceId)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.DataGridCache_Delete, new { workspaceId });
			}
		}
	}
}
