namespace kCura.PDB.Service.Tests.RecoverabilityIntegrity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DataProviders;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common;
	using kCura.PDB.Tests.Common.Extensions;
	using Moq;
	using NUnit.Framework;
	using RiDefaults = Core.Constants.Defaults.RecoverabilityIntegrity;

	[TestFixture]
	[Category("Unit")]
	public class ServerBackupProcessorTests
	{
		private ServerBackupProcessor serverBackupProcessor;
		private Mock<IBackupProvider> backupProvider;
		private Mock<IDatabaseRepository> databaseRepository;
		private Mock<IDatabaseGapsRepository> databaseGapsRepository;
		private Mock<IDatabaseBackupProcessor> databaseBackupProcessor;
		private ILogger logger;

		[SetUp]
		public void Setup()
		{
			this.backupProvider = new Mock<IBackupProvider>();
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.databaseBackupProcessor = new Mock<IDatabaseBackupProcessor>();
			this.databaseGapsRepository = new Mock<IDatabaseGapsRepository>();
			this.logger = TestUtilities.GetMockLogger().Object;
			this.serverBackupProcessor = new ServerBackupProcessor(
				this.backupProvider.Object,
				this.databaseRepository.Object,
				this.databaseGapsRepository.Object,
				this.databaseBackupProcessor.Object,
				this.logger);
		}

		[Test]
		public async Task ServerBackupProcessor_ProcessBackupsForServer()
		{
			// Arrange
			var hour = new Hour();
			var server = new Server { ServerId = 123 };


			// Create 3x the batch size so we can test that batching works
			var databases = Enumerable
				.Range(0, 3 * Defaults.RecoverabilityIntegrity.DatabaseBatchSize)
				.Select(n => new Database { Name = $"EDDS{n}", Id = n })
				.ToList();

			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(databases);

			// For each database batch return a backup for each database
			this.backupProvider.Setup(bp => bp.GetBackupsAsync(hour, server, It.IsAny<IList<Database>>()))
				.Returns<Hour, Server, IList<Database>>((h, s, dbs) =>
					Task.FromResult<IList<Backup>>(dbs.Select(db => new Backup { DatabaseName = db.Name, Type = BackupType.Full }).ToList()));

			// For each database batch return a last backup  for each database
			this.backupProvider.Setup(bp => bp.GetLastBackupsBeforeHourAsync(hour, server, It.IsAny<IList<Database>>(), RiDefaults.AllSupportedBackupTypes))
				.Returns<Hour, Server, IList<Database>, IList<BackupType>>((h, s, dbs, bts) =>
					Task.FromResult<IList<Backup>>(dbs.Select(db => new Backup { DatabaseName = db.Name }).ToList()));

			// For each database batch return a last full/diff backup  for each database
			this.backupProvider.Setup(bp => bp.GetLastBackupsBeforeHourAsync(hour, server, It.IsAny<IList<Database>>(), RiDefaults.FullAndDiffBackupTypes))
				.Returns<Hour, Server, IList<Database>, IList<BackupType>>((h, s, dbs, bts) =>
					Task.FromResult<IList<Backup>>(dbs.Select(db => new Backup { DatabaseName = db.Name, Type = BackupType.Full }).ToList()));

			// For each database return 2 backup gaps
			this.databaseBackupProcessor.Setup(dbgp =>
					dbgp.CreateGaps<BackupAllGap>(It.IsAny<Database>(), It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns<Database, IList<Backup>, Backup>((db, backups, lastBackup) =>
					new[] { new BackupAllGap { DatabaseId = db.Id }, new BackupAllGap { DatabaseId = db.Id } });

			// For each database return 2 backup full and diff gaps
			this.databaseBackupProcessor.Setup(dbgp =>
					dbgp.CreateGaps<BackupFullDiffGap>(It.IsAny<Database>(), It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns<Database, IList<Backup>, Backup>((db, backups, lastBackup) => new[]
				{
					new BackupFullDiffGap { DatabaseId = db.Id, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow },
					new BackupFullDiffGap { DatabaseId = db.Id, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow }
				});

			this.databaseGapsRepository.Setup(r => r.CreateDatabaseGapsAsync(It.IsAny<IList<BackupAllGap>>())).ReturnsAsyncDefault();

			// Act
			await this.serverBackupProcessor.ProcessBackupsForServer(hour, server);

			// Assert
			this.databaseGapsRepository.Verify(r => r.CreateDatabaseGapsAsync(It.Is<IList<Gap>>(list =>
				list.Count == 4 * 3 * RiDefaults.DatabaseBatchSize))); // 4 backup gaps (2 and 2 returned by databaseBackupProcessor.creategaps each call) * number of databases
		}

		[Test]
		public async Task ServerBackupProcessor_ProcessBackupsForDatabases()
		{
			// Arrange
			var hour = new Hour();
			var server = new Server();

			var db7 = new Database { Name = $"EDDS7", Id = 7 };
			var db8 = new Database { Name = $"EDDS8", Id = 8 }; // 8 mocks a db that has no backups
			var db9 = new Database { Name = $"EDDS9", Id = 9 };

			var databases = new[] { db7, db8, db9 };

			this.backupProvider.Setup(bp => bp.GetBackupsAsync(hour, server, databases))
				.ReturnsAsync(new[]
				{
					new Backup { DatabaseName = "EDDS7", Type = BackupType.Full },
					new Backup { DatabaseName = "EDDS7", Type = BackupType.Log },
					new Backup { DatabaseName = "EDDS9", Type = BackupType.Full },
					new Backup { DatabaseName = "EDDS9", Type = BackupType.Differential },
					new Backup { DatabaseName = "EDDS9", Type = BackupType.Log },
				});

			this.backupProvider.Setup(bp => bp.GetLastBackupsBeforeHourAsync(hour, server, It.IsAny<IList<Database>>(), RiDefaults.AllSupportedBackupTypes))
				.ReturnsAsync(new[]
				{
					new Backup { DatabaseName = "EDDS7", Type = BackupType.Log },
					new Backup { DatabaseName = "EDDS9", Type = BackupType.Log },
				});

			this.backupProvider.Setup(bp => bp.GetLastBackupsBeforeHourAsync(hour, server, It.IsAny<IList<Database>>(), RiDefaults.FullAndDiffBackupTypes))
				.ReturnsAsync(new[]
				{
					new Backup { DatabaseName = "EDDS7", Type = BackupType.Full },
					new Backup { DatabaseName = "EDDS9", Type = BackupType.Full },
				});

			this.databaseBackupProcessor.Setup(dbgp =>
				dbgp.CreateGaps<BackupAllGap>(db7, It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns(new[] { new BackupAllGap { DatabaseId = 7 }, new BackupAllGap { DatabaseId = 7 } });

			this.databaseBackupProcessor.Setup(dbgp =>
					dbgp.CreateGaps<BackupAllGap>(db9, It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns(new[] { new BackupAllGap { DatabaseId = 9 }, new BackupAllGap { DatabaseId = 9 }, new BackupAllGap { DatabaseId = 9 } });

			this.databaseBackupProcessor.Setup(dbgp =>
					dbgp.CreateGaps<BackupFullDiffGap>(db7, It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns(new[]
				{
					new BackupFullDiffGap { DatabaseId = 7, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow },
					new BackupFullDiffGap { DatabaseId = 7, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow }
				});

			this.databaseBackupProcessor.Setup(dbgp =>
					dbgp.CreateGaps<BackupFullDiffGap>(db9, It.IsAny<IList<Backup>>(), It.IsAny<Backup>()))
				.Returns(new[]
				{
					new BackupFullDiffGap { DatabaseId = 9, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow },
					new BackupFullDiffGap { DatabaseId = 9, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow },
					new BackupFullDiffGap { DatabaseId = 9, Start = DateTime.UtcNow.AddDays(-10), End = DateTime.UtcNow }
				});

			// Act
			var result = await this.serverBackupProcessor.ProcessBackupsForDatabaseBatches(hour, server, databases);

			// Assert
			Assert.That(result.Where(bg => bg.ActivityType == GapActivityType.Backup).Count(bg => bg.DatabaseId == 7), Is.EqualTo(2));
			Assert.That(result.Where(bg => bg.ActivityType == GapActivityType.Backup).Count(bg => bg.DatabaseId == 9), Is.EqualTo(3));
			Assert.That(result.Where(bg => bg.ActivityType == GapActivityType.BackupFullAndDiff).Count(bg => bg.DatabaseId == 7), Is.EqualTo(2));
			Assert.That(result.Where(bg => bg.ActivityType == GapActivityType.BackupFullAndDiff).Count(bg => bg.DatabaseId == 9), Is.EqualTo(3));
		}
	}
}
