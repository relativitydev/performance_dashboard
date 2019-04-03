namespace kCura.PDB.Service.Integration.Tests.DatabaseDeployment
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Integration.Tests.Properties;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	//If any of these tests are interrupted or fail, you may have cruft left over in C:\Users\{User}\AppData\Roaming
	//Be sure to delete the numbered PDB folders there to prevent collisions in future test runs
	public class DatabaseMigratorTests_TestSuite
	{
		private DatabaseMigratorFactory databaseMigratorFactory;
		private IConnectionFactory connectionFactory;

		[SetUp]
		public void Setup()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.databaseMigratorFactory = new DatabaseMigratorFactory(this.connectionFactory);
		}


		/// <summary>
		/// This test needs to run under x86 Architecture to pass b/c it references the 
		/// roundhouse dll
		/// </summary>
		[Test]
		//[Timeout(TestTimeout.Infinite)]
		public void PerformanceDeploymentService_SmokeTest()
		{
			//Arrange
			var sqlRepository = new SqlServerRepository(this.connectionFactory);
			var migrator = this.databaseMigratorFactory.GetPerformanceMigrator(Config.Server);

			//Act
			var results = migrator.Deploy();

			var messages = results.Messages
				.Select((m, i) => $"{i} - {m.Severity}:\r\n{m.Message}\r\n");

			Console.WriteLine(string.Join("\r\n", messages.ToArray()));
			//Assert
			Assert.IsTrue(results.Success, "RoundHouse failed.");
		}

		[Test]
		public void QoSDeploymentService_SmokeTest()
		{
			//Arrange
			var migrator = this.databaseMigratorFactory.GetQosDeploymentMigrator(kCura.PDB.Tests.Common.Config.Server);

			//Act
			var results = migrator.Deploy();

			var messages = from message in results.Messages
						   where message.Severity == LogSeverity.Warning || message.Severity == LogSeverity.Error
						   select message.Message;

			//Assert
			Assert.IsTrue(results.Success, String.Join(" ", messages.ToArray()));
		}

		[Test]
		public void QoSDeploymentService_RedeployScripts()
		{
			//Arrange
			var migrator = this.databaseMigratorFactory.GetQosDeploymentMigrator(kCura.PDB.Tests.Common.Config.Server);

			//Act
			var results = migrator.ReDeployScripts();

			var messages = from message in results.Messages
						   where message.Severity == LogSeverity.Warning || message.Severity == LogSeverity.Error
						   select message.Message;

			//Assert
			var msg = String.Join(" ", messages.ToArray());
			Assert.IsTrue(results.Success, String.Join(" ", messages.ToArray()));
		}

		[Test]
		public void ResourceDeploymentService_Deploy_SmokeTest()
		{
			//Arrange
			var migrator = this.databaseMigratorFactory.GetResourceMigrator(kCura.PDB.Tests.Common.Config.Server, TestUtilities.GetSACredentialInfo());

			//Act
			var results = migrator.Deploy();

			var errorMessages = from message in results.Messages
								where message.Severity == LogSeverity.Warning || message.Severity == LogSeverity.Error
								select message.Message;

			//Assert
			Assert.IsTrue(results.Success, errorMessages.Join("{0}\r\n{1}"));
			Assert.That(errorMessages, Is.Empty, errorMessages.Join("{0}\r\n{1}"));
		}

		[Test]
		public void ResourceDeploymentService_ReDeployScripts_SmokeTest()
		{
			//Arrange
			var migrator = this.databaseMigratorFactory.GetResourceMigrator(kCura.PDB.Tests.Common.Config.Server, TestUtilities.GetSACredentialInfo());

			//Act
			var results = migrator.ReDeployScripts();

			var errorMessages = from message in results.Messages
								where message.Severity == LogSeverity.Warning || message.Severity == LogSeverity.Error
								select message.Message;

			//Assert
			Assert.IsTrue(results.Success, errorMessages.Join("{0}\r\n{1}"));
			Assert.That(errorMessages, Is.Empty, errorMessages.Join("{0}\r\n{1}"));
		}

		[Test]
		public void TestingDeploymentService_SmokeTest()
		{
			// Arrange
			var migrator = this.databaseMigratorFactory.GetTestingDeploymentMigrator(Config.Server, Names.Database.EddsPerformance,
				Resources.MigrateTesting);

			// Act
			var results = migrator.Deploy();

			var messages = from message in results.Messages
						   where message.Severity == LogSeverity.Warning || message.Severity == LogSeverity.Error
						   select message.Message;

			//Assert
			Assert.IsTrue(results.Success, string.Join(" ", messages.ToArray()));
		}
	}
}
