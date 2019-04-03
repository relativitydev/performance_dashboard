namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck;
	using kCura.PDB.Service.Services;
	using kCura.Relativity.Client.DTOs;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class AnalyticsCheckMemoryCheckTests
	{
		[SetUp]
		public void Setup()
		{
			_wmiHelper = new Mock<IWMIHelper>();
			_sqlRepo = new Mock<ISqlServerRepository>();
			_workspaceRepo = new Mock<IWorkspaceRepository>();
			_logger = new Mock<ILogger>();
		}

		private Mock<IWMIHelper> _wmiHelper;
		private Mock<ISqlServerRepository> _sqlRepo;
		private Mock<IWorkspaceRepository> _workspaceRepo;
		private Mock<ILogger> _logger;

		[Test, Category("Unit"), Explicit("TODO: Investigate why this would take longer than 1 second.")]
		public void AnalyticsCheckMemoryCheck_ProcessControlID()
		{
			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			var result = task.ProcessControlID;

			//Assert
			Assert.That(result, Is.EqualTo(ProcessControlId.EnvironmentCheckServerInfo));
		}

		[Test, Category("Unit")]
		public void GetServerMemory()
		{
			//Arrange
			var server = new Server();
			var diagnostics = new List<KeyValuePair<string, string>>();
			diagnostics.Add(new KeyValuePair<string, string>(ManagementField.TotalVisibleMemorySize.ToString(), "8388608"));
			_wmiHelper.Setup(w => w.CreateDiagnostics(
				server,
				ManagementField.TotalVisibleMemorySize,
				"Win32_OperatingSystem",
				It.IsAny<String>()))
				.Returns(diagnostics);

			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			var result = task.GetServerMemory(server);

			//Assert
			Assert.That(result, Is.EqualTo(8192));
		}

		[Test, Category("Unit")]
		public void GetServerMemory_Error()
		{
			//Arrange
			var server = new Server();
			var diagnostics = new List<KeyValuePair<string, string>>();
			_wmiHelper.Setup(w => w.CreateDiagnostics(
				server,
				ManagementField.TotalVisibleMemorySize,
				"Win32_OperatingSystem",
				It.IsAny<String>()))
				.Returns(diagnostics);

			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			Assert.Throws<Exception>(() => task.GetServerMemory(server));
		}

		[Test, Category("Integration")]
		public void GetServerMemory_Int()
		{
			//Arrange
			var server = new Server() { ServerName = "localhost", ServerIpAddress = "127.0.0.1" };
			var logger = new Mock<ILogger>();
			var wmiHelper = new WMIHelper(logger.Object);

			//Act
			var task = new AnalyticsCheckMemoryCheck(wmiHelper, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			var result = task.GetServerMemory(server);

			//Assert
		}

		[Test, Category("Unit"), Explicit("TODO -- Fix after connection string refactor")]
		public void GetServerDocuments_Success()
		{
			//Arrange
			var serverId = 444;
			var server = new Server() { ArtifactId = serverId };
			var workspaceIds = new[] { 123, 234 };
			var workspaces = workspaceIds.Select(wid => new Workspace(wid) { ServerID = serverId }).ToList();

			var caatPopTables = new List<String>() { "zca_pop_1_1", "zca_pop_2_1", "zca_pop_3_1" };
			var caatIndexes = new List<Int32>() { 1, 2, 3 };
			_sqlRepo.Setup(s => s.AnalyticsRepository.ReadCaatPopTables(It.IsIn(workspaceIds)))
				.Returns(caatPopTables);
			_sqlRepo.Setup(s => s.AnalyticsRepository.ReadCaatIndexes(It.IsIn(workspaceIds)))
				.Returns(caatIndexes);
			_sqlRepo.Setup(s => s.AnalyticsRepository.ReadCaatSearchableDocuments(It.IsIn(workspaceIds), caatPopTables, caatIndexes))
				.Returns(new Dictionary<int, long>() { { 555, 999 }, { 777, 10 }, { 333, 6 } });
			_sqlRepo.Setup(s => s.AnalyticsRepository.ReadCaatTrainingDocuments(It.IsIn(workspaceIds), caatPopTables, caatIndexes))
				.Returns(new Dictionary<int, long>() { { 555, 888 }, { 777, 15 }, { 999, 5 } });


			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			var result = task.GetServerIndexedDocuments(server, workspaces);

			//Assert
			Assert.That(result.Keys.Count, Is.EqualTo(4));
			Assert.That(result[333].Searchable, Is.EqualTo(12));
			Assert.That(result[555].Searchable, Is.EqualTo(1998));
			Assert.That(result[777].Searchable, Is.EqualTo(20));
			Assert.That(result[999].Searchable, Is.EqualTo(0));
			Assert.That(result[333].Training, Is.EqualTo(0));
			Assert.That(result[555].Training, Is.EqualTo(1776));
			Assert.That(result[777].Training, Is.EqualTo(30));
			Assert.That(result[999].Training, Is.EqualTo(10));
		}

		[Test, Category("Unit")]
		public void GetServerDocuments_LogError()
		{
			//Arrange
			var serverId = 444;
			var server = new Server() { ArtifactId = serverId };
			var workspaceIds = new[] { 123, 234 };
			var workspaces = workspaceIds.Select(wid => new Workspace(wid) { ServerID = serverId, Name = $"AbcQ{wid}" }).ToList();

			//Arrange
			_sqlRepo.Setup(s => s.AnalyticsRepository.ReadCaatPopTables(It.IsIn(workspaceIds)))
				.Throws(new Exception("Boom"));
			_logger.Setup(l => l.LogError(It.IsAny<String>(), It.IsAny<String>()));

			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			var result = task.GetServerIndexedDocuments(server, workspaces);

			//Assert
			_logger.Verify(l => l.LogError(It.IsAny<String>(), It.IsAny<String>()));
		}


		[Test, Category("Unit")]
		[TestCase(1, 999999, true)]// less than 1 mill docs are ignored
		[TestCase(6144, 1000000, true)]//6gb for 1 mill docs
		[TestCase(6144, 1000001, false)]//6gb for +1 mill docs
		[TestCase(131941396, 21474836471, true)]//6gb for more than Max Int docs
												//[TestCase(0, 9999999, false)]//6gb for more than Max Int docs
		[TestCase(6144, 0, true)]
		public void SaveServerRecommendation(Int32 serverMemory, long documents, Boolean expectedGoodRec)
		{
			//Arrange
			var server = new Server() { ArtifactId = 444 };
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
						Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultGood, It.IsAny<String>()));
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
						Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultWarning, It.IsAny<String>()));
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
						Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultGood, It.IsAny<String>()));
			_sqlRepo.Setup(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
						Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultWarning, It.IsAny<String>()));
			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			task.SaveServerRecommendation(server, serverMemory, documents, documents);

			//Assert
			if (expectedGoodRec)
			{
				_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
					Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultGood, It.IsAny<String>())
					, "Searchable should have been good");
				_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
					Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultGood, It.IsAny<String>())
					, "Training should have been good");
			}
			else
			{
				_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
					Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultWarning, It.IsAny<String>())
					, "Searchable should have been warning");
				_sqlRepo.Verify(s => s.AnalyticsRepository.SaveAnalyticsRecommendation(server,
					Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultWarning, It.IsAny<String>())
					, "Training should have been warning");
			}
		}

		[Test, Category("Unit")]
		public void SaveServerRecommendation_ZeroServerMemory()
		{
			//Arrange
			var server = new Server() { ArtifactId = 444 };
			//Act
			var task = new AnalyticsCheckMemoryCheck(_wmiHelper.Object, _workspaceRepo.Object, _logger.Object, _sqlRepo.Object, 1);
			Assert.Throws<ArgumentException>(() => task.SaveServerRecommendation(server, 0, 1000000, 1000000));

			//Assert
		}
	}
}

