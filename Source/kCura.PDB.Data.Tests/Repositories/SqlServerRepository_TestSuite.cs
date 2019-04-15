namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class SqlServerRepository_TestSuite
	{
		private SqlServerRepository sqlServerRepository;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.sqlServerRepository = new SqlServerRepository(connectionFactory);
		}

		[Test]
		public void GetRegisteredSQLServers()
		{
			var result = this.sqlServerRepository.GetRegisteredSQLServers();

			Assert.That(result, Is.Not.Empty);
		}
		
		[Test]
		public void ReadRelativitySMTPSettings()
		{
			var result = this.sqlServerRepository.ReadRelativitySMTPSettings();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ReadServerUtcTime()
		{
			var result = this.sqlServerRepository.ReadServerUtcTime();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ReadInstanceName()
		{
			var result = this.sqlServerRepository.ReadInstanceName();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		[Ignore("TODO Fix me")]
		public void GetUserExperienceForecast()
		{
			var server = Config.Server;
			var result = this.sqlServerRepository.UserExperienceForecastForServer(server);

			Assert.That(result, Is.Not.Empty);
		}

		[Test]
		public void GetSystemLoadForecast()
		{
			var result = this.sqlServerRepository.SystemLoadForecast();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		[Ignore("TODO Fix me")]
		public void SummarizeSqlServerPageouts()
		{
			this.sqlServerRepository.SummarizeSqlServerPageouts();
		}

		[Test]
		[Ignore("TODO Fix me")]
		public void RunBackupDBCCMonitor()
		{
			this.sqlServerRepository.ExecuteBackupDBCCMonitor(51773, Names.Database.BackupAndDBCCMonLauncherSproc);
		}
	}
}
