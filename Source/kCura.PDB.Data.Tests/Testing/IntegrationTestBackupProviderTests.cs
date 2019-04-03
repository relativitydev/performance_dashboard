namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class IntegrationTestBackupProviderTests
	{
		private IntegrationTestBackupProvider backupProvider;
		private BackupTestDataRepository backupTestDataRepository;

		[SetUp]
		public async Task Setup()
		{
			this.backupProvider = new IntegrationTestBackupProvider(ConnectionFactorySetup.ConnectionFactory);
			this.backupTestDataRepository = new BackupTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task GetBackupsAsync()
		{
			// Arrange 
			var hourTimeStamp = DateTime.UtcNow.NormilizeToHour();
			var backupEnd = hourTimeStamp.AddMinutes(5);
			var serverName = "TestServerName";
			var databaseName = "TestDatabaseName";

			var testBackup = new MockBackupSet
			{
				Server = serverName,
				Database = databaseName,
				BackupStartDate = hourTimeStamp,
				BackupEndDate = backupEnd,
				BackupType = ((char)BackupType.Full).ToString()
			};
			await this.backupTestDataRepository.CreateAsync(new List<MockBackupSet> { testBackup });

			var hour = new Hour { HourTimeStamp = hourTimeStamp };
			var server = new Server { ServerName = serverName };
			var database = new Database { Name = databaseName };
			var databases = new List<Database> { database };

			// Act
			var results = await this.backupProvider.GetBackupsAsync(hour, server, databases);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			var result = results.First();
			Assert.That(result.DatabaseName, Is.EqualTo(testBackup.Database));
			Assert.That(result.Start, Is.EqualTo(testBackup.BackupStartDate));
			Assert.That(result.End, Is.EqualTo(testBackup.BackupEndDate));
			Assert.That(result.BackupType.ToString(), Is.EqualTo(testBackup.BackupType));
		}

		[Test]
		public async Task GetLastBackupsBeforeHourAsync()
		{
			// Arrange 
			var hourTimeStamp = DateTime.UtcNow.NormilizeToHour();
			var backupEnd = DateTime.UtcNow.NormilizeToHour().AddMinutes(-30);
			var serverName = "TestServerName";
			var databaseName = "TestDatabaseName";
			var backupType = BackupType.Full;

			var testBackup = new MockBackupSet
				                 {
					                 Server = serverName,
					                 Database = databaseName,
					                 BackupStartDate = backupEnd.AddMinutes(-5),
					                 BackupEndDate = backupEnd,
					                 BackupType = ((char)backupType).ToString()
				                 };
			await this.backupTestDataRepository.CreateAsync(new List<MockBackupSet> { testBackup });

			var hour = new Hour { HourTimeStamp = hourTimeStamp };
			var server = new Server { ServerName = serverName };
			var database = new Database { Name = databaseName };
			var databases = new List<Database> { database };

			var backupTypes = new List<BackupType> { backupType };

			var results = await this.backupProvider.GetLastBackupsBeforeHourAsync(hour, server, databases, backupTypes);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			var result = results.First();
			Assert.That(result.DatabaseName, Is.EqualTo(testBackup.Database));
			Assert.That(result.Start, Is.EqualTo(testBackup.BackupStartDate));
			Assert.That(result.End, Is.EqualTo(testBackup.BackupEndDate));
			Assert.That(result.BackupType.ToString(), Is.EqualTo(testBackup.BackupType));
		}

		[TearDown]
		public async Task TearDown()
		{
			await this.backupTestDataRepository.ClearAsync();
		}
	}
}
