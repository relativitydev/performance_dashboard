namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class DatabaseBackupProcessorTests
	{
		private DatabaseBackupProcessor databaseBackupProcessor;
		private Mock<IDatabaseRepository> databaseRepository;

		[SetUp]
		public void Setup()
		{
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.databaseBackupProcessor = new DatabaseBackupProcessor(this.databaseRepository.Object);
		}

		[Test]
		public void CreateGaps()
		{
			// Arrange
			var database = new Database { Id = 123 };
			var backups =
				new[] { 1, 2, 10, 20, 30 }
					.Select(x => new Backup { End = new DateTime(2018, 10, 23, 5, x, 0) })
					.ToList();

			var lastBackupBeforeHour = new Backup { End = new DateTime(2018, 10, 23, 4, 50, 0) };

			// Act
			var result = this.databaseBackupProcessor.CreateGaps<BackupAllGap>(database, backups, lastBackupBeforeHour);

			// Assert
			Assert.That(result.Count, Is.EqualTo(5));

			Assert.That(result[0].Duration, Is.EqualTo(11 * 60), "11 min duration");
			Assert.That(result[1].Duration, Is.EqualTo(1 * 60), "1 min duration");
			Assert.That(result[2].Duration, Is.EqualTo(8 * 60), "8 min duration");
			Assert.That(result[3].Duration, Is.EqualTo(10 * 60), "10 min duration");
			Assert.That(result[4].Duration, Is.EqualTo(10 * 60), "10 min duration");

			Assert.That(result[0].End, Is.EqualTo(new DateTime(2018, 10, 23, 5, 1, 0)));
			Assert.That(result[1].End, Is.EqualTo(new DateTime(2018, 10, 23, 5, 2, 0)));
			Assert.That(result[2].End, Is.EqualTo(new DateTime(2018, 10, 23, 5, 10, 0)));
			Assert.That(result[3].End, Is.EqualTo(new DateTime(2018, 10, 23, 5, 20, 0)));
			Assert.That(result[4].End, Is.EqualTo(new DateTime(2018, 10, 23, 5, 30, 0)));

			Assert.That(result[0].Start, Is.EqualTo(new DateTime(2018, 10, 23, 4, 50, 0)));
			Assert.That(result[1].Start, Is.EqualTo(new DateTime(2018, 10, 23, 5, 1, 0)));
			Assert.That(result[2].Start, Is.EqualTo(new DateTime(2018, 10, 23, 5, 2, 0)));
			Assert.That(result[3].Start, Is.EqualTo(new DateTime(2018, 10, 23, 5, 10, 0)));
			Assert.That(result[4].Start, Is.EqualTo(new DateTime(2018, 10, 23, 5, 20, 0)));

			Assert.That(result.All(r => r.ActivityType == GapActivityType.Backup), Is.True);
		}

		[Test]
		public async Task ServerBackupProcessor_UpdateLatestDatabaseBackups()
		{
			// Arrange
			var db7 = new Database
			{
				Id = 7,
				LastBackupFullDate = new DateTime(2018, 5, 19),
				LastBackupDiffDate = new DateTime(2018, 5, 18),
				LastBackupLogDate = new DateTime(2018, 5, 17)
			};

			var backups = new[]
			{
				new Backup { End = new DateTime(2018,5,20), Start = new DateTime(2018,5,19), Type = BackupType.Full},
				new Backup { End = new DateTime(2018,5,21), Start = new DateTime(2018,5,20), Type = BackupType.Full}, // most recent full
				new Backup { End = new DateTime(2018,5,21,5,0,0), Start = new DateTime(2018,5,20), Type = BackupType.Log}, // this log backup is NOT after the diffs and fulls so it's duration will NOT be added to log duration
				new Backup { End = new DateTime(2018,5,22), Start = new DateTime(2018,5,21), Type = BackupType.Differential},
				new Backup { End = new DateTime(2018,5,23), Start = new DateTime(2018,5,22), Type = BackupType.Differential}, // most recent Differential
				new Backup { End = new DateTime(2018,5,24), Start = new DateTime(2018,5,23), Type = BackupType.Log}, // this log backup is after the diffs and fulls so it's duration will be added to log duration
				new Backup { End = new DateTime(2018,5,25), Start = new DateTime(2018,5,24), Type = BackupType.Log}, // most recent log
			};

			this.databaseRepository.Setup(r => r.UpdateAsync(db7))
				.ReturnsAsyncDefault()
				.Callback<Database>(d => Console.WriteLine($"Full={d.LastBackupFullDate}: {d.LastBackupFullDuration}, Diff={d.LastBackupDiffDate}: {d.LastBackupDiffDuration}, Log={d.LastBackupLogDate}: {d.LogBackupsDuration}"));

			// Act
			await this.databaseBackupProcessor.UpdateLatestDatabaseBackups(db7, backups);

			// Assert
			this.databaseRepository.Verify(r => r.UpdateAsync(It.Is<Database>(db =>

				// Check the last backup dates were recorded correctly
				db.LastBackupFullDate == new DateTime(2018, 5, 21)
				&& db.LastBackupDiffDate == new DateTime(2018, 5, 23)
				&& db.LastBackupLogDate == new DateTime(2018, 5, 25)

				// Check that the durations were recorded correctly
				&& db.LastBackupFullDuration == (int)TimeSpan.FromDays(1).TotalMinutes
				&& db.LastBackupDiffDuration == (int)TimeSpan.FromDays(1).TotalMinutes
				&& db.LogBackupsDuration == (int)TimeSpan.FromDays(2).TotalMinutes)));
		}

		[Test]
		public async Task ServerBackupProcessor_UpdateLatestDatabaseBackups_NoChange()
		{
			// Arrange
			var db7 = new Database
			{
				Id = 7,
				LastBackupFullDate = new DateTime(2018, 5, 19),
				LastBackupDiffDate = new DateTime(2018, 5, 18),
				LastBackupLogDate = new DateTime(2018, 5, 17)
			};

			var backups = new[]
			{
				// All these backups are for April when the backups on the database above are in May
				new Backup { End = new DateTime(2018,4,20), Start = new DateTime(2018,4,19), Type = BackupType.Full},
				new Backup { End = new DateTime(2018,4,23), Start = new DateTime(2018,4,22), Type = BackupType.Differential},
				new Backup { End = new DateTime(2018,4,25), Start = new DateTime(2018,4,24), Type = BackupType.Log},
			};

			this.databaseRepository.Setup(r => r.UpdateAsync(db7))
				.ReturnsAsyncDefault();

			// Act
			await this.databaseBackupProcessor.UpdateLatestDatabaseBackups(db7, backups);

			// Assert
			this.databaseRepository.Verify(r => r.UpdateAsync(It.IsAny<Database>()), Times.Never);
		}
	}
}
