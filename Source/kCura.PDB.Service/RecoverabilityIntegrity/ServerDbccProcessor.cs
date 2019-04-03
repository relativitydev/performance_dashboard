namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class ServerDbccProcessor : IServerDbccProcessor
	{
		private readonly IDbccProvider dbccProvider;
		private readonly IDatabaseRepository databaseRepository;
		private readonly IDatabaseGapsRepository databaseGapsRepository;
		private readonly ILogger logger;

		public ServerDbccProcessor(
			IDbccProvider dbccProvider,
			IDatabaseRepository databaseRepository,
			IDatabaseGapsRepository databaseGapsRepository,
			ILogger logger)
		{
			this.dbccProvider = dbccProvider;
			this.databaseRepository = databaseRepository;
			this.databaseGapsRepository = databaseGapsRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.RecoverabilityIntegrity);
		}

		public async Task ProcessDbccsForServer(Hour hour, Server server)
		{
			var databases = await this.databaseRepository.ReadByServerIdAsync(server.ServerId);

			await this.logger.LogVerboseAsync($"Processing dbccs for {databases.Count} on {server.ServerName} during {hour.HourTimeStamp}.");

			if (!databases.Any())
			{
				return;
			}

			var gaps =
				await databases
					.AsBatches(Defaults.RecoverabilityIntegrity.DatabaseBatchSize)
					.Select(batch => this.ProcessDbccForDatabaseBatches(server, batch))
					.WhenAllStreamed(1);

			var gapsToCreate = gaps
				.SelectMany(g => g)
				.ToList();

			await this.logger.LogVerboseAsync($"Creating {gapsToCreate.Count} dbcc gaps for {server.ServerName} during {hour.HourTimeStamp}.");

			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gapsToCreate);
		}

		internal async Task<IList<DbccGap>> ProcessDbccForDatabaseBatches(Server server, IList<Database> databases)
		{
			var hourDbccs = await this.dbccProvider.GetDbccsAsync(server, databases);

			return
				(await hourDbccs
					.GroupBy(d => d.DatabaseId)
					.Select(dg => this.CreateDbccGap(
						databases.FirstOrDefault(lb => lb.Id == dg.Key),
						dg.First()
					))
					.WhenAllStreamed())
					.Where(g => g != null)
					.ToList();
		}

		internal async Task<DbccGap> CreateDbccGap(Database database, Dbcc dbcc)
		{
			var lastDbcc = database.LastDbccDate;

			// If there isn't a last dbcc then we'll record the current dbcc as the last _if_ it's not the default dbcc date of '1900-01-01 00:00:00.000'
			if (!lastDbcc.HasValue && dbcc.End > new DateTime(1901, 1, 1))
			{
				database.LastDbccDate = dbcc.End;
				await this.databaseRepository.UpdateAsync(database);
			}
			else if (lastDbcc.HasValue && (dbcc.End - lastDbcc.Value).TotalMilliseconds > 10) // checking if the current doesn't equal the last with a little buffer
			{
				database.LastDbccDate = dbcc.End;
				await this.databaseRepository.UpdateAsync(database);
				return new DbccGap { DatabaseId = dbcc.DatabaseId, Start = lastDbcc.Value, End = dbcc.End };
			}

			return null;
		}
	}
}
