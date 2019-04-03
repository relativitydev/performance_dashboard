namespace kCura.PDD.Web.Test.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDD.Web.Controllers;
	using Moq;
	using NUnit.Framework;
	
	[TestFixture, Category("Unit")]
	public class BackfillControllerTests
	{
		private BackfillController backfillController;
		private Mock<ISqlServerRepository> sqlServerRepositoryMock;

		[SetUp]
		public void SetUp()
		{
			this.sqlServerRepositoryMock = new Mock<ISqlServerRepository>();
			this.backfillController = new BackfillController(sqlServerRepositoryMock.Object);
		}

		[Test]
		[TestCase(null, null, 0, 0, EventSystemState.Normal, null, null, 0, 0, 0, 0)]
		public async Task GetBackfillStatus(string logEntryTaskCompleted, DateTime? lastEventLastUpdated, int pendingEvents, int erroredEvents, EventSystemState systemState, DateTime? minSampleDate, DateTime? maxSampleDate, int hoursAwaitingAnalysis, int hoursAwaitingDiscovery, int hoursAwaitingScoring, int hoursCompleted)
		{
			// Arrange
			var logEntryResult = logEntryTaskCompleted == null ? null : new LogEntry {TaskCompleted = logEntryTaskCompleted};
			this.sqlServerRepositoryMock.Setup(m => m.LogRepository.ReadLastAsync()).ReturnsAsync(logEntryResult);
			var lastEventResult = lastEventLastUpdated == null ? null : new Event {LastUpdated = lastEventLastUpdated.Value};
			this.sqlServerRepositoryMock.Setup(m => m.EventRepository.ReadLastAsync()).ReturnsAsync(lastEventResult);
			this.sqlServerRepositoryMock.Setup(m => m.EventRepository.ReadCountByStatusAsync(It.Is<IList<EventStatus>>(val =>
					val.Contains(EventStatus.Pending) && val.Contains(EventStatus.PendingHangfire) && val.Count == 2)))
				.ReturnsAsync(pendingEvents);
			this.sqlServerRepositoryMock
				.Setup(m => m.EventRepository.ReadCountByStatusAsync(It.Is<EventStatus>(val => val == EventStatus.Error)))
				.ReturnsAsync(erroredEvents);
			this.sqlServerRepositoryMock.Setup(m => m.EventRepository.ReadEventSystemStateAsync()).ReturnsAsync(systemState);
			this.sqlServerRepositoryMock.Setup(m => m.SampleHistoryRepository.ReadSampleRange())
				.Returns(new SampleHistoryRange() {MaxSampleDate = maxSampleDate, MinSampleDate = minSampleDate});
			this.sqlServerRepositoryMock.Setup(m => m.BackfillRepository.ReadHoursAwaitingAnalysis())
				.ReturnsAsync(hoursAwaitingAnalysis);
			this.sqlServerRepositoryMock.Setup(m => m.BackfillRepository.ReadHoursAwaitingDiscovery())
				.ReturnsAsync(hoursAwaitingDiscovery);
			this.sqlServerRepositoryMock.Setup(m => m.BackfillRepository.ReadHoursAwaitingScoring())
				.ReturnsAsync(hoursAwaitingScoring);
			this.sqlServerRepositoryMock.Setup(m => m.BackfillRepository.ReadHoursCompletedScoring())
				.ReturnsAsync(hoursCompleted);
			
			// Act
			var result = await this.backfillController.GetBackfillStatus();

			// Assert
			Assert.That(result, Is.Not.Null);
		}
	}
}
