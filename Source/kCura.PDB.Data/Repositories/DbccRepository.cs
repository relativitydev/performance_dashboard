namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class DbccRepository : IDbccRepository
	{
		public DbccRepository(IConnectionFactory connectionFactory, IGeneralSqlRepository generalSqlRepository)
		{
			this.connectionFactory = connectionFactory;
			this.generalSqlRepository = generalSqlRepository;
		}

		private readonly IConnectionFactory connectionFactory;
		private readonly IGeneralSqlRepository generalSqlRepository;

		public async Task<IList<Dbcc>> GetDbccsAsync(Server server, IList<Database> databases)
		{
			var results = new List<Dbcc>();

			using (var conn = this.connectionFactory.GetTargetConnection(Names.Database.PdbResource, server.ServerName))
			{
				// Backups are stored in local time, not UTC, so we get the timezone offset and add that to our query
				var currentDbTimezoneOffset = await this.generalSqlRepository.GetSqlTimezoneOffset(conn);

				foreach (var database in databases)
				{
					var dbccResult = await conn.QueryFirstOrDefaultAsync<string>("eddsdbo.ReadDatabaseDbcc", new { databaseName = database.Name }, commandType: CommandType.StoredProcedure);
					DateTime end;
					if (!string.IsNullOrEmpty(dbccResult) && DateTime.TryParse(dbccResult, out end))
					{
						results.Add(new Dbcc { DatabaseId = database.Id, End = end });
					}
				}

				UpdateDbccTimesToUtc(results, currentDbTimezoneOffset);
			}

			return results;
		}

		public async Task RunDbcc()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync("DBCC CHECKDB");
			}
		}

		internal static void UpdateDbccTimesToUtc(IList<Dbcc> dbccs, int currentDbTimezoneOffsetInMinutes) =>
			dbccs.ForEach(dbcc => dbcc.End = dbcc.End.AddMinutes(-currentDbTimezoneOffsetInMinutes));
	}
}
