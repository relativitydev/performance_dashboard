namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class EventSourceServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventRepository = new Mock<IEventRepository>();
			this.configurationRepository = new Mock<IConfigurationRepository>();
			this.categoryRepository = new Mock<ICategoryRepository>();
			this.eventSourceService = new EventSourceService(
				eventRepository.Object,
				new MockQueuingService(),
				this.configurationRepository.Object,
				this.categoryRepository.Object);
		}

		private Mock<IEventRepository> eventRepository;
		private Mock<IConfigurationRepository> configurationRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private EventSourceService eventSourceService;

		[Test]
		public async Task EnqueueTasksForPendingEvents()
		{
			// Arrange
			var events = new[]
			{
				new Event { SourceType = EventSourceType.ScoreHour, SourceId = 1234, Id = 1 }, // Duplicate events will be distincted out
				new Event { SourceType = EventSourceType.ScoreHour, SourceId = 1234, Id = 2 },
				new Event { SourceType = EventSourceType.ScoreCategoryScore, SourceId = 4567, Id = 3 },
				new Event { SourceType = EventSourceType.StartHour, SourceId = 4567, Id = 4 },
			};
			var eventIdsToEnqueue = new long[] { 1, 2, 3, 4 }; // should not include 4 since it should be filtered out
			var numEventsToEnqueue = 50;
			this.configurationRepository.Setup(m => m.ReadValueAsync<long>(ConfigurationKeys.NumberOfEventsToEnqueue))
				.ReturnsAsync(numEventsToEnqueue);
			this.eventRepository.Setup(r => r.ReadIdsByStatusAsync(EventStatus.Pending, numEventsToEnqueue, It.IsAny<IList<EventSourceType>>()))
				.ReturnsAsync(eventIdsToEnqueue);
			this.eventRepository.Setup(r => r.UpdateStatusAsync(eventIdsToEnqueue, EventStatus.PendingHangfire)).ReturnsAsync(events);

			// Prevent any StartHour events from running.

			this.eventRepository.Setup(r => r.ReadSingleByTypeAndStatusAsync(
				EventConstants.SingletonEventTypes, EventStatus.Pending, EventConstants.ActiveEventStatuses))
				.ReturnsAsync(new long[] { 4 });

			// Act
			await this.eventSourceService.EnqueueTasksForPendingEvents();

			// Assert
			this.eventRepository.Verify(r => r.UpdateStatusAsync(It.Is<IList<long>>(list => list.Count == 4 && list.All(id => eventIdsToEnqueue.Contains(id))), EventStatus.PendingHangfire));
			this.eventRepository.Verify(r => r.UpdateAsync(It.Is<Event>(e => e.Status == EventStatus.Duplicate)), Times.Once);
		}

		[Test]
		public async Task CreateHourProcessingEvents()
		{
			// Arrange
			this.eventRepository.Setup(r => r.CreateAsync(It.IsAny<Event>()))
				.ReturnsAsync(new Event());

			// Prevent any StartHour events from running.
			var pendingOrActiveEventTypes = EventConstants.ActiveEventStatuses.Concat(new[] { EventStatus.Pending }).ToList();
			this.eventRepository.Setup(r => r.ReadTypesByTypeAndStatusAsync(
					EventConstants.HourBootstrapEvents,
					pendingOrActiveEventTypes))
				.ReturnsAsync(new EventSourceType[0]);

			// Act
			await this.eventSourceService.CreateHourProcessingEvents();

			// Assert
			this.eventRepository.Verify(r => r.CreateAsync(It.IsAny<Event>()));
		}

		[Test]
		[TestCase(new[] { 1, 2, 3 }, new[] { EventSourceType.CheckAllPrerequisitesComplete }, 50, 1)]
		[TestCase(new[] { 1, 2, 3 }, new EventSourceType[] { }, 50, 1)]
		[TestCase(null, new[] { EventSourceType.CheckAllPrerequisitesComplete }, 50, 1)]
		public async Task CreateNextEvents_ListSourceTypes(IList<int> sourceIds, IList<EventSourceType> eventSourceTypes, int? delay, int? previousEventIdInt)
		{
			// Arrange
			var previousEventId = (long?)previousEventIdInt; // Fix for Nunit

			this.eventRepository.Setup(r => r.CreateAsync(It.IsAny<IList<Event>>())).Returns(Task.Delay(10));

			// Act
			await this.eventSourceService.CreateNextEvents(sourceIds, eventSourceTypes, delay, previousEventId, 1234);

			// Assert
			this.eventRepository.Verify(r => r.CreateAsync(It.IsAny<IList<Event>>()), Times.Exactly(eventSourceTypes.Any() ? 1 : 0));
		}

		[Test]
		public async Task SetInitialHourIdForEvents_ScoreCategoryScore()
		{
			// Arrange
			int hourId = 2345;
			var nextEvents = new[]
			{
				new Event { SourceId = 123, SourceType = EventSourceType.ScoreCategoryScore },
				new Event { SourceId = 234, SourceType = EventSourceType.ScoreCategoryScore }
			};
			this.categoryRepository.Setup(r => r.ReadByCategoryScoreAsync(It.IsAny<int>())).ReturnsAsync(new Category { HourId = hourId });

			// Act
			await this.eventSourceService.SetInitialHourIdForEvents(nextEvents);

			// Assert
			Assert.That(nextEvents.All(e => e.HourId == hourId));
		}

		[Test]
		public async Task SetInitialHourIdForEvents_StartHour()
		{
			// Arrange
			var nextEvents = new[]
			{
				new Event { SourceId = 123, SourceType = EventSourceType.StartHour },
				new Event { SourceId = 234, SourceType = EventSourceType.StartHour }
			};

			// Act
			await this.eventSourceService.SetInitialHourIdForEvents(nextEvents);

			// Assert
			Assert.That(nextEvents.All(e => e.HourId != null));
		}

		[Test]
		[TestCase(0, EventConstants.DefaultEventDelay)]
		[TestCase(int.MaxValue, EventConstants.DefaultMaxEventDelay)]
		public void GetIncreasingDelay(int retries, int expectedDelay)
		{
			var evnt = new Event { Retries = retries };
			var result = EventSourceService.GetIncreasingDelay(evnt);
			Assert.That(result, Is.EqualTo(expectedDelay));
		}

		private class MockQueuingService : IQueuingService
		{
			public void Enqueue<T>(Expression<Action<T>> methodCall, int? delay = null)
			{
				// does nothing
			}
		}
	}
}
