namespace kCura.PDB.Data.Repositories
{
	using System;
	using Dapper;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using Properties;
	using System.Data.SqlClient;
	using kCura.PDB.Core.Extensions;

	public class BackupRepository : IBackupRepository
	{
		public BackupRepository(IConnectionFactory connectionFactory, IGeneralSqlRepository generalSqlRepository)
		{
			this.connectionFactory = connectionFactory;
			this.generalSqlRepository = generalSqlRepository;
		}

		private readonly IGeneralSqlRepository generalSqlRepository;
		private readonly IConnectionFactory connectionFactory;

		public async Task<IList<Backup>> GetBackupsAsync(Hour hour, Server server, IList<Database> databases)
		{
			var databaseNames = databases.Select(d => d.Name).ToList();

			using (var conn = this.connectionFactory.GetTargetConnection(Names.Database.Msdb, server.ServerName))
			{
				// Backups are stored in local time, not UTC, so we get the timezone offset and add that to our query
				var currentDbTimezoneOffset = await this.generalSqlRepository.GetSqlTimezoneOffset(conn);

				var backups = (await conn.QueryAsync<Backup>(Resources.Backup_ReadBackups,
					new
					{
						hourStartDate = hour.HourTimeStamp.AddMinutes(currentDbTimezoneOffset),
						hourEndDate = hour.HourTimeStamp.AddMinutes(currentDbTimezoneOffset).AddHours(1),
						databaseNames
					}))
					.Where(b => Defaults.RecoverabilityIntegrity.AllSupportedBackupTypes.Contains(b.Type))
					.ToList();

				UpdateBackupTimesToUtc(backups, currentDbTimezoneOffset);

				return backups;
			}
		}

		public async Task<IList<Backup>> GetLastBackupsBeforeHourAsync(Hour hour, Server server, IList<Database> databases, IList<BackupType> backupTypes)
		{
			var databaseNames = databases.Select(d => d.Name).ToList();
			var types = backupTypes.Select(t => (char) t);

			using (var conn = this.connectionFactory.GetTargetConnection(Names.Database.Msdb, server.ServerName))
			{
				// Backups are stored in local time, not UTC, so we get the timezone offset and add that to our query
				var currentDbTimezoneOffset = await this.generalSqlRepository.GetSqlTimezoneOffset(conn);

				var backups = (await conn.QueryAsync<Backup>(Resources.Backup_ReadBackupsBeforeHour,
					new
					{
						hourEndDate = hour.HourTimeStamp.AddMinutes(currentDbTimezoneOffset),
						databaseNames,
						backupTypes = types
					})).ToList();

				UpdateBackupTimesToUtc(backups, currentDbTimezoneOffset);

				return backups;
			}
		}

		public async Task RunBackupAsync(BackupType backupType, string backupPath)
		{
			if (backupType == BackupType.Log)
			{
				throw new ArgumentException($"BackupType.Log is currently not supported");
			}

			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var databaseName = new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
				var queryText = backupType == BackupType.Full
					? $"BACKUP Database [{databaseName}] TO DISK = \'{backupPath}\'" // full backup
					: $"BACKUP Database [{databaseName}] TO DISK = \'{backupPath}\' with DIFFERENTIAL"; // diff backup

				await connection.ExecuteAsync(queryText);

				/*
				Running log backup fails. not sure whre to backup to
				connection.Execute($"ALTER DATABASE [{databaseName}] SET RECOVERY FULL;"); // set recovery mode to full so we can do log backup
				connection.Execute($"BACKUP LOG [{databaseName}] TO [EDDSPerformance_log]"); // log backup
				connection.Execute($"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;"); // set recovery mode to full so we can do log backup
				*/
			}
		}

		internal static void UpdateBackupTimesToUtc(IList<Backup> backups, int currentDbTimezoneOffsetInMinutes) =>
			backups.ForEach(backup =>
			{
				backup.Start = backup.Start.AddMinutes(-currentDbTimezoneOffsetInMinutes);
				backup.End = backup.End.AddMinutes(-currentDbTimezoneOffsetInMinutes);
			});
	}
}
