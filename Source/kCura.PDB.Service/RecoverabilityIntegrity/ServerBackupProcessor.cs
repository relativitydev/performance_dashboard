namespace kCura.PDB.Service.RecoverabilityIntegrity
{
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
	using RiDefaults = Core.Constants.Defaults.RecoverabilityIntegrity;

	public class ServerBackupProcessor : IServerBackupProcessor
	{
		private readonly IBackupProvider backupProvider;
		private readonly IDatabaseRepository databaseRepository;
		private readonly IDatabaseGapsRepository databaseGapsRepository;
		private readonly IDatabaseBackupProcessor databaseBackupProcessor;
		private readonly ILogger logger;

		public ServerBackupProcessor(
			IBackupProvider backupProvider,
			IDatabaseRepository databaseRepository,
			IDatabaseGapsRepository databaseGapsRepository,
			IDatabaseBackupProcessor databaseBackupProcessor,
			ILogger logger)
		{
			this.backupProvider = backupProvider;
			this.databaseRepository = databaseRepository;
			this.databaseBackupProcessor = databaseBackupProcessor;
			this.databaseGapsRepository = databaseGapsRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.RecoverabilityIntegrity);
		}

		public async Task ProcessBackupsForServer(Hour hour, Server server)
		{
			var databases = await this.databaseRepository.ReadByServerIdAsync(server.ServerId);

			await this.logger.LogVerboseAsync($"Processing backups for {databases.Count} on {server.ServerName} during {hour.HourTimeStamp}.");

			if (!databases.Any())
			{
				return;
			}

			var gaps =
				await databases
				.AsBatches(Defaults.RecoverabilityIntegrity.DatabaseBatchSize)
				.Select(batch => this.ProcessBackupsForDatabaseBatches(hour, server, batch))
				.WhenAllStreamed(1);

			var gapsToCreate = gaps
				.SelectMany(g => g)
				.ToList();

			await this.logger.LogVerboseAsync($"Creating {gapsToCreate.Count} backup gaps for {server.ServerName} during {hour.HourTimeStamp}.");

			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gapsToCreate);
		}

		internal async Task<IList<Gap>> ProcessBackupsForDatabaseBatches(Hour hour, Server server, IList<Database> databases)
		{
			var hourBackups = await this.backupProvider.GetBackupsAsync(hour, server, databases);
			await this.logger.LogVerboseAsync($"Found {hourBackups.Count} backups for {server.ServerName} during {hour.HourTimeStamp}.");

			if (!hourBackups.Any())
			{
				return new List<Gap>();
			}

			var nameToDatabase = databases.ToDictionary(d => d.Name.ToLower());

			// Filter out any databases that don't have backups in the current hour to speed up finding the most recent backups before the current hour.
			var databasesNamesWithBackups = hourBackups.Select(b => b.DatabaseName.ToLower()).Distinct().ToList();
			var databasesWithBackups = databases.Where(db => databasesNamesWithBackups.Contains(db.Name.ToLower())).ToList();

			// Get the most recent backups, before the current hour, with all supported types (D, I, L).
			var mostRecentDatabaseBackups =
				await this.backupProvider.GetLastBackupsBeforeHourAsync(hour, server, databasesWithBackups, RiDefaults.AllSupportedBackupTypes);

			// Get the most recent backups, before the current hour, without log backups (D, I)
			var mostRecentNonLogDatabaseBackups =
				await this.backupProvider.GetLastBackupsBeforeHourAsync(hour, server, databasesWithBackups, RiDefaults.FullAndDiffBackupTypes);

			return await hourBackups
				.GroupBy(b => b.DatabaseName)
				.Select(bg => this.HandleDatabaseBackups(
					database: nameToDatabase[bg.Key.ToLower()],
					backups: bg.ToList(),
					mostRecentDatabaseBackup: mostRecentDatabaseBackups.FirstOrDefault(lb => lb.DatabaseName.ToLower() == bg.Key.ToLower()),
					mostRecentNonLogDatabaseBackup: mostRecentNonLogDatabaseBackups.FirstOrDefault(lb => lb.DatabaseName.ToLower() == bg.Key.ToLower())
				))
				.WhenAllStreamed(2)
				.SelectManyAsync(bg => bg)
				.ToListAsync();
		}

		internal async Task<IList<Gap>> HandleDatabaseBackups(Database database, IList<Backup> backups, Backup mostRecentDatabaseBackup, Backup mostRecentNonLogDatabaseBackup)
		{
			// Update the latest database backups.
			// Note: we only pass the backups for the current hour to be updated since they would be after any "most recent" backup and
			// if there weren't any backups in the hour then there are no gaps for the hour
			await this.databaseBackupProcessor.UpdateLatestDatabaseBackups(database, backups);

			// Get gaps using backups with all supported types (D, I, L)
			var backupsGapsAll = this.databaseBackupProcessor.CreateGaps<BackupAllGap>(database, backups, mostRecentDatabaseBackup);

			// Get gaps using backups without log backups (D, I)
			var fullAndDiffBackups = backups.Where(b => b.Type == BackupType.Full || b.Type == BackupType.Differential).ToList();
			var backupGapsFullAndDiff = this.databaseBackupProcessor.CreateGaps<BackupFullDiffGap>(database, fullAndDiffBackups, mostRecentNonLogDatabaseBackup);

			// return all the gaps
			return backupsGapsAll.Cast<Gap>()
				.Concat(
					backupGapsFullAndDiff.Where(gap => gap.Duration >= RiDefaults.WindowInDays)) // filter out full/diff gaps that are not exceeding the window
				.ToList();
		}
	}
}
