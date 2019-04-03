namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class DeploymentRepositoryTests
	{
		[SetUp]
		public void SetUp()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.deploymentRepository = new DeploymentRepository(this.connectionFactory);
		}

		private IConnectionFactory connectionFactory;
		private IDeploymentRepository deploymentRepository;

		[TestCase("")]
		[TestCase(null)]
		public void ReadMdfLdfDirectories(string targetServer)
		{
			// Act
			var directories = this.deploymentRepository.ReadMdfLdfDirectories(targetServer);

			// Assert
			Assert.That(directories.MdfPath, Is.Not.Null);
			Assert.That(directories.LdfPath, Is.Not.Null);
		}

		[Test]
		public void ReadCollationSettings()
		{
			// Act
			var collation = this.deploymentRepository.ReadCollationSettings();

			// Assert
			Assert.That(collation, Is.EqualTo("SQL_Latin1_General_CP1_CI_AS"));
		}

		[Test]
		public void RunSqlScripts_Success()
		{
			var settings = TestUtilities.GetDeploymentSettings();
			var resultSet = new MigrationResultSet(true, new List<LogMessage>());
			var sprocText = "select 1";
			var path = Path.GetTempPath();
			path = Path.Combine(path, Guid.NewGuid().ToString());
			Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, "test_sproc.sql"), sprocText);

			//create repository
			this.deploymentRepository.RunSqlScripts(settings, path, resultSet);

			//Assert
			Assert.That(resultSet.Messages.Count, Is.EqualTo(1));
		}

		[Test]
		public void RunSqlScripts_ConnectionError()
		{
			var settings = TestUtilities.GetDeploymentSettings();
			settings.Server = "SomeServerThatDoesntExist";
			var resultSet = new MigrationResultSet(true, new List<LogMessage>());
			var sprocText = "select 1";
			var path = Path.GetTempFileName();
			File.WriteAllText(path, sprocText);

			// Act
			this.deploymentRepository.RunSqlScripts(settings, path, resultSet);

			// Assert
			Assert.That(resultSet.Messages.Count, Is.EqualTo(0));
		}

		[Test]
		public void RemoveOldApplicationReferencesFromWorkspace_Success()
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;

			// Act
			Guids.Agent.AgentGuidsToRemove.ForEach(guid => this.deploymentRepository.RemoveOldApplicationReferencesFromWorkspace(guid, workspaceId));

			// Assert
			Assert.Pass();
		}

		[Test]
		public void RemoveOldApplicationReferences_Success()
		{
			// Act
			this.deploymentRepository.RemoveOldApplicationReferences();

			// Assert
			Assert.Pass();
		}

		[Test]
		public void RunCreateDatabaseScripts()
		{
			// Arrange
			var settings = new DeploymentSettings { Server = Config.Server, DatabaseName = Names.Database.EddsPerformance };
			var resultSet = new MigrationResultSet(true, new List<LogMessage>());
			var sprocText = "select 1";
			var path = Path.GetTempPath();
			path = Path.Combine(path, Guid.NewGuid().ToString());
			Directory.CreateDirectory(path);
			var file = Path.Combine(path, "test_sproc.sql");
			File.WriteAllText(file, sprocText);

			// Act
			this.deploymentRepository.RunCreateDatabaseScripts(settings, file, resultSet);

			// Assert
			Assert.That(resultSet.Success, Is.True);
			Assert.That(resultSet.Messages.Count, Is.EqualTo(5));
		}
	}
}

