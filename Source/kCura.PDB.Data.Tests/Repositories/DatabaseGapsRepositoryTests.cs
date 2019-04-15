namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class DatabaseGapsRepositoryTests
	{
		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var serverRepo = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			this.server = await serverRepo.CreateAsync(new Server
			{
				ServerName = Environment.MachineName,
				CreatedOn = DateTime.Now,
				DeletedOn = null,
				ServerTypeId = 3,
				ServerIpAddress = "127.0.0.1",
				IgnoreServer = false,
				ResponsibleAgent = "",
				ArtifactId = 1234,
			});

			this.databaseRepository = new DatabaseRepository(ConnectionFactorySetup.ConnectionFactory);
			this.databaseGapsRepository = new DatabaseGapsRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			var serverRepo = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			await serverRepo.DeleteAsync(this.server);
		}

		private Server server;
		private ServerRepository serverRepository;
		private DatabaseRepository databaseRepository;
		private DatabaseGapsRepository databaseGapsRepository;

		[Test]
		public async Task DatabaseRepository_CreateDatabaseGapsAsync()
		{
			// Arrange
			var database = await this.databaseRepository.CreateAsync(new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234
			});
			var gaps = new[]
			{
				new BackupAllGap { DatabaseId = database.Id, Start = DateTime.UtcNow.AddHours(1), End = DateTime.UtcNow },
				new BackupAllGap { DatabaseId = database.Id, Start = DateTime.UtcNow.AddHours(2), End = DateTime.UtcNow.AddHours(1) }
			};

			// Act
			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gaps);

			// Assert
			Assert.Pass("No return result");
			// TODO when reading gaps is implemented then add query to read back the gaps to confirm they're created
		}

		[Test]
		public async Task DatabaseRepository_ReadLargestGapsForHourAsync()
		{
			// Arrange
			var database = await this.databaseRepository.CreateAsync(new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234
			});

			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };

			var gaps = new[]
			{
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(1), End = hour.HourTimeStamp.AddMinutes(2) }, // duration = 1 mins
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(3), End = hour.HourTimeStamp.AddMinutes(5) }, // duration = 2 mins
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(6), End = hour.HourTimeStamp.AddMinutes(16) }, // duration = 10 mins ***
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(17), End = hour.HourTimeStamp.AddMinutes(18) }, // duration = 1 mins
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(18), End = hour.HourTimeStamp.AddMinutes(19) }, // duration = 1 mins
				new BackupAllGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(20), End = hour.HourTimeStamp.AddMinutes(25) }, // duration = 5 mins
			};

			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gaps);

			// Act
			var result = await this.databaseGapsRepository.ReadLargestGapsForHourAsync<BackupAllGap>(server, hour, GapActivityType.Backup);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ActivityType, Is.EqualTo(GapActivityType.Backup));
			Assert.That(result.Start, Is.EqualTo(hour.HourTimeStamp.AddMinutes(6)));
			Assert.That(result.End, Is.EqualTo(hour.HourTimeStamp.AddMinutes(16)));
			Assert.That(result.Duration, Is.EqualTo(TimeSpan.FromMinutes(10).TotalSeconds));
		}

		[Test]
		public async Task DatabaseGaps_ReadGapsLargerThanForHourAsync()
		{
			// Arrange
			var database = await this.databaseRepository.CreateAsync(new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database",
				Type = DatabaseType.Other,
				WorkspaceId = 1234
			});

			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };

			var gaps = new[]
			{
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(1), End = hour.HourTimeStamp.AddMinutes(2) }, // duration = 1 mins
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(3), End = hour.HourTimeStamp.AddMinutes(5) }, // duration = 2 mins
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(6), End = hour.HourTimeStamp.AddMinutes(16) }, // duration = 10 mins ***
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(17), End = hour.HourTimeStamp.AddMinutes(18) }, // duration = 1 mins
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(18), End = hour.HourTimeStamp.AddMinutes(19) }, // duration = 1 mins
				new DbccGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(20), End = hour.HourTimeStamp.AddMinutes(25) }, // duration = 5 mins ***
			};

			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gaps);
			var minDuration = TimeSpan.FromMinutes(5).TotalSeconds;

			// Act
			var results = await this.databaseGapsRepository.ReadGapsLargerThanForHourAsync<DbccGap>(server, hour, GapActivityType.Dbcc, (int)minDuration);

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.Count, Is.GreaterThanOrEqualTo(2), "There should be at least two since we created 2 above");
			Assert.That(results.All(r => r.Duration >= minDuration), Is.True, "All durations should be greater than or equal to the min duration");
		}

		[Test]
		public async Task DatabaseGaps_ReadLargestGapsForEachDatabaseAsync()
		{
			// Arrange
			var database = await this.databaseRepository.CreateAsync(new Database
			{
				ServerId = this.server.ServerId,
				Name = "test database 2",
				Type = DatabaseType.Other,
				WorkspaceId = 12345
			});

			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };

			var gaps = new[]
			{
				new BackupFullDiffGap { DatabaseId = database.Id, Start = hour.HourTimeStamp.AddMinutes(1), End = hour.HourTimeStamp.AddMinutes(2) }, // duration = 1 mins
			};

			await this.databaseGapsRepository.CreateDatabaseGapsAsync(gaps);
			var minDuration = TimeSpan.FromMinutes(5).TotalSeconds;

			// Act
			var results = await this.databaseGapsRepository.ReadLargestGapsForEachDatabaseAsync<BackupFullDiffGap>(server, hour, GapActivityType.BackupFullAndDiff);

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.Count, Is.GreaterThanOrEqualTo(1), "There should be at least one");
		}
	}
}
