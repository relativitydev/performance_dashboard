namespace kCura.PDB.Service.Tests.Servers
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Servers;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class DatabaseServiceTests
	{
		private DatabaseService databaseService;
		private Mock<IServerRepository> serverRepository;
		private Mock<IDatabaseRepository> databaseRepository;
		private ITimeService timeService;

		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.databaseRepository = new Mock<IDatabaseRepository>();
			this.timeService = TestUtilities.GetTimeService(1);
			this.databaseService = new DatabaseService(this.serverRepository.Object, this.databaseRepository.Object, this.timeService);
		}

		[Test]
		public async Task DatabaseService_UpdateTrackedDatabasesAsync_NoChange()
		{
			// Arrange
			var server = new Server { ServerId = 1234 };
			var previousServerDatabases = new[]
			{
				new Database { Id = 1, Name = "d1" },
				new Database { Id = 2, Name = "d2" },
			};
			this.serverRepository.Setup(r => r.ReadAllActiveAsync())
				.ReturnsAsync(new[] { server });
			this.databaseRepository.Setup(r => r.ReadByServerIdAsync(server.ServerId))
				.ReturnsAsync(previousServerDatabases);
			this.databaseRepository.Setup(r => r.GetByServerAsync(server))
				.ReturnsAsync(new[] { "d1", "d2" });
			this.databaseRepository.Setup(r => r.UpdateAsync(It.IsAny<Database>()))
				.Returns(Task.FromResult(1));
			this.databaseRepository.Setup(r => r.CreateAsync(It.IsAny<Database>()))
				.ReturnsAsync(new Database { Id = 123 });

			// Act
			await this.databaseService.UpdateTrackedDatabasesAsync();

			// Assert
			this.databaseRepository.Verify(r => r.UpdateAsync(It.IsAny<Database>()), Times.Never());
			this.databaseRepository.Verify(r => r.CreateAsync(It.IsAny<Database>()), Times.Never());
		}

		[Test]
		public async Task DatabaseService_MarkDatabasesDeleted()
		{
			// Arrange
			var dbToDelete = new Database { Id = 3, Name = "d3" };
			var previousServerDatabases = new[]
			{
				new Database { Id = 1, Name = "d1" },
				new Database { Id = 2, Name = "d2" },
				dbToDelete
			};

			var currentServerDatabaseNames = new[]
			{
				"d1",
				"d2"
			};

			this.databaseRepository.Setup(r => r.UpdateAsync(dbToDelete)).Returns(Task.FromResult(1));

			// Act
			await this.databaseService.MarkDatabasesDeleted(currentServerDatabaseNames, previousServerDatabases);

			// Assert
			this.databaseRepository.Verify(r => r.UpdateAsync(dbToDelete));
		}

		[Test]
		public async Task DatabaseService_CreateNewDatabases()
		{
			// Arrange
			var dbToCreate = "EDDS3";
			var dbName1 = "EDDS1";
			var dbName2 = "EDDS2";
			var previousServerDatabases = new[]
			{
				new Database { Id = 1, Name = dbName1 },
				new Database { Id = 2, Name = dbName2 },
			};

			var currentServerDatabaseNames = new[]
			{
				dbName1,
				dbName2,
				dbToCreate
			};
			var server = new Server { ServerId = 123 };

			this.databaseRepository.Setup(r => r.CreateAsync(It.Is<Database>(db => db.Name == dbToCreate && db.ServerId == server.ServerId)))
				.ReturnsAsync(new Database { Id = 123 });

			// Act
			await this.databaseService.CreateNewDatabases(server, currentServerDatabaseNames, previousServerDatabases);

			// Assert
			this.databaseRepository.Verify(r => r.CreateAsync(It.Is<Database>(db => db.Name == dbToCreate && db.ServerId == server.ServerId)));
			this.databaseRepository.VerifyAll();
		}

		[Test]
		[TestCase("CustomTestDB")]
		[TestCase("INV123")]
		public async Task DatabaseService_CreateNewDatabases_IgnoreNonRelativity(string dbToCreate)
		{
			// Arrange
			var dbName1 = "EDDS1";
			var dbName2 = "EDDS2";
			var previousServerDatabases = new[]
				                              {
					                              new Database { Id = 1, Name = dbName1 },
					                              new Database { Id = 2, Name = dbName2 },
				                              };

			var currentServerDatabaseNames = new[]
				                                 {
					                                 dbName1,
					                                 dbName2,
					                                 dbToCreate
				                                 };
			var server = new Server { ServerId = 123 };

			// Act
			await this.databaseService.CreateNewDatabases(server, currentServerDatabaseNames, previousServerDatabases);

			// Assert
			this.databaseRepository.VerifyAll();
		}

		[Test]
		[TestCase("edds", null)]
		[TestCase("edds123", 123)]
		[TestCase("inv123", 123)]
		[TestCase("blah", null)]
		[TestCase("edds123_123", null)]
		[TestCase("inv123_123", null)]
		public void DatabaseService_GetDatabaseWorkspace(string databaseName, int? expectedResult)
		{
			// Act
			var result = DatabaseService.GetDatabaseWorkspace(databaseName);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("edds", DatabaseType.Primary)]
		[TestCase("edds123", DatabaseType.Workspace)]
		[TestCase("inv123", DatabaseType.Invariant)]
		[TestCase("blah", DatabaseType.Other)]
		[TestCase("edds123_123", DatabaseType.Other)]
		[TestCase("inv123_123", DatabaseType.Other)]
		public void DatabaseService_GetDatabaseType(string databaseName, DatabaseType expectedResult)
		{
			// Act
			var result = DatabaseService.GetDatabaseType(databaseName);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
