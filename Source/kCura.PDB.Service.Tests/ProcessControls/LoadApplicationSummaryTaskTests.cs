namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class LoadApplicationSummaryTaskTests
	{
		[SetUp]
		public void Setup()
		{
			this.sqlRepo = new Mock<ISqlServerRepository>();
		}

		private Mock<ISqlServerRepository> sqlRepo;

		[Test]
		public void LoadApplicationSummaryTask_Execute()
		{
			// Arrange
			this.sqlRepo.Setup(r => r.PerformanceSummaryRepository.LoadApplicationHealthSummary(It.IsAny<DateTime>()));

			// Act
			var task = new LoadApplicationSummaryTask(this.sqlRepo.Object, 123);
			task.Execute(new ProcessControl() { LastProcessExecDateTime = DateTime.UtcNow });

			// Assert
			this.sqlRepo.Verify(r => r.PerformanceSummaryRepository.LoadApplicationHealthSummary(It.IsAny<DateTime>()), Times.Once);
		}
	}
}
