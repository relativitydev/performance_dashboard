namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;

	public class CleanupTablesRepository : ICleanupTablesRepository
	{
		private readonly IConnectionFactory connectionFactory;
		private const int CleanupQueryConnectionTimeout = 10 * 60;

		public CleanupTablesRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		/// <inheritdoc />
		public async Task CleanupPerformanceTable(string tableScope, string dateTimeColumn, DateTime threshold, int batchSize, bool maxdopLimit = true)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(
					Resources.CleanupTables_DateTimeDynamicDelete,
					new {tableScope, dateTimeColumn, threshold, batchSize, maxdopLimit},
					commandTimeout: CleanupQueryConnectionTimeout);
			}
		}

		/// <inheritdoc />
		public async Task CleanupQoSGlassRunLog(DateTime threshold, int batchSize, bool maxdopLimit = true)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(
					Resources.CleanupTables_QosGlassRunLog,
					new { threshold, batchSize, maxdopLimit },
					commandTimeout: CleanupQueryConnectionTimeout);
			}
		}

		/// <inheritdoc />
		public async Task CleanupQosTable(string serverName, string tableScope, long threshold, int batchSize, bool maxdopLimit = true)
		{
			using (var connection = this.connectionFactory.GetEddsQosConnection(serverName))
			{
				await connection.ExecuteAsync(
					Resources.CleanupTables_QosHourIdDynamicDelete,
					new {tableScope, threshold, batchSize, maxdopLimit},
					commandTimeout: CleanupQueryConnectionTimeout);
			}
		}

		public async Task CleanupQosTable(
			string serverName, 
			string tableScope,
			string dateTimeColumn,
			DateTime threshold,
			int batchSize,
			bool maxdopLimit = true)
		{
			using (var connection = this.connectionFactory.GetEddsQosConnection(serverName))
			{
				await connection.ExecuteAsync(
					Resources.CleanupTables_DateTimeDynamicDelete,
					new { tableScope, dateTimeColumn, threshold, batchSize, maxdopLimit },
					commandTimeout: CleanupQueryConnectionTimeout);
			}
		}

		/// <inheritdoc />
		public async Task CleanupDecommissionedServers(DateTime threshold, int batchSize, bool maxdopLimit = true)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(
					Resources.CleanupTables_CasesToAuditDecommission,
					new { threshold, batchSize, maxdopLimit },
					commandTimeout: CleanupQueryConnectionTimeout);
			}
		}
	}
}
