namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class IntegrationTestDbccProviderTests
	{
		private IntegrationTestDbccProvider dbccProvider;
		private DbccTestDataRepository dbccTestDataRepository;

		private MockDbccServerResults dbccTest;

		[SetUp]
		public async Task Setup()
		{
			// Setup provider
			this.dbccProvider = new IntegrationTestDbccProvider(ConnectionFactorySetup.ConnectionFactory);
			this.dbccTestDataRepository = new DbccTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task GetDbccsAsync()
		{
			// Setup Server
			var server = new Server { ServerName = "TestServerName" };
			var databaseName = "TestDatabaseName";
			this.dbccTest = new MockDbccServerResults
				                {
					                Database = databaseName,
					                LastCleanDBCCDate = DateTime.UtcNow.NormilizeToHour().AddHours(-2),
					                Server = server.ServerName
				                };
			await this.dbccTestDataRepository.CreateAsync(new List<MockDbccServerResults> { this.dbccTest });

			var testDatabase = new Database { Name = databaseName };
			var databases = new List<Database> { testDatabase };

			// Act
			var results = await this.dbccProvider.GetDbccsAsync(server, databases);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			var result = results.First();
			Assert.That(result.End, Is.EqualTo(this.dbccTest.LastCleanDBCCDate));
		}

		[TearDown]
		public async Task TearDown()
		{
			await this.dbccTestDataRepository.ClearAsync();
		}
	}
}
