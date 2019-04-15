namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class DatabaseBackupProcessor : IDatabaseBackupProcessor
	{
		private readonly IDatabaseRepository databaseRepository;
		public DatabaseBackupProcessor(IDatabaseRepository databaseRepository)
		{
			this.databaseRepository = databaseRepository;
		}

		public IList<TGap> CreateGaps<TGap>(Database database, IList<Backup> backups, Backup lastBackupBeforeHour)
			where TGap : Gap, new()
		{
			var backupGaps = new List<TGap>();

			if (backups.Count <= 0)
			{
				return backupGaps;
			}

			var backupsQueue = new Queue<Backup>(backups.OrderBy(b => b.End));

			// if there was a previous backup then create the gap between that and the first backup within the hour
			if (lastBackupBeforeHour != null)
			{
				backupGaps.Add(new TGap
				{
					//ActivityType = GapActivityType.Backup,
					DatabaseId = database.Id,
					Duration = (int)(backupsQueue.Peek().End - lastBackupBeforeHour.End).TotalSeconds,
					Start = lastBackupBeforeHour.End,
					End = backupsQueue.Peek().End
				});
			}

			// Pair up the backups with the next backup to create the gaps. The last backup will not have a next backup
			while (backupsQueue.Any())
			{
				var backup = backupsQueue.Dequeue();
				if (backupsQueue.Any())
				{
					var nextBackup = backupsQueue.Peek();
					backupGaps.Add(new TGap
					{
						DatabaseId = database.Id,
						Start = backup.End,
						End = nextBackup?.End
					});
				}
			}

			return backupGaps;
		}

		public async Task UpdateLatestDatabaseBackups(Database database, IList<Backup> backups)
		{
			// might want to consider re-reading the db if there is concern that it would be updated else where asynchronously and we'd be overwriting those values
			var latestFull = backups.OrderByDescending(b => b.End).FirstOrDefault(b => b.Type == BackupType.Full);
			var latestDiff = backups.OrderByDescending(b => b.End).FirstOrDefault(b => b.Type == BackupType.Differential);
			var latestLog = backups.OrderByDescending(b => b.End).FirstOrDefault(b => b.Type == BackupType.Log);

			bool dirty = false;
			if (latestFull != null && (latestFull.End > database.LastBackupFullDate || !database.LastBackupFullDate.HasValue))
			{
				database.LastBackupFullDate = latestFull.End;
				database.LastBackupFullDuration = latestFull.Duration;
				database.LogBackupsDuration = 0;
				dirty = true;
			}

			if (latestDiff != null && (latestDiff.End > database.LastBackupDiffDate || !database.LastBackupDiffDate.HasValue))
			{
				database.LastBackupDiffDate = latestDiff.End;
				database.LastBackupDiffDuration = latestDiff.Duration;
				database.LogBackupsDuration = 0;
				dirty = true;
			}

			if (latestLog != null && (latestLog.End > database.LastBackupLogDate || !database.LastBackupDiffDate.HasValue))
			{
				var logsSinceFullAndDiffDuration = backups
					.OrderByDescending(b => b.End)
					.Where(b => b.Type == BackupType.Log
								&& (latestFull == null || b.End > latestFull.End)
								&& (latestDiff == null || b.End > latestDiff.End)
								&& (!database.LastBackupLogDate.HasValue || b.End > database.LastBackupLogDate.Value))
					.Sum(b => b.Duration);

				database.LastBackupLogDate = latestLog.End;
				database.LogBackupsDuration = database.LogBackupsDuration + logsSinceFullAndDiffDuration;
				dirty = true;
			}

			if (dirty)
			{
				await this.databaseRepository.UpdateAsync(database);
			}
		}
	}
}
