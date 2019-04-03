namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;
	using Dapper;

	public class MaintenanceWindowRepository : IMaintenanceWindowRepository
	{
		public MaintenanceWindowRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<MaintenanceWindow> CreateAsync(MaintenanceWindow window)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<MaintenanceWindow>(Resources.MaintenanceWindow_Create, window);
			}
		}

		public async Task<MaintenanceWindow> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<MaintenanceWindow>(Resources.MaintenanceWindow_ReadByID, new { id });
			}
		}

		public async Task UpdateAsync(MaintenanceWindow window)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MaintenanceWindow_Update, window);
			}
		}

		public async Task DeleteAsync(MaintenanceWindow window)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MaintenanceWindow_Delete, window);
			}
		}

		public async Task<IEnumerable<MaintenanceWindow>> ReadSortedAsync(MaintenanceWindowDataTableQuery query)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryAsync<MaintenanceWindow>(Resources.MaintenanceWindow_ReadTable, query);
			}
		}

		public async Task<bool> HourIsInMaintenanceWindowAsync(Hour hour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<bool>(Resources.MaintenanceWindow_IsHourScheduled, hour);
			}
		}
	}
}