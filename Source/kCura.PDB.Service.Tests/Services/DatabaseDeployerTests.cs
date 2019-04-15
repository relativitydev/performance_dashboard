namespace kCura.PDB.Service.Tests.Services
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class DatabaseDeployerTests
	{
		[SetUp]
		public void Setup()
		{
			this.logger = TestUtilities.GetMockLogger().Object;
			this.databaseMigratorFactory = new Mock<IDatabaseMigratorFactory>();
			this.databaseMigrator = new Mock<IDatabaseMigrator>();
			this.migrationResultHandler = new Mock<IMigrationResultHandler>();
			this.migrationResultHandler.Setup(h => h.HandleDeploymentResponse(It.IsAny<MigrationResultSet>()))
				.Returns<MigrationResultSet>(mrs => mrs);
			var resultSet = new MigrationResultSet() { Success = true, Messages = new[]
			{
				new LogMessage(LogSeverity.Debug, "a"),
				new LogMessage(LogSeverity.Info, "b"),
				new LogMessage(LogSeverity.Warning, "c")
			} };
			this.databaseMigrator.Setup(s => s.Deploy()).Returns(resultSet);
			this.databaseDeployer = new DatabaseDeployer(databaseMigratorFactory.Object, this.migrationResultHandler.Object);
		}

		private ILogger logger;
		private Mock<IDatabaseMigratorFactory> databaseMigratorFactory;
		private Mock<IMigrationResultHandler> migrationResultHandler;
		private Mock<IDatabaseMigrator> databaseMigrator;
		private DatabaseDeployer databaseDeployer;

		[Test]
		public void DatabaseDeployer_DeployQos()
		{
			// Arrange
			var server = new Server { ServerId = 123, ServerName = "abc" };
			this.databaseMigratorFactory.Setup(f => f.GetQosDeploymentMigrator(server.ServerName))
				.Returns(this.databaseMigrator.Object);

			// Act
			var results = databaseDeployer.DeployQos(server);

			// Assert
			Assert.That(results.Success, Is.True);
		}

		[Test]
		public void DatabaseDeployer_DeployPerformance()
		{
			// Arrange
			this.databaseMigratorFactory.Setup(f => f.GetPerformanceMigrator(It.IsAny<string>()))
				.Returns(this.databaseMigrator.Object);

			// Act
			var results = databaseDeployer.DeployPerformance("primary-sql-server");

			// Assert
			Assert.That(results.Success, Is.True);
		}

		[Test]
		public void DatabaseDeployer_DeployResource()
		{
			// Arrange
			var creds = new GenericCredentialInfo();
			var server = new Server { ServerId = 123, ServerName = "abc" };
			this.databaseMigratorFactory.Setup(f => f.GetResourceMigrator(server.ServerName, creds))
				.Returns(this.databaseMigrator.Object);

			// Act
			var results = databaseDeployer.DeployResource(server, creds);

			// Assert
			Assert.That(results.Success, Is.True);
		}

		[Test]
		public void DatabaseDeployer_DeployTesting()
		{
			// Arrange
			var serverName = "abc";
			var database = Names.Database.EddsPerformance;
			this.databaseMigratorFactory.Setup(f => f.GetTestingDeploymentMigrator(serverName, database, It.IsAny<byte[]>()))
				.Returns(this.databaseMigrator.Object);

			// Act
			var results = databaseDeployer.DeployTesting(serverName, database, new byte[0]);

			// Assert
			Assert.That(results.Success, Is.True);
		}
	}
}
