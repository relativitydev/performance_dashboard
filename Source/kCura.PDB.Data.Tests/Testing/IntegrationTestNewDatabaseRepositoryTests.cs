namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Repositories.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class IntegrationTestNewDatabaseRepositoryTests
	{
		private IntegrationTestNewDatabaseRepository repository;
		private DatabasesCheckedTestDataRepository databasesCheckedTestDataRepository;


		[SetUp]
		public async Task Setup()
		{
			this.repository = new IntegrationTestNewDatabaseRepository(ConnectionFactorySetup.ConnectionFactory);
			this.databasesCheckedTestDataRepository = new DatabasesCheckedTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task GetByServerAsync()
		{
			// Arrange
			var server = new Server { ServerName = "TestServerName" };

			var mockDatabase = new MockDatabaseChecked
				                    {
					                    Database = "TestDatabaseName",
					                    Server = server.ServerName,
					                    CreatedOn = DateTime.UtcNow
				                    };
			await this.databasesCheckedTestDataRepository.CreateAsync(new List<MockDatabaseChecked> { mockDatabase });


			// Act
			var results = await this.repository.GetByServerAsync(server);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			var testDatabase = results.First();
			Assert.That(testDatabase, Is.EqualTo(mockDatabase.Database));
		}

		[TearDown]
		public async Task TearDown()
		{
			await this.databasesCheckedTestDataRepository.ClearAsync();
		}
	}
}
