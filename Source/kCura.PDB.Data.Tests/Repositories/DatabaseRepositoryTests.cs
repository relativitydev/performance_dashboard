namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Data.SqlTypes;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using RiDefaults = kCura.PDB.Core.Constants.Defaults.RecoverabilityIntegrity;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class DatabaseRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			this.serverRepository = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			this.server = await this.serverRepository.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.1",
				IgnoreServer = false,
				ResponsibleAgent = "",
				ArtifactId = 1234,
				LastChecked = null,
				UptimeMonitoringResourceHost = null,
				UptimeMonitoringResourceUseHttps = null,
				LastServerBackup = null,
				AdminScriptsVersion = null,
			});

			this.databaseRepository = new DatabaseRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			await this.serverRepository.DeleteAsync(this.server);
		}

		private Server server;
		private ServerRepository serverRepository;
		private DatabaseRepository databaseRepository;

		[Test]
		public async Task DatabaseRepository_Create()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = new DateTime(2018, 1, 1),
				LastBackupLogDate = new DateTime(2018, 2, 2),
				LastBackupDiffDate = new DateTime(2018, 3, 3),
				LastBackupFullDate = new DateTime(2018, 4, 4),
				LastBackupFullDuration = 7,
				LastBackupDiffDuration = 11,
				LogBackupsDuration = 13,
			};

			// Act
			var result = await this.databaseRepository.CreateAsync(database);

			// Assert
			Assert.That(result.Id, Is.GreaterThan(0));
			Assert.That(result.ServerId, Is.EqualTo(this.server.ServerId));
			Assert.That(result.Name, Is.EqualTo("test database"));
			Assert.That(result.Type, Is.EqualTo(DatabaseType.Other));
			Assert.That(result.WorkspaceId, Is.EqualTo(1234));
			Assert.That(result.DeletedOn, Is.Null);
			Assert.That(result.Ignore, Is.False);
			Assert.That((result.LastDbccDate.Value - new DateTime(2018, 1, 1)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupLogDate.Value - new DateTime(2018, 2, 2)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupDiffDate.Value - new DateTime(2018, 3, 3)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupFullDate.Value - new DateTime(2018, 4, 4)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That(result.LastBackupFullDuration, Is.EqualTo(7));
			Assert.That(result.LastBackupDiffDuration, Is.EqualTo(11));
			Assert.That(result.LogBackupsDuration, Is.EqualTo(13));
		}

		[Test]
		public async Task DatabaseRepository_Create_AlreadyExists()
		{
			// Arrange
			var name = Guid.NewGuid().ToString();
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = name,
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = new DateTime(2018, 1, 1),
				LastBackupLogDate = new DateTime(2018, 2, 2),
				LastBackupDiffDate = new DateTime(2018, 3, 3),
				LastBackupFullDate = new DateTime(2018, 4, 4),
				LastBackupFullDuration = 7,
				LastBackupDiffDuration = 11,
				LogBackupsDuration = 13,
			};
			await this.databaseRepository.CreateAsync(database);
			database.DeletedOn = new DateTime(2012, 1, 1);
			database.Ignore = true;
			database.LastDbccDate = new DateTime(2012, 5, 8);
			database.LastBackupLogDate = new DateTime(2012, 6, 7);
			database.LastBackupDiffDate = new DateTime(2012, 7, 6);
			database.LastBackupFullDate = new DateTime(2012, 8, 5);
			database.LastBackupFullDuration = 17;
			database.LastBackupDiffDuration = 19;
			database.LogBackupsDuration = 23;

			// Act
			var result = await this.databaseRepository.CreateAsync(database);

			// Assert
			Assert.That(result.Id, Is.GreaterThan(0));
			Assert.That(result.ServerId, Is.EqualTo(this.server.ServerId));
			Assert.That(result.Name, Is.EqualTo(name));
			Assert.That(result.Type, Is.EqualTo(DatabaseType.Other));
			Assert.That(result.WorkspaceId, Is.EqualTo(1234));
			Assert.That((result.DeletedOn.Value - new DateTime(2012, 1, 1)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That(result.Ignore, Is.True);
			Assert.That((result.LastDbccDate.Value - new DateTime(2012, 5, 8)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupLogDate.Value - new DateTime(2012, 6, 7)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupDiffDate.Value - new DateTime(2012, 7, 6)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupFullDate.Value - new DateTime(2012, 8, 5)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That(result.LastBackupFullDuration, Is.EqualTo(17));
			Assert.That(result.LastBackupDiffDuration, Is.EqualTo(19));
			Assert.That(result.LogBackupsDuration, Is.EqualTo(23));
		}

		[Test]
		public async Task DatabaseRepository_Update()
		{
			// Arrange
			var name = Guid.NewGuid().ToString();
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = name,
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = new DateTime(2018, 1, 1),
				LastBackupLogDate = new DateTime(2018, 2, 2),
				LastBackupDiffDate = new DateTime(2018, 3, 3),
				LastBackupFullDate = new DateTime(2018, 4, 4),
				LastBackupFullDuration = 7,
				LastBackupDiffDuration = 11,
				LogBackupsDuration = 13,
			};
			var createdDatabase = await this.databaseRepository.CreateAsync(database);
			database.DeletedOn = new DateTime(2012, 1, 1);
			database.Ignore = true;
			database.LastDbccDate = new DateTime(2012, 5, 8);
			database.LastBackupLogDate = new DateTime(2012, 6, 7);
			database.LastBackupDiffDate = new DateTime(2012, 7, 6);
			database.LastBackupFullDate = new DateTime(2012, 8, 5);
			database.LastBackupFullDuration = 17;
			database.LastBackupDiffDuration = 19;
			database.LogBackupsDuration = 23;

			// Act
			await this.databaseRepository.UpdateAsync(database);
			var result = await this.databaseRepository.ReadAsync(createdDatabase.Id);

			// Assert
			Assert.That(result.Id, Is.GreaterThan(0));
			Assert.That(result.ServerId, Is.EqualTo(this.server.ServerId));
			Assert.That(result.Name, Is.EqualTo(name));
			Assert.That(result.Type, Is.EqualTo(DatabaseType.Other));
			Assert.That(result.WorkspaceId, Is.EqualTo(1234));
			Assert.That((result.DeletedOn.Value - new DateTime(2012, 1, 1)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That(result.Ignore, Is.True);
			Assert.That((result.LastDbccDate.Value - new DateTime(2012, 5, 8)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupLogDate.Value - new DateTime(2012, 6, 7)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupDiffDate.Value - new DateTime(2012, 7, 6)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That((result.LastBackupFullDate.Value - new DateTime(2012, 8, 5)).TotalMilliseconds, Is.LessThanOrEqualTo(0.1));
			Assert.That(result.LastBackupFullDuration, Is.EqualTo(17));
			Assert.That(result.LastBackupDiffDuration, Is.EqualTo(19));
			Assert.That(result.LogBackupsDuration, Is.EqualTo(23));
		}

		[Test]
		public async Task DatabaseRepository_ReadByServerId()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadByServerIdAsync(this.server.ServerId);

			// Assert
			Assert.That(result, Is.Not.Empty);
			Assert.That(result.All(d => d.ServerId == this.server.ServerId), Is.True);
		}

		[Test]
		public async Task DatabaseRepository_ReadMostOutOfDateDbccByServerAsync()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = SqlDateTime.MinValue.Value // Create worst case so it has to be the most out of date by dbcc
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadMostOutOfDateActivityByServerAsync(this.server, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Value, Is.EqualTo(SqlDateTime.MinValue.Value));
		}

		[Test]
		public async Task DatabaseRepository_ReadMostOutOfDateBackupByServerAsync()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastBackupFullDate = SqlDateTime.MinValue.Value.AddHours(1), // Create worst case so it has to be the most out of date by backup
				LastBackupDiffDate = SqlDateTime.MinValue.Value.AddHours(2), // this should not be picked since it's after the full
				LastBackupLogDate = SqlDateTime.MinValue.Value // this is the worst of the 3 but it should not be picked since it's log backup
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadMostOutOfDateActivityByServerAsync(this.server, GapActivityType.BackupFullAndDiff);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Value, Is.EqualTo(SqlDateTime.MinValue.Value.AddHours(1)));
		}

		[Test]
		public async Task DatabaseRepository_ReadMostOutOfDateBackupByServerAsync_Backup_Null()
		{
			// Arrange
			var testServer = await this.serverRepository.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.2",
				IgnoreServer = false,
				ArtifactId = 2345,
			});
			var database = new Database
			{
				ServerId = testServer.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadMostOutOfDateActivityByServerAsync(testServer, GapActivityType.BackupFullAndDiff);

			// Assert
			Assert.That(result, Is.Null);

			// TearDown
			await this.serverRepository.DeleteAsync(testServer);
		}

		[Test]
		public async Task DatabaseRepository_ReadMostOutOfDateBackupByServerAsync_Dbcc_Null()
		{
			// Arrange
			var testServer = await this.serverRepository.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.3",
				IgnoreServer = false,
				ArtifactId = 3456,
			});
			var database = new Database
			{
				ServerId = testServer.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadMostOutOfDateActivityByServerAsync(testServer, GapActivityType.Dbcc);

			// Assert
			Assert.That(result, Is.Null);

			// TearDown
			await this.serverRepository.DeleteAsync(testServer);
		}

		[Test]
		public async Task DatabaseRepository_ReadOutOfDateDatabasesAsync_Backups()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastBackupFullDate = SqlDateTime.MinValue.Value.AddDays(15),
			};
			database = await this.databaseRepository.CreateAsync(database);

			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var windowExceededDate = hour.HourTimeStamp.AddDays(-RiDefaults.WindowInDays);

			// Act
			var results = await this.databaseRepository.ReadOutOfDateDatabasesAsync(this.server, windowExceededDate, GapActivityType.BackupFullAndDiff);

			// Assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Any(r => r.Id == database.Id), Is.True, "results should contain the database just created");
			Assert.That(
				results.All(r =>
					(r.LastBackupDiffDate.HasValue && r.LastBackupDiffDate.Value < windowExceededDate)
					|| (r.LastBackupFullDate.HasValue && r.LastBackupFullDate.Value < windowExceededDate)),
				Is.True,
				"all results should have a LastBackupDiffDate or LastBackupFullDate date that is less than the window exceeded by date");
		}

		[Test]
		public async Task DatabaseRepository_ReadOutOfDateDatabasesAsync_Dbccs()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = SqlDateTime.MinValue.Value.AddDays(15),
			};
			database = await this.databaseRepository.CreateAsync(database);

			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var windowExceededDate = hour.HourTimeStamp.AddDays(-RiDefaults.WindowInDays);

			// Act
			var results = await this.databaseRepository.ReadOutOfDateDatabasesAsync(this.server, windowExceededDate, GapActivityType.Dbcc);

			// Assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Any(r => r.Id == database.Id), Is.True, "results should contain the database just created");
			Assert.That(
				results.All(r => r.LastDbccDate.HasValue && r.LastDbccDate.Value < windowExceededDate),
				Is.True,
				"all results should have a LastBackupDiffDate or LastBackupFullDate date that is less than the window exceeded by date");
		}

		[Test]
		public async Task DatabaseRepository_ReadCountByServerAsync()
		{
			// Arrange
			var database = new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234,
				LastDbccDate = SqlDateTime.MinValue.Value.AddDays(15),
			};
			await this.databaseRepository.CreateAsync(database);

			// Act
			var result = await this.databaseRepository.ReadCountByServerAsync(this.server);

			// Assert
			// Backups
			Assert.That(result, Is.GreaterThanOrEqualTo(1));
		}
	}
}
