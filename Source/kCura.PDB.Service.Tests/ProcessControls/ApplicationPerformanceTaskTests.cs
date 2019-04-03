namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class ApplicationPerformanceTaskTests
	{
		[SetUp]
		public void Setup()
		{
			this.sqlRepo = new Mock<ISqlServerRepository>();
			this.dataContext = new Mock<IPDDModelDataContext>();
			this.logger = new Mock<ILogger>();
		}

		private Mock<ISqlServerRepository> sqlRepo;
		private Mock<IPDDModelDataContext> dataContext;
		private Mock<ILogger> logger;

		[Test]
		public void ApplicationPerformanceTask_LoadApplicationDW()
		{
			// Arrange
			this.dataContext.Setup(m => m.ReadMeasures())
				.Returns(new List<Measure>()
				{
					new Measure
					{
						Frequency = 5,
						LastDataLoadDateTime = DateTime.UtcNow.AddHours(-1),
						MeasureID = (int)ApplicationPerformanceTask.ProcTypes.LoadErrorHealthDwData
					}
				}.AsQueryable());
			this.sqlRepo.Setup(r => r.PerformanceSummaryRepository.LoadErrorHealthDwData(It.IsAny<DateTime>()));

			// Act
			var task = new ApplicationPerformanceTask(this.logger.Object, this.sqlRepo.Object, this.dataContext.Object, 123);
			task.LoadApplicationDw(ApplicationPerformanceTask.ProcTypes.LoadErrorHealthDwData);

			// Assert
			this.dataContext.Verify(m => m.SubmitChanges(), Times.Exactly(13));
			this.sqlRepo.Verify(r => r.PerformanceSummaryRepository.LoadErrorHealthDwData(It.IsAny<DateTime>()), Times.Exactly(13));
		}

		[Test]
		public void ApplicationPerformanceTask_Execute()
		{
			// Arrange
			this.dataContext.Setup(m => m.ReadMeasures())
				.Returns(new List<Measure>()
				{
					new Measure
					{
						Frequency = 5,
						LastDataLoadDateTime = DateTime.UtcNow.AddHours(-1),
						MeasureID = (int)ApplicationPerformanceTask.ProcTypes.LoadErrorHealthDwData
					},
					new Measure
					{
						Frequency = 5,
						LastDataLoadDateTime = DateTime.UtcNow.AddHours(-1),
						MeasureID = (int)ApplicationPerformanceTask.ProcTypes.LoadUserHealthDwData
					}
				}.AsQueryable());
			this.dataContext.Setup(m => m.SubmitChanges());
			this.sqlRepo.Setup(r => r.PerformanceSummaryRepository.LoadErrorHealthDwData(It.IsAny<DateTime>()));
			this.sqlRepo.Setup(r => r.PerformanceSummaryRepository.LoadUserHealthDwData(It.IsAny<DateTime>()));

			// Act
			var task = new ApplicationPerformanceTask(this.logger.Object, this.sqlRepo.Object, this.dataContext.Object, 123);
			task.Execute(null);

			// Assert
			this.dataContext.Verify(m => m.SubmitChanges(), Times.Exactly(13 * 2));
			this.sqlRepo.Verify(r => r.PerformanceSummaryRepository.LoadErrorHealthDwData(It.IsAny<DateTime>()), Times.Exactly(13));
			this.sqlRepo.Verify(r => r.PerformanceSummaryRepository.LoadUserHealthDwData(It.IsAny<DateTime>()), Times.Exactly(13));
		}
	}
}
