namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class CheckEventTaskTests
	{
		private Mock<IEventRepository> eventRepositoryMock;
		private Mock<IEventRouter> eventRouter;
		private Mock<ILogger> loggerMock;
		private Mock<IEventSourceService> eventSourceServiceMock;
		private CheckEventTask checkEventTask;


		[SetUp]
		public void Setup()
		{
			this.eventRepositoryMock = new Mock<IEventRepository>();
			this.eventRouter = new Mock<IEventRouter>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.eventSourceServiceMock = new Mock<IEventSourceService>();
			this.checkEventTask = new CheckEventTask(
				this.eventRepositoryMock.Object,
				this.eventRouter.Object,
				this.loggerMock.Object,
				this.eventSourceServiceMock.Object);
		}

		[Test]
		[TestCase(true, EventStatus.Completed)]
		[TestCase(false, EventStatus.Completed)]
		public async Task CheckEventTask_ProcessEvent(bool shouldContinue, EventStatus expectedStatus)
		{
			// Arrange -- eventTaskFactory Route Event - returns result, update event with status, return result
			var evnt = new Event { Id = 123, SourceType = EventSourceType.CheckAllPrerequisitesComplete };
			var eventResult = new LoopEventResult(shouldContinue);
			this.eventRouter.Setup(f => f.RouteEvent(evnt)).ReturnsAsync(eventResult);

			var updatedEvnt = new Event { Id = evnt.Id, Status = expectedStatus };

			// Act
			var result = await this.checkEventTask.ProcessEvent(evnt);

			// Assert
			Assert.That(result.Succeeded, Is.True); // Success should always be true unless the event errors
		}

		[Test]
		[TestCase(true, 1, EventStatus.Completed, 1)]
		[TestCase(false, 1, EventStatus.Error, 2)]
		[TestCase(false, null, EventStatus.Error, 1)]
		public async Task CheckEventTask_MarkEventResultAsync(bool success, int? existingRetries, EventStatus expectedStatus, int expectedRetries)
		{
			var evnt = new Event { Id = 123, Retries = existingRetries };

			Expression<Func<IEventRepository, Task>> updateExpression = r => r.UpdateAsync(It.Is<Event>(
					e => e.Id == evnt.Id
					&& e.Status == expectedStatus
					&& e.Retries == expectedRetries
				));

			this.eventRepositoryMock.Setup(updateExpression).Returns(Task.Delay(5));

			await this.checkEventTask.MarkEventResultAsync(new LoopEventResult(true) { Succeeded = success }, evnt);

			this.eventRepositoryMock.Verify(updateExpression, Times.Once);
		}

		[Test]
		public async Task CheckEventTask_ProcessEvent_ThrowsException()
		{
			// Arrange -- eventTaskFactory Route Event - returns result, update event with status, return result
			var evnt = new Event { Id = 123, SourceType = EventSourceType.CheckAllPrerequisitesComplete };
			this.eventRouter.Setup(f => f.RouteEvent(evnt)).ThrowsAsync(new Exception());

			var updatedEvnt = new Event { Id = evnt.Id, Status = EventStatus.Error };

			// Act
			var result = await this.checkEventTask.ProcessEvent(evnt);

			// Assert
			Assert.That(result.Succeeded, Is.EqualTo(false)); // Success should always be false when the event errors
			Assert.That(result.ShouldContinue, Is.EqualTo(false));
		}

		/// Note: CreateCategoriesForHour and CreateCategoryScoresForCategory are used because at this
		/// this time CreateCategoryScoresForCategory is the only next event type for CreateCategoriesForHour. If that changes other event types should be used
		[Test,
			TestCase(true, true, null, EventSourceType.CreateCategoriesForHour, EventSourceType.CreateCategoryScoresForCategory),
			TestCase(false, true, null, EventSourceType.CreateCategoriesForHour, EventSourceType.CreateCategoriesForHour),
			TestCase(false, true, 3, EventSourceType.CreateCategoriesForHour, EventSourceType.CreateCategoriesForHour),
			TestCase(true, false, null, EventSourceType.CreateCategoriesForHour, EventSourceType.CreateCategoryScoresForCategory),
			TestCase(false, false, 3, EventSourceType.CreateCategoriesForHour, EventSourceType.CreateCategoriesForHour)]
		public async Task CheckEventTask_CreateNextEvents(bool shouldContinue, bool succeeded, int? previousEventSourceId, EventSourceType previousEventSourceType, EventSourceType expectedNextSourceType)
		{
			// Arrange
			var eventResult = new LoopEventResult(shouldContinue) { Succeeded = succeeded };
			var previousEventId = 22;
			var previousEvent = new Event { SourceType = previousEventSourceType, SourceId = previousEventSourceId, Id = previousEventId };

			this.eventSourceServiceMock.Setup(
					m => m.CreateNextEvents(eventResult.SourceIds, eventResult.Types, It.IsAny<int>(), previousEvent.Id, previousEvent.HourId))
				.Returns(Task.Delay(5));

			// Act
			await this.checkEventTask.CreateNextEvents(eventResult, previousEvent);

			// Assert
			// Grab next types
			var nextTypes = shouldContinue ? new[] { expectedNextSourceType } : new[] { previousEventSourceType }; // Returned from Static helper, or previous
			var nextSourceIds = previousEventSourceId.HasValue ? new int[] { previousEventSourceId.Value } : null;
			var shouldCreateNextEvents = succeeded ? Times.Once() : Times.Never();
			this.eventSourceServiceMock.Verify(m => m.CreateNextEvents(nextSourceIds, nextTypes, It.IsAny<int>(), previousEvent.Id, previousEvent.HourId), shouldCreateNextEvents);
		}

		[Test]
		[TestCase(true, true)]
		[TestCase(false, false)]
		public async Task CheckEventTask_CreateNextEvents_Unsuccessful(bool succeeded, bool expectedResult)
		{
			// Arrange
			var eventResult = new LoopEventResult(true) { Succeeded = succeeded };
			var previousEvent = new Event { SourceType = EventSourceType.DeployServerDatabases, SourceId = 3, Id = 22 };

			this.eventSourceServiceMock.Setup(
					m => m.CreateNextEvents(eventResult.SourceIds, eventResult.Types, It.IsAny<int>(), previousEvent.Id, previousEvent.HourId))
				.Returns(Task.Delay(5));

			// Act
			await this.checkEventTask.CreateNextEvents(eventResult, previousEvent);

			// Assert
			// Grab next types
			var shouldCreateNextEvents = expectedResult ? Times.Once() : Times.Never();
			this.eventSourceServiceMock.Verify(m => m.CreateNextEvents(It.IsAny<IList<int>>(), It.IsAny<IList<EventSourceType>>(), It.IsAny<int>(), previousEvent.Id, previousEvent.HourId), shouldCreateNextEvents);
		}
	}
}
