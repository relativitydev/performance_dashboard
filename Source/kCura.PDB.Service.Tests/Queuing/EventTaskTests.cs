namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
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
	public class EventTaskTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventRepository = new Mock<IEventRepository>();
			this.eventRouter = new Mock<IEventRouter>();
			this.eventSourceService = new Mock<IEventSourceService>();
			this.logger = TestUtilities.GetMockLogger();
			this.eventTask = new EventTask(eventRepository.Object, eventRouter.Object, this.logger.Object, this.eventSourceService.Object);
		}

		private Mock<IEventRepository> eventRepository;
		private Mock<IEventRouter> eventRouter;
		private Mock<IEventSourceService> eventSourceService;
		private Mock<ILogger> logger;
		private EventTask eventTask;

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task EventTask_ProcessEvent(bool eventResultSuccess)
		{
			// Arrange -- eventTaskFactory Route Event - returns result, update event with status, return result
			var evnt = new Event { Id = 123 };
			var eventResult = new EventResult(1) { Succeeded = eventResultSuccess };
			this.eventRouter.Setup(f => f.RouteEvent(evnt)).ReturnsAsync(eventResult);

			// Act
			var result = await this.eventTask.ProcessEvent(evnt);

			// Assert
			Assert.That(result, Is.EqualTo(eventResult));
		}

		[Test]
		[TestCase(true, 3, EventStatus.Completed, 3)]
		[TestCase(false, 3, EventStatus.Error, 4)]
		[TestCase(false, 15, EventStatus.Error, 16)]
		[TestCase(false, 1, EventStatus.Pending, 2)]
		[TestCase(false, null, EventStatus.Pending, 1)]
		public async Task EventTask_MarkEventResultAsync(bool eventResultSuccess, int? existingRetries, EventStatus expectedStatus, int expectedRetries)
		{
			var evnt = new Event { Id = 123, Retries = existingRetries };

			Expression<Func<IEventRepository, Task>> updateExpression = r => r.UpdateAsync(It.Is<Event>(
					e => e.Id == evnt.Id
					&& e.Status == expectedStatus
					&& e.Retries == expectedRetries
				));

			this.eventRepository.Setup(updateExpression).Returns(Task.Delay(5));

			await this.eventTask.MarkEventResultAsync(new EventResult(1) { Succeeded = eventResultSuccess }, evnt);

			this.eventRepository.Verify(updateExpression, Times.Once);
		}

		[Test]
		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(false, false)]
		public async Task EventTask_CreateNextEvents(bool shouldContinue, bool succeeded)
		{
			// Arrange
			var sourceId = 1;
			var eventResult = new EventResult(sourceId) { Succeeded = succeeded };
			var previousEvent = new Event { SourceType = EventSourceType.CreateNextHour };

			this.eventSourceService.Setup(
					m => m.CreateNextEvents(eventResult.SourceIds, eventResult.Types, eventResult.Delay, previousEvent.Id, previousEvent.HourId))
				.Returns(Task.Delay(5));

			// Act
			await this.eventTask.CreateNextEvents(eventResult, previousEvent);

			// Assert
			this.eventSourceService.Verify(m => m.CreateNextEvents(eventResult.SourceIds, eventResult.Types, eventResult.Delay, previousEvent.Id, previousEvent.HourId), Times.Exactly(eventResult.Succeeded && eventResult.ShouldContinue ? 1 : 0));
		}
	}
}
