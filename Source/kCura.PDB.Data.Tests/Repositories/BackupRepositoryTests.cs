namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class BackupRepositoryTests
	{
		private BackupRepository backupRepository;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
			var generalSqlRepository = new GeneralSqlRepository();
			this.backupRepository = new BackupRepository(connectionFatory, generalSqlRepository);
		}

		[Test]
		public async Task BackupRepository_GetLastBackupsBeforeHour()
		{

			// Arrange
			var hour = new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { "EDDS" }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.backupRepository.GetLastBackupsBeforeHourAsync(hour, server, databaseNames, new[] { BackupType.Full, BackupType.Differential, BackupType.Log });

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.All(b => b.End < hour.HourTimeStamp), Is.Not.Empty, "results are before the hour");
			Assert.That(results.GroupBy(b => b.DatabaseName).All(bg => bg.Count() == 1), Is.Not.Empty, "results have only one record per database");
		}
	}

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class BackupRepositoryUnitPlatformTests
	{
		private BackupRepository backupRepository;

		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var generalSqlRepository = new GeneralSqlRepository();
			this.backupRepository = new BackupRepository(ConnectionFactorySetup.ConnectionFactory, generalSqlRepository);

			// Run full and diff backups
			await this.backupRepository.RunBackupAsync(BackupType.Full, Path.GetTempFileName());
			await this.backupRepository.RunBackupAsync(BackupType.Differential, Path.GetTempFileName());

			// Get the database name for queries (note: with localdb unit platform test implementation the database name probably has a random suffix)
			using (var connection = ConnectionFactorySetup.ConnectionFactory.GetEddsPerformanceConnection())
			{
				this.databaseName = new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
			}
		}

		private string databaseName;

		[Test]
		public async Task BackupRepositoryUnitPlatform_ReadBackupGaps()
		{
			// Arrange
			var normilizedHour = DateTime.UtcNow.NormilizeToHour();
			var hour = new Hour { HourTimeStamp = normilizedHour };
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { this.databaseName }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.backupRepository.GetBackupsAsync(hour, server, databaseNames);

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.Any(r => r.Type == BackupType.Full), Is.True, "full");
			Assert.That(results.Any(r => r.Type == BackupType.Differential), Is.True, "diff");
			//Assert.That(result.Any(r => r.Type == BackupType.Log), Is.True, "log");
			Assert.That(results.GroupBy(b => b.DatabaseName).All(bg => bg.Key == this.databaseName), Is.True, "results are filtered by target database");
			Assert.That(results.All(r => r.Start >= normilizedHour && r.Start < normilizedHour.AddHours(1)), Is.True, "results start are filtered to correct UTC time");
			Assert.That(results.All(r => r.End >= normilizedHour && r.End < normilizedHour.AddHours(1)), Is.True, "results end are filtered to correct UTC time");
		}

		[Test]
		public async Task BackupRepository_GetLastBackupsBeforeHour()
		{

			// Arrange
			var normilizedHour = DateTime.UtcNow.NormilizeToHour();
			var hour = new Hour { HourTimeStamp = normilizedHour.AddHours(1) };
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { this.databaseName }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.backupRepository.GetLastBackupsBeforeHourAsync(hour, server, databaseNames, Defaults.RecoverabilityIntegrity.AllSupportedBackupTypes);

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.All(b => b.End < hour.HourTimeStamp), Is.True, "results are before the hour");
			Assert.That(results.GroupBy(b => b.DatabaseName).All(bg => bg.Key == this.databaseName), Is.True, "results are filtered by target database");
			Assert.That(results.All(r => r.Start < normilizedHour.AddHours(1)), Is.True, "results start are filtered to correct UTC time");
			Assert.That(results.All(r => r.End < normilizedHour.AddHours(1)), Is.True, "results end are filtered to correct UTC time");
		}

		[Test]
		public async Task BackupRepository_GetLastBackupsBeforeHour_FilteredType()
		{

			// Arrange
			var normilizedHour = DateTime.UtcNow.NormilizeToHour();
			var hour = new Hour { HourTimeStamp = normilizedHour.AddHours(1) };
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { this.databaseName }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.backupRepository.GetLastBackupsBeforeHourAsync(hour, server, databaseNames, new[] { BackupType.Full });

			// Assert
			Assert.That(results, Is.Not.Empty);
			Assert.That(results.All(b => b.End < hour.HourTimeStamp), Is.True, "results are before the hour");
			Assert.That(results.GroupBy(b => b.DatabaseName).All(bg => bg.Key == this.databaseName), Is.True, "results are filtered by target database");
			Assert.That(results.All(r => r.Start < normilizedHour.AddHours(1)), Is.True, "results start are filtered to correct UTC time");
			Assert.That(results.All(r => r.End < normilizedHour.AddHours(1)), Is.True, "results end are filtered to correct UTC time");

			Assert.That(results.All(r => r.Type == BackupType.Full), Is.True, "results are all of type Full");
		}
	}
}
