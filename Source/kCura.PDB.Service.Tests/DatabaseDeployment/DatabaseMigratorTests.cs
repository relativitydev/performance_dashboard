namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class DatabaseMigratorTests
	{
		[SetUp]
		public void Setup()
		{
			_fileService = new Mock<IRoundHouseFileService>();
			_scriptTokenService = new Mock<ISqlScriptTokenService>();
			_roundhouseManager = new Mock<IRoundhouseManager>();
			_deploymentRepo = new Mock<IDeploymentRepository>();
			_availabilityGroupRepo = new Mock<IAvailabilityGroupRepository>();
			this.connectionFactoryMock = new Mock<IConnectionFactory>();
			this.timeService = TestUtilities.GetTimeService(60);

			var sprocText = "select 1";
			path = Path.GetTempPath();
			path = Path.Combine(path, $"{Guid.NewGuid()}_ok_to_delete");
			Directory.CreateDirectory(path);
			Directory.CreateDirectory(Path.Combine(path, DeploymentDirectoryStructureConstants.SprocsFolderName));
			Directory.CreateDirectory(Path.Combine(path, DeploymentDirectoryStructureConstants.FunctionsFolderName));
			File.WriteAllText(Path.Combine(path, "test_script.sql"), sprocText);
			_fileService.Setup(fs => fs.UnzipResourceFile(It.IsAny<Byte[]>())).Returns(path);

			settings = new DeploymentSettings()
			{
				CreateScriptName = "create script",
				DatabaseName = "EDDS",
				Server = "server",
			};

			this.databaseMigrator = new DatabaseMigrator(
				settings, 
				this.connectionFactoryMock.Object, 
				_scriptTokenService.Object,
				_roundhouseManager.Object, 
				_deploymentRepo.Object,
				_availabilityGroupRepo.Object,
				_fileService.Object,
				this.timeService);

			var connectionString = DatabaseConstants.TestConnectionString;
			this.connectionMock = new Mock<IDbConnection>();
			this.connectionMock.Setup(m => m.ConnectionString).Returns(connectionString);
			this.connectionFactoryMock.Setup(m => m.GetTargetConnection(settings.DatabaseName, settings.Server, null))
				.Returns(this.connectionMock.Object);
		}

		private Mock<IRoundHouseFileService> _fileService;
		private Mock<ISqlScriptTokenService> _scriptTokenService;
		private Mock<IRoundhouseManager> _roundhouseManager;
		private Mock<IDeploymentRepository> _deploymentRepo;
		private Mock<IAvailabilityGroupRepository> _availabilityGroupRepo;
		private Mock<IConnectionFactory> connectionFactoryMock;
		private Mock<IDbConnection> connectionMock;
		private ITimeService timeService;
		private DeploymentSettings settings;
		private string path;

		private DatabaseMigrator databaseMigrator;

		[Test]
		public void Deploy()
		{
			//Arrange
			_scriptTokenService.Setup(sts => sts.Replace(It.IsAny<IEnumerable<string>>()));
			_roundhouseManager.Setup(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(new MigrationResultSet(true, new List<LogMessage>()));

			//Act
			var results = this.databaseMigrator.Deploy();

			//Assert
			Assert.That(results.Success, Is.True);
		}

		[Test]
		public void ReDeployScripts_Success()
		{
			//Arrange
			_deploymentRepo.Setup(dr => dr.RunSqlScripts(settings, It.IsAny<String>(), It.IsAny<MigrationResultSet>()));
			_scriptTokenService.Setup(sts => sts.Replace(It.IsAny<IEnumerable<string>>()));

			//Act
			var results = this.databaseMigrator.ReDeployScripts();

			//Assert
			Assert.That(results.Success, Is.True);
			_deploymentRepo.Verify(dr => dr.RunSqlScripts(settings, It.IsAny<String>(), It.IsAny<MigrationResultSet>()), Times.Exactly(2));
		}

		[Test]
		public void ReDeployScripts_Error()
		{
			//Arrange
			_deploymentRepo.Setup(dr => dr.RunSqlScripts(settings, It.IsAny<String>(), It.IsAny<MigrationResultSet>()))
				.Throws<Exception>();
			_scriptTokenService.Setup(sts => sts.Replace(It.IsAny<IEnumerable<string>>()));

			//Act
			var results = this.databaseMigrator.ReDeployScripts();

			//Assert
			Assert.That(results.Success, Is.False);
		}

		[Test]
		public void RetryDeployment()
		{
			//Arrange
			var firstAttemptResults = new MigrationResultSet(false, new List<LogMessage>() { new LogMessage(LogSeverity.Info, "msg 1") });
			var secondAttemptResults = new MigrationResultSet(true, new List<LogMessage>() { new LogMessage(LogSeverity.Info, "msg 2") });
			var expectedMessageCount = firstAttemptResults.Messages.Count + secondAttemptResults.Messages.Count + 1;
			_roundhouseManager.Setup(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(secondAttemptResults);

			//Act
			var results = this.databaseMigrator.RetryDeployment(path, firstAttemptResults);

			//Assert
			Assert.That(results.Success, Is.True);
			Assert.That(results.Messages.Count, Is.GreaterThanOrEqualTo(expectedMessageCount), "final merged message count must be greater than or equal to both first and second attempt and any additional messages logged when re-trying");
		}

		[Test]
		public void Execute_Success()
		{
			//Arrange
			var migrationResults = new MigrationResultSet(true, new List<LogMessage>() { new LogMessage(LogSeverity.Info, "msg 1") });

			_roundhouseManager.Setup(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(migrationResults);

			//Act
			var results = this.databaseMigrator.Execute(path, false);

			//Assert
			Assert.That(results.Success, Is.True);
			Assert.That(results.Messages, Is.EquivalentTo(migrationResults.Messages));
		}

		[Test]
		public void Execute_Success_LogMuting()
		{
			//Arrange
			var migrationResults = new MigrationResultSet(true, new List<LogMessage>()
			{
				new LogMessage(LogSeverity.Info, "msg 1"),
				new LogMessage(LogSeverity.Warning, "msg 2"),
				new LogMessage(LogSeverity.Debug, "msg 3")
			});

			var migrationResultsMuted = new MigrationResultSet(true, new List<LogMessage>())
			{
				Messages = migrationResults.Messages.Select(m =>
				{
					m.Message = $"[{m.Severity}] -- {m.Message}";
					m.Severity = LogSeverity.Debug;
					return m;
				}).ToList()
			};


			_roundhouseManager.Setup(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(migrationResults);

			//Act
			var results = this.databaseMigrator.Execute(path, false);

			//Assert
			Assert.That(results.Success, Is.True);
			Assert.That(results.Messages, Is.EquivalentTo(migrationResultsMuted.Messages));
		}

		[Test]
		public void Execute_Failure_LogMuting()
		{
			//Arrange
			var migrationResults = new MigrationResultSet(false, new List<LogMessage>()
			{
				new LogMessage(LogSeverity.Info, "msg 1"),
				new LogMessage(LogSeverity.Warning, "msg 2"),
				new LogMessage(LogSeverity.Debug, "msg 3"),
				new LogMessage(LogSeverity.Error, "msg 4")
			});


			_roundhouseManager.Setup(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(migrationResults);

			//Act
			var results = this.databaseMigrator.Execute(path, false);

			//Assert
			Assert.That(results.Success, Is.False);
			Assert.That(results.Messages, Is.EquivalentTo(migrationResults.Messages));
		}

		[Test]
		public void Execute_Retry_AoagIssue()
		{
			//Arrange
			var firstAttemptResults = new MigrationResultSet(false, new List<LogMessage>() { new LogMessage(LogSeverity.Error, "SqlException: involved in a database mirroring session or an availability group") });
			var secondAttemptResults = new MigrationResultSet(true,
				new List<LogMessage>() { new LogMessage(LogSeverity.Info, "msg 2") });
			var expectedMessageCount = firstAttemptResults.Messages.Count + secondAttemptResults.Messages.Count + 1;
			_availabilityGroupRepo.Setup(r => r.RemoveFromAvailabilityGroup("EDDS")).Returns(true);
			_roundhouseManager.SetupSequence(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(firstAttemptResults)
				.Returns(secondAttemptResults);

			//Act
			var results = this.databaseMigrator.Execute(path, false);

			//Assert
			Assert.That(results.Success, Is.True);
			Assert.That(results.Messages.Count, Is.GreaterThanOrEqualTo(expectedMessageCount), "final merged message count must be greater than or equal to both first and second attempt and any additional messages logged when re-trying");
			_availabilityGroupRepo.Verify(r => r.RemoveFromAvailabilityGroup("EDDS"), Times.Once);
		}

		[Test]
		public void Execute_Retry_LoginIssue()
		{
			//Arrange
			var firstAttemptResults = new MigrationResultSet(false, new List<LogMessage>() { new LogMessage(LogSeverity.Error, @"SqlException: Cannot open database ""EDDSPerformance"" requested by the login. The login failed.") });
			var secondAttemptResults = new MigrationResultSet(true,
				new List<LogMessage>() { new LogMessage(LogSeverity.Info, "msg 2") });
			var expectedMessageCount = firstAttemptResults.Messages.Count + secondAttemptResults.Messages.Count + 1;
			_roundhouseManager.SetupSequence(
				rm => rm.Run(It.IsAny<string>(), path, "create script", "EDDS", "server", It.IsAny<int>()))
				.Returns(firstAttemptResults)
				.Returns(secondAttemptResults);

			//Act
			var results = this.databaseMigrator.Execute(path, false);

			//Assert
			Assert.That(results.Success, Is.True);
			Assert.That(results.Messages.Count, Is.GreaterThanOrEqualTo(expectedMessageCount), "final merged message count must be greater than or equal to both first and second attempt and any additional messages logged when re-trying");
		}

		[Test]
		[TestCase("100", 100)]
		[TestCase("0", 0)]
		[TestCase("", 3600)]
		[TestCase(null, 3600)]
		[TestCase("asdf", 3600)]
		[TestCase("-1", 3600)]
		[TestCase("3700", 3700)]
		public void ReadRoundhouseTimeoutValue(string configValue, int expectedResult)
		{
			// Arrange
			var configRepoMock = new Mock<IConfigurationRepository>();
			configRepoMock.Setup(m => m.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.RoundhouseTimeoutSeconds)).Returns(configValue);
			var config = DatabaseMigrator.ReadRoundhouseTimeoutValue(configRepoMock.Object);

			Assert.That(config, Is.EqualTo(expectedResult));
		}
	}
}
