namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class EventOrphanServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventLockRepository = new Mock<IEventLockRepository>();
			this.eventRepository = new Mock<IEventRepository>();
			this.eventWorkerRepository = new Mock<IEventWorkerRepository>();
			this.agentRepository = new Mock<IAgentRepository>();
			this.logger = TestUtilities.GetMockLogger();
			this.eventOrphanService = new EventOrphanService(
				this.eventLockRepository.Object,
				this.eventRepository.Object,
				this.eventWorkerRepository.Object,
				this.agentRepository.Object,
				this.logger.Object);
		}

		private Mock<IEventLockRepository> eventLockRepository;
		private Mock<IEventRepository> eventRepository;
		private Mock<IEventWorkerRepository> eventWorkerRepository;
		private Mock<IAgentRepository> agentRepository;
		private Mock<ILogger> logger;
		private EventOrphanService eventOrphanService;

		[Test]
		public async Task MarkOrphanedEventsErrored()
		{
			// Arrange
			var worker = new EventWorker { Id = 234 };
			var eventLocks = new[] { new EventLock { EventId = 333 }, new EventLock { EventId = 444 } };
			this.eventLockRepository.Setup(r => r.ReadByWorker(It.IsAny<EventWorker>()))
				.ReturnsAsync(eventLocks);
			this.eventRepository.Setup(r => r.ReadAsync(It.IsAny<long>()))
				.ReturnsAsync(new Event { Status = EventStatus.InProgress });
			this.eventRepository.Setup(r => r.UpdateAsync(It.IsAny<Event>()))
				.Returns(Task.Delay(1));
			this.eventLockRepository.Setup(r => r.Release(It.IsAny<EventWorker>())).Returns(Task.Delay(1));

			// Act
			await this.eventOrphanService.MarkOrphanedEventsErrored(worker);

			// Assert
			this.eventRepository.Verify(r => r.UpdateAsync(It.Is<Event>(e => e.Status == EventStatus.Error)), Times.Exactly(eventLocks.Length));
			this.eventLockRepository.Verify(r => r.Release(It.IsAny<EventWorker>()));
		}

		[Test]
		public async Task ResolveOrphanedEventLocks()
		{
			// Arrange
			var orphanedWorkerId = 444;

			// Setup for MarkOrphanedEventsErrored
			var eventLocks = new[] { new EventLock { EventId = 333 }, new EventLock { EventId = 444 } };
			this.eventLockRepository.Setup(r => r.ReadByWorker(It.IsAny<EventWorker>()))
				.ReturnsAsync(eventLocks);
			this.eventRepository.Setup(r => r.ReadAsync(It.IsAny<long>()))
				.ReturnsAsync(new Event { Status = EventStatus.InProgress });
			this.eventRepository.Setup(r => r.UpdateAsync(It.IsAny<Event>()))
				.Returns(Task.Delay(1));
			this.eventLockRepository.Setup(r => r.Release(It.IsAny<EventWorker>())).Returns(Task.Delay(1));

			this.agentRepository.Setup(r => r.ReadAllEnabledAgentsAsync()).ReturnsAsync(new[] { 111, 222, 333 });
			this.eventWorkerRepository.Setup(r => r.ReadAllWorkersAsync())
				.ReturnsAsync(new[] { new EventWorker { Id = 111, Type = EventWorkerType.Agent }, new EventWorker { Id = orphanedWorkerId, Type = EventWorkerType.Agent } });
			this.eventWorkerRepository.Setup(r => r.DeleteAsync(It.IsAny<EventWorker>())).Returns(Task.Delay(1));

			// Act
			await this.eventOrphanService.ResolveOrphanedEventLocks();

			// Assert
			this.eventLockRepository.Verify(r => r.Release(It.Is<EventWorker>(w => w.Id == orphanedWorkerId)), Times.Once);
			this.eventLockRepository.Verify(r => r.Release(It.Is<EventWorker>(w => w.Id != orphanedWorkerId)), Times.Never);
			this.eventWorkerRepository.Verify(r => r.DeleteAsync(It.Is<EventWorker>(w => w.Id == orphanedWorkerId)), Times.Once);
			this.eventWorkerRepository.Verify(r => r.DeleteAsync(It.Is<EventWorker>(w => w.Id != orphanedWorkerId)), Times.Never);
		}

		[Test]
		public async Task ResolveTimedOutEvents()
		{
			// Arrange
			this.eventRepository.Setup(r => r.UpdateTimedOutEventsAsync(It.IsAny<DateTime>())).Returns(Task.Delay(1));

			// Act
			await this.eventOrphanService.ResolveTimedOutEvents();

			// Assert
			this.eventRepository.Verify(r => r.UpdateTimedOutEventsAsync(It.Is<DateTime>(dt => dt < DateTime.UtcNow)));
		}
	}
}
