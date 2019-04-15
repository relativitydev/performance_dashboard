namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class PerformanceSqlServerRepositoryTests
	{
		private SqlServerRepository sqlServerRepository;

		[OneTimeSetUp]
		public void SetUp()
		{
			this.sqlServerRepository = new SqlServerRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task AdminScriptsInstalled_True()
		{
			// Ensure environment state
			var serverRepo = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			var servers = serverRepo.ReadAllActive();
			await servers.Select(s => serverRepo.DeleteAsync(s)).WhenAllStreamed(1);

			var result = this.sqlServerRepository.AdminScriptsInstalled();

			Assert.That(result, Is.True); // This should be true if there are no servers listed in the db
		}

		[Test]
		public async Task AdminScriptsInstalled_False()
		{
			// Ensure environment state
			var serverRepo = new ServerRepository(ConnectionFactorySetup.ConnectionFactory);
			var testServer = await serverRepo.CreateAsync(new Server { CreatedOn = DateTime.UtcNow, ServerType = ServerType.Database});

			var result = this.sqlServerRepository.AdminScriptsInstalled();

			Assert.That(result, Is.False);

			// Teardown
			await serverRepo.DeleteAsync(testServer);
		}

		[Test]
		public void CleanupDataTables()
		{
			// Arrange
			this.sqlServerRepository.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.DataCleanupThresholdDays);

			// Act
			this.sqlServerRepository.CleanupDataTables();

			// Asserts
			Assert.Pass("No return result");
		}

		[Test]
		public void IfInDebugModeLaunchDebugger_Test_False()
		{
			// Arrange
			this.sqlServerRepository.ConfigurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.LaunchDebugger, "false");

			var success = this.sqlServerRepository.IfInDebugModeLaunchDebugger();

			Assert.IsFalse(success);
		}

		[Test]
		public void IsVersionedWithLegacyHashes()
		{
			var result = this.sqlServerRepository.IsVersionedWithLegacyHashes();

			Assert.That(result, Is.False);
		}

		[Test]
		public void PerformanceExists()
		{
			var result = this.sqlServerRepository.PerformanceExists();

			Assert.Pass($"Result can be any value.  Current value: {result}");
			Console.WriteLine($"PerformanceExists - {result}");
		}

		[Test]
		public void ListDbccTargets()
		{
			this.sqlServerRepository.RefreshDbccTargets();

			var result = this.sqlServerRepository.ListDbccTargets();

			Assert.That(result, Is.Not.Empty);
		}
	}
}
