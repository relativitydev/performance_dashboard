namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class FileLevelLatencyTaskTests
	{
		[SetUp]
		public void Setup()
		{
			_sqlRepo = new Mock<ISqlServerRepository>();
			_logger = new Mock<ILogger>();
		}

		private Mock<ISqlServerRepository> _sqlRepo;
		private Mock<ILogger> _logger;

		[Test, Category("Unit"), Category("Ignore"), Ignore("TODO fix connection string issue")]
		public void FileLevelLatencyTask_Execute()
		{
			//Arrange
			var eddsPerformanceServer = "Prim1";
			var registeredSqlServers = new[] { "abc1" };
			var sqlServers = new[] { "abc1", "abc2" };
			_logger.Setup(l => l.LogVerbose(It.IsAny<String>(), It.IsAny<String>())).Callback<String, String>((m, c) => Console.WriteLine(String.Format("{0} - {1}", m, c)));
			_logger.Setup(l => l.LogError(It.IsAny<String>(), It.IsAny<String>())).Callback<String, String>((m, c) => Console.WriteLine(String.Format("{0} - {1}", m, c)));
			_sqlRepo.Setup(r => r.GetRegisteredSQLServers()).Returns(registeredSqlServers.Select(s => new ServerInfo { Name = s }).ToArray());
			_sqlRepo.Setup(m => m.PrimarySqlServerRepository.GetPrimarySqlServer()).Returns(new ResourceServer { Name = eddsPerformanceServer });
			_sqlRepo.Setup(r => r.FileLatencyRepository.ExecuteSaveFileLevelLatencyDetails(It.IsIn(sqlServers), eddsPerformanceServer));

			//Act
			var tsk = new FileLevelLatencyTask(_logger.Object, _sqlRepo.Object, 0);
			var result = tsk.Execute(new ProcessControl());

			//Assert
			Assert.That(result, Is.True);
			_sqlRepo.Verify(r => r.FileLatencyRepository.ExecuteSaveFileLevelLatencyDetails(It.IsIn(sqlServers), eddsPerformanceServer), Times.Once);
		}
	}
}
