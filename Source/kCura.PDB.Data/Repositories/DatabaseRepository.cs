namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Properties;

	public class DatabaseRepository : IDatabaseRepository
	{
		public DatabaseRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<Database> ReadAsync(int databaseId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Database>(Resources.Database_Read, new { Id = databaseId }).ConfigureAwait(false); ;
			}
		}

		public async Task<IList<Database>> ReadByServerIdAsync(int serverId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Database>(Resources.Database_ReadByServer, new { serverId }).ConfigureAwait(false)).ToList();
			}
		}

		public virtual async Task<IList<string>> GetByServerAsync(Server server)
		{
			using (var conn = connectionFactory.GetTargetConnection(Names.Database.Msdb, server.ServerName))
			{
				return (await conn.QueryAsync<string>(Resources.Database_GetByServer).ConfigureAwait(false)).ToList();
			}
		}

		public async Task<Database> CreateAsync(Database database)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var updateResult = await conn.ExecuteAsync(Resources.Database_UpdateByNameAndServer, database)
					.ConfigureAwait(false);

				if (updateResult <= 0)
				{
					await conn.ExecuteAsync(Resources.Database_Create, database).ConfigureAwait(false);
				}

				return await conn.QueryFirstOrDefaultAsync<Database>(Resources.Database_ReadByNameAndServer, database).ConfigureAwait(false);
			}
		}

		public async Task UpdateAsync(Database database)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Database_UpdateByNameAndServer, database).ConfigureAwait(false);
			}
		}

		public async Task<DateTime?> ReadMostOutOfDateActivityByServerAsync(Server server, GapActivityType activityType)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return activityType == GapActivityType.Dbcc
					? await conn.QueryFirstOrDefaultAsync<DateTime?>(Resources.Database_ReadMostOutOfDateDbccByServer, server).ConfigureAwait(false)
					: await conn.QueryFirstOrDefaultAsync<DateTime?>(Resources.Database_ReadMostOutOfDateBackupFullDiffByServer, server).ConfigureAwait(false);
			}
		}

		public async Task<IList<Database>> ReadOutOfDateDatabasesAsync(Server server, DateTime windowExceededDate, GapActivityType activityType)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var parameters = new { server.ServerId, windowExceededDate };
				return activityType == GapActivityType.Dbcc
					? await conn.QueryAsync<Database>(Resources.Database_ReadOutOfDateDatabasesByDbccs, parameters).ToListAsync().ConfigureAwait(false)
					: await conn.QueryAsync<Database>(Resources.Database_ReadOutOfDateDatabasesByBackups, parameters).ToListAsync().ConfigureAwait(false);
			}
		}

		public async Task<int> ReadCountByServerAsync(Server server)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>(Resources.Database_ReadCountByServer, new { server.ServerId }).ConfigureAwait(false);
			}
		}
	}
}
