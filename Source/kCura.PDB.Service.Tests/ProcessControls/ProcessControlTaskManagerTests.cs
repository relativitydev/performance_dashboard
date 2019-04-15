namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ProcessControlTaskManagerTests
	{
		[SetUp]
		public void Setup()
		{
			_sqlRepo = new Mock<ISqlServerRepository>();
			_pcRepo = new Mock<IProcessControlRepository>();
			_logger = new Mock<ILogger>();
		}

		private Mock<ISqlServerRepository> _sqlRepo;
		private Mock<IProcessControlRepository> _pcRepo;
		private Mock<ILogger> _logger;

		[Test]
		public void RunTasks_RunMultiple()
		{
			var pc1 = new ProcessControl { Id = ProcessControlId.CollectWaitStatistics, Frequency = 60, LastProcessExecDateTime = DateTime.UtcNow.AddYears(-1), };
			var pc2 = new ProcessControl { Id = ProcessControlId.CycleErrorLog, Frequency = 60, LastProcessExecDateTime = DateTime.UtcNow.AddYears(-1), };
			var processControls = new List<ProcessControl>() { pc1, pc2 };
			_pcRepo.Setup(r => r.ReadAll()).Returns(processControls);
			_sqlRepo.Setup(r => r.ReadServerUtcTime()).Returns(DateTime.UtcNow);

			var taskA = new Mock<IProcessControlTask>();
			var taskB = new Mock<IProcessControlTask>();
			taskA.Setup(t => t.Execute(It.IsAny<ProcessControl>()));
			taskB.Setup(t => t.Execute(It.IsAny<ProcessControl>()));
			taskA.SetupGet(t => t.ProcessControlID).Returns(ProcessControlId.CollectWaitStatistics);
			taskB.SetupGet(t => t.ProcessControlID).Returns(ProcessControlId.CycleErrorLog);
			_sqlRepo.SetupGet(r => r.ProcessControlRepository).Returns(_pcRepo.Object);

			//Act
			var manager = new ProcessControlTaskManager(_sqlRepo.Object, _logger.Object);
			manager.Subscribe(taskA.Object, taskB.Object);
			manager.Execute();

			//Assert
			taskA.Verify(t => t.Execute(It.IsAny<ProcessControl>()), Times.Once);
			taskB.Verify(t => t.Execute(It.IsAny<ProcessControl>()), Times.Once);
		}

		[Test]
		public void RunTasks_RunErrors()
		{
			var pc1 = new ProcessControl { Id = ProcessControlId.EnvironmentCheckSqlConfig, Frequency = 60, LastProcessExecDateTime = DateTime.UtcNow.AddYears(-1), LastExecSucceeded = true };
			var processControls = new List<ProcessControl>() { pc1 };
			_pcRepo.Setup(r => r.ReadAll()).Returns(processControls);
			_sqlRepo.Setup(r => r.ReadServerUtcTime()).Returns(DateTime.UtcNow);
			_sqlRepo.SetupGet(r => r.ProcessControlRepository).Returns(_pcRepo.Object);

			var taskE = new Mock<IProcessControlTask>();
			taskE.Setup(t => t.Execute(It.IsAny<ProcessControl>())).Throws(new Exception("Error running task E"));
			taskE.SetupGet(t => t.ProcessControlID).Returns(ProcessControlId.EnvironmentCheckSqlConfig);

			var manager = new ProcessControlTaskManager(_sqlRepo.Object, _logger.Object);
			manager.Subscribe(taskE.Object);
			manager.Execute();

			taskE.Verify(t => t.Execute(It.IsAny<ProcessControl>()), Times.Once);
			Assert.That(pc1.LastExecSucceeded, Is.False);
			Assert.That(pc1.LastErrorMessage, Is.Not.Empty);
		}

		[Test]
		public void RunTasks_RunSkipped()
		{
			var pc3 = new ProcessControl { Id = ProcessControlId.ReadErrorLog, Frequency = 60, LastProcessExecDateTime = DateTime.UtcNow, };
			var processControls = new List<ProcessControl>() { pc3 };
			_pcRepo.Setup(r => r.ReadAll()).Returns(processControls);
			_sqlRepo.Setup(r => r.ReadServerUtcTime()).Returns(DateTime.UtcNow);
			_sqlRepo.SetupGet(r => r.ProcessControlRepository).Returns(_pcRepo.Object);

			var taskD = new Mock<IProcessControlTask>();
			taskD.Setup(t => t.Execute(It.IsAny<ProcessControl>()));
			taskD.SetupGet(t => t.ProcessControlID).Returns(ProcessControlId.ReadErrorLog);

			//Act
			var manager = new ProcessControlTaskManager(_sqlRepo.Object, _logger.Object);
			manager.Subscribe(taskD.Object);
			manager.Execute();

			//Assert
			taskD.Verify(t => t.Execute(It.IsAny<ProcessControl>()), Times.Never);
		}

		[Test]
		public void RunTasks_NoObservers()
		{
			var originalDate = new DateTime(1900, 1, 1);
			var pc3 = new ProcessControl { Id = ProcessControlId.ReadErrorLog, Frequency = 60, LastProcessExecDateTime = originalDate, LastExecSucceeded = true };
			var pc4 = new ProcessControl { Id = ProcessControlId.EnvironmentCheckSqlConfig, Frequency = 60, LastProcessExecDateTime = originalDate, LastExecSucceeded = false };
			var processControls = new List<ProcessControl>() { pc3, pc4 };
			_pcRepo.Setup(r => r.ReadAll()).Returns(processControls);
			_sqlRepo.Setup(r => r.ReadServerUtcTime()).Returns(DateTime.UtcNow);
			_sqlRepo.SetupGet(r => r.ProcessControlRepository).Returns(_pcRepo.Object);

			//Act
			var manager = new ProcessControlTaskManager(_sqlRepo.Object, _logger.Object);
			manager.Execute();

			//Assert
			Assert.That(pc3.LastProcessExecDateTime.Year, Is.EqualTo(originalDate.Year));
			Assert.That(pc4.LastProcessExecDateTime.Year, Is.EqualTo(originalDate.Year));
			Assert.That(pc3.LastExecSucceeded, Is.True);
			Assert.That(pc4.LastExecSucceeded, Is.False);
		}

		[Test]
		[TestCase(5, false)]
		[TestCase(60, true)]
		[TestCase(120, true)]
		public void GetNormilizedLastExecDate(Int32 freq, Boolean expectsToBeNormilized)
		{
			//Act
			var result = ProcessControlTaskManager.GetLastExecDate(new ProcessControl() { Frequency = freq });

			//Assert
			if (expectsToBeNormilized)
			{
				Assert.That(result.Minute, Is.EqualTo(0));
				Assert.That(result.Second, Is.EqualTo(0));
			}
			else
			{
				if (DateTime.UtcNow.Minute != 0)
					Assert.That(result.Minute, Is.Not.EqualTo(0));
				if (DateTime.UtcNow.Second != 0)
					Assert.That(result.Second, Is.Not.EqualTo(0));
				Assert.That(result.Millisecond, Is.Not.EqualTo(0));
			}
		}

		[Test]
		public void GetNormilizedLastExecDate_Failure()
		{
			//Act
			Assert.Throws<ArgumentException>(() => ProcessControlTaskManager.GetLastExecDate(new ProcessControl() { Frequency = null }));
		}

	}
}
