namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.ScriptInstallation;
	using kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class EnvironmentCheckSqlConfigTaskTests
	{
		[SetUp]
		public void Setup()
		{
			_sqlRepo = new Mock<ISqlServerRepository>();
			_logger = TestUtilities.GetMockLogger();
			_data = new DataTable();
			_data.Columns.Add("Status", typeof(string));
			_data.Columns.Add("Scope", typeof(string));
			_data.Columns.Add("Section", typeof(string));
			_data.Columns.Add("Name", typeof(string));
			_data.Columns.Add("Description", typeof(string));
			_data.Columns.Add("Value", typeof(string));
			_data.Columns.Add("Recommendation", typeof(string));
			_data.Columns.Add("Severity", typeof(int));
			var row = _data.Rows.Add();
			row["Status"] = "abc";
			row["Scope"] = "abc";
			row["Section"] = "abc";
			row["Name"] = "abc";
			row["Description"] = "abc";
			row["Value"] = "abc";
			row["Recommendation"] = "abc";
			row["Severity"] = 1;
		}

		private Mock<ISqlServerRepository> _sqlRepo;
		private Mock<ILogger> _logger;
		private DataTable _data;

		[Test, Category("Integration")]
		public void EnvironmentCheckSqlConfig_Execute()
		{
			//Arrange
			var serverName = "localhost";
			var server = new Server { ServerName = serverName, ServerTypeId = 3 };
			var databaseDirs = new DatabaseDirectoryInfo { };
			_sqlRepo.Setup(r => r.AdminScriptsInstalled()).Returns(true);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ExecuteTuningForkSystem(server.ServerName)).Returns(_data);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.SaveTuningForkSystemData(server.ServerName, _data));
			_sqlRepo.Setup(r => r.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableInstantFileInitializationCheck)).Returns("true");
			_sqlRepo.Setup(r => r.DeploymentRepository.ReadMdfLdfDirectories(server.ServerName)).Returns(databaseDirs);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ReadCheckIFISettings(databaseDirs)).Returns(true);
			_sqlRepo.Setup(r => r.PerformanceServerRepository.ReadAllActiveAsync())
				.ReturnsAsync(new List<Server>() { server });

			//Act
			var task = new EnvironmentCheckSqlConfigTask(_logger.Object, _sqlRepo.Object, 123);
			var result = task.Execute(new ProcessControl());

			//Assert
			Assert.That(result, Is.True);
			_sqlRepo.VerifyAll();
			Assert.That(_data.Rows.Count, Is.EqualTo(2));
		}

		[Test, Category("Integration")]
		public void ExecuteTuningForkForServer_Int_Error()
		{
			//Arrange
			var serverName = "localHost";
			var server = new Server { ServerName = serverName, ServerTypeId = 3 };
			var processControl = new ProcessControl();
			_sqlRepo.Setup(r => r.AdminScriptsInstalled()).Returns(true);
			_sqlRepo.Setup(
				r =>
					r.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section,
						ConfigurationKeys.EnableInstantFileInitializationCheck)).Returns("true");
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ExecuteTuningForkSystem(server.ServerName)).Throws(new Exception("abc"));
			_sqlRepo.Setup(r => r.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.EnableInstantFileInitializationCheck)).Returns("false");
			_logger.Setup(l => l.LogError(It.Is<String>(m => m.Contains("Failure Running tunning for")), "Environment Check SQL Configuration"));
			_sqlRepo.Setup(r => r.PerformanceServerRepository.ReadAllActiveAsync())
				.ReturnsAsync(new[] { server });

			//Act
			var task = new EnvironmentCheckSqlConfigTask(_logger.Object, _sqlRepo.Object, 123);
			var result = task.Execute(processControl);

			//Assert
			Assert.That(result, Is.False);
			_sqlRepo.VerifyAll();
			Assert.That(processControl.LastErrorMessage, Is.StringContaining("abc"));
			Assert.That(_data.Rows.Count, Is.EqualTo(1));
		}

		[Test, Category("Unit")]
		public void ExecuteTuningForkForServer()
		{
			//Arrange
			var serverName = "abc";
			var server = new Server { ServerName = serverName };
			var databaseDirs = new DatabaseDirectoryInfo { };
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ExecuteTuningForkSystem(server.ServerName)).Returns(_data);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.SaveTuningForkSystemData(server.ServerName, _data));

			//Act
			var task = new EnvironmentCheckSqlConfigTask(_logger.Object, _sqlRepo.Object, 123);
			task.ExecuteTuningForkForServer(server, false);

			//Assert
			_sqlRepo.VerifyAll();
			Assert.That(_data.Rows.Count, Is.EqualTo(1));
		}

		[Test, Category("Unit")]
		public void ExecuteTuningForkForServer_GetIFISettings()
		{
			//Arrange
			var server = new Server { ServerName = "abc" };
			var databaseDirs = new DatabaseDirectoryInfo { };
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ExecuteTuningForkSystem(server.ServerName)).Returns(_data);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.SaveTuningForkSystemData(server.ServerName, _data));
			_sqlRepo.Setup(r => r.DeploymentRepository.ReadMdfLdfDirectories(server.ServerName)).Returns(databaseDirs);
			_sqlRepo.Setup(r => r.EnvironmentCheckRepository.ReadCheckIFISettings(databaseDirs)).Returns(true);

			//Act
			var task = new EnvironmentCheckSqlConfigTask(_logger.Object, _sqlRepo.Object, 123);
			task.ExecuteTuningForkForServer(server, true);

			//Assert
			_sqlRepo.VerifyAll();
			Assert.That(_data.Rows.Count, Is.EqualTo(2));
		}
	}
}
