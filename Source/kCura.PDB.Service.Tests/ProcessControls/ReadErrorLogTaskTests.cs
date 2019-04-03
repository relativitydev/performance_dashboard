namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ReadErrorLogTaskTests
	{
		[SetUp]
		public void Setup()
		{
			_sqlRepo = new Mock<ISqlServerRepository>();
			_logger = new Mock<ILogger>();
		}

		private Mock<ISqlServerRepository> _sqlRepo;
		private Mock<ILogger> _logger;

		[Test]
		public void ReadErrorLogTask_Execute()
		{
			//Arrange
			_sqlRepo.Setup(r => r.AdminScriptsInstalled()).Returns(true);
			_sqlRepo.Setup(r => r.SummarizeSqlServerPageouts());

			//Act
			var tsk = new ReadErrorLogTask(_logger.Object, _sqlRepo.Object, 0);
			var result = tsk.Execute(new ProcessControl());

			//Assert
			Assert.That(result, Is.True);
			_sqlRepo.Verify(r => r.SummarizeSqlServerPageouts(), Times.Once);
		}

		[Test]
		public void ReadErrorLogTask_AdminScriptsNotInstalled()
		{
			//Arrange
			_sqlRepo.Setup(r => r.AdminScriptsInstalled()).Returns(false);
			_logger.Setup(l => l.LogWarning(It.Is<String>(s => s.Contains("Installation of Performance Dashboard is incomplete")), It.IsAny<String>()));

			//Act
			var task = new ReadErrorLogTask(_logger.Object, _sqlRepo.Object, 123);
			var result = task.Execute(new ProcessControl());

			//Assert
			Assert.That(result, Is.True);
			_sqlRepo.VerifyAll();

		}
	}
}
