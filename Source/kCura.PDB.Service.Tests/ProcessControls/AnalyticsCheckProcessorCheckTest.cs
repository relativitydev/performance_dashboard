namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class AnalyticsCheckProcessorCheckTest
	{
		[SetUp]
		public void Setup()
		{
			_wmiHelper = new Mock<IWMIHelper>();
			_sqlRepo = new Mock<ISqlServerRepository>();
		}

		private Mock<IWMIHelper> _wmiHelper;
		private Mock<ISqlServerRepository> _sqlRepo;

		[Test]
		public void AnalyticsCheckProcessorCheck_ProcessControlID()
		{
			//Act
			var task = new AnalyticsCheckProcessorCheck(_wmiHelper.Object, _sqlRepo.Object, 1);
			var result = task.ProcessControlID;

			//Assert
			Assert.That(result, Is.EqualTo(ProcessControlId.EnvironmentCheckServerInfo));
		}


		[Test, Category("Ignore"), Ignore("TODO fix the connection string issues")]
		public void SaveServerRecommendation_Good()
		{
			//Assert
			var server = new Server() { ServerTypeId = (int)ServerType.Analytics };
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(
				server, Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultGood, "8"));

			//Act
			var task = new AnalyticsCheckProcessorCheck(_wmiHelper.Object, _sqlRepo.Object, 1);
			task.SaveServerRecommendation(server, 8, 8);

			//Assert
			_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(
				server, Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultGood, "8"), Times.Once());
		}

		[Test, Category("Ignore"), Ignore("TODO fix the connection string issues")]
		public void SaveServerRecommendation_Warning()
		{
			//Assert
			var server = new Server() { ServerTypeId = (int)ServerType.Analytics };
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(
				server, Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultWarning, "4"));

			//Act
			var task = new AnalyticsCheckProcessorCheck(_wmiHelper.Object, _sqlRepo.Object, 1);
			task.SaveServerRecommendation(server, 4, 8);

			//Assert
			_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(
				server, Guids.EnvironmentCheck.ContentAnalystMaxConnectorsPerIndexDefaultWarning, "4"), Times.Once());
		}

		[Test]
		public void GetServerCores()
		{
			//Arrange
			var server = new Server();
			var diagnostics = new List<KeyValuePair<string, string>>();
			diagnostics.Add(new KeyValuePair<string, string>(ManagementField.NumberOfCores.ToString(), "8"));
			_wmiHelper.Setup(w => w.CreateDiagnostics(
				server,
				ManagementField.Name | ManagementField.NumberOfCores | ManagementField.NumberOfLogicalProcessors,
				"Win32_Processor",
				It.IsAny<String>()))
				.Returns(diagnostics);

			//Act
			var task = new AnalyticsCheckProcessorCheck(_wmiHelper.Object, _sqlRepo.Object, 1);
			var result = task.GetServerCores(server);

			//Assert
			Assert.That(result, Is.EqualTo(8));
		}

		[Test]
		public void GetConfiguredConnectors()
		{
			//Arrange
			_sqlRepo.Setup(s => s.ConfigurationRepository.ReadConfigurationValue("Relativity.Core", "ContentAnalystMaxConnectorsPerIndexDefault")).Returns("8");

			//Act
			var task = new AnalyticsCheckProcessorCheck(_wmiHelper.Object, _sqlRepo.Object, 1);
			var result = task.GetConfiguredConnectors();

			//Assert
			Assert.That(result, Is.EqualTo(8));
		}
	}
}
