namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class DbccRepositoryTests
	{
		private DbccRepository dbccRepository;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var generalSqlRepository = new GeneralSqlRepository();
			var connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
			this.dbccRepository = new DbccRepository(connectionFatory, generalSqlRepository);
		}

		[Test]
		public async Task DbccRepository_ReadDbccsAsync()
		{
			// Arrange
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { "EDDS" }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.dbccRepository.GetDbccsAsync(server, databaseNames);

			// Assert
			Assert.That(results, Is.Not.Empty);
		}
	}

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class DbccRepositoryUnitPlatformTests
	{
		private DbccRepository dbccRepository;
		private string databaseName;
		private DateTime startDbccRun;
		private DateTime endDbccRun;

		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			var generalSqlRepository = new GeneralSqlRepository();
			this.dbccRepository = new DbccRepository(ConnectionFactorySetup.ConnectionFactory, generalSqlRepository);

			this.startDbccRun = DateTime.UtcNow;
			await this.dbccRepository.RunDbcc();
			this.endDbccRun = DateTime.UtcNow;

			// Get the database name for queries (note: with localdb unit platform test implementation the database name probably has a random suffix)
			using (var connection = ConnectionFactorySetup.ConnectionFactory.GetEddsPerformanceConnection())
			{
				this.databaseName = new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
			}
		}

		[Test]
		public async Task DbccRepository_ReadDbccsAsync()
		{
			// Arrange
			var server = new Server { ServerName = Config.Server };
			var databaseNames = new[] { databaseName }.Select(d => new Database { Name = d }).ToList();

			// Act
			var results = await this.dbccRepository.GetDbccsAsync(server, databaseNames);

			// Assert
			Assert.That(results, Is.Not.Empty);
			
			// Shows that the dbcc dates are being  correctly updated to UTC
			Assert.That(results.First().End, Is.GreaterThan(this.startDbccRun));
			Assert.That(results.First().End, Is.LessThan(this.endDbccRun));
		}
	}
}
