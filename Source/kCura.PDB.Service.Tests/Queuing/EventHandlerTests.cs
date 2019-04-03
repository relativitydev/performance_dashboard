namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;
	using EventHandler = kCura.PDB.Service.Queuing.EventHandler;

	[TestFixture]
	[Category("Unit")]
	public class EventHandlerTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventTask = new Mock<IEventTask>();
			this.eventTaskFactory = new Mock<IEventTaskFactory>();
			this.eventRepository = new Mock<IEventRepository>();
			this.eventLockRepositoryMock = new Mock<IEventLockRepository>();
			this.eventWorkerService = new Mock<IEventWorkerService>();
			this.eventChildKernelFactory = new Mock<IEventChildKernelFactory>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.eventHandler = new EventHandler(
				this.eventTaskFactory.Object,
				this.eventRepository.Object,
				this.eventLockRepositoryMock.Object,
				this.eventWorkerService.Object,
				this.eventChildKernelFactory.Object,
				this.loggerMock.Object);

			this.eventWorkerService.Setup(ews => ews.GetCurrentWorker()).ReturnsAsync(new EventWorker { Id = 1234 });
		}

		private Mock<IEventTaskFactory> eventTaskFactory;
		private Mock<IEventTask> eventTask;
		private Mock<IEventRepository> eventRepository;
		private Mock<IEventLockRepository> eventLockRepositoryMock;
		private Mock<IEventWorkerService> eventWorkerService;
		private Mock<IEventChildKernelFactory> eventChildKernelFactory;
		private Mock<ILogger> loggerMock;
		private EventHandler eventHandler;

		[TestCase(true)]
		[TestCase(false)]
		public async Task HandleEvent_Success(bool eventResultSuccess)
		{
			var eventId = 1234;
			var eventType = EventSourceType.ScoreCategoryScore;
			var evnt = new Event { Id = eventId, SourceType = eventType, Status = EventStatus.PendingHangfire };
			this.eventTaskFactory.Setup(f => f.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType)).Returns(this.eventTask.Object);
			this.eventRepository.Setup(m => m.ReadAsync(eventId)).ReturnsAsync(evnt);
			var eventResult = new EventResult(evnt.SourceType) { Succeeded = eventResultSuccess };
			this.eventTask.Setup(t => t.ProcessEvent(It.IsAny<Event>())).ReturnsAsync(eventResult);
			var kernelWrapper = new Mock<IKernelWrapper>();
			var kernel = new Mock<IKernel>();
			kernelWrapper.SetupGet(w => w.Kernel).Returns(kernel.Object);
			this.eventChildKernelFactory.Setup(f => f.CreateChildKernel(evnt))
				.Returns(kernelWrapper.Object);
			var eventLock = new EventLock
			{
				Id = 1,
				EventId = evnt.Id,
				EventTypeId = evnt.SourceTypeId,
				SourceId = evnt.SourceId
			};
			this.eventLockRepositoryMock.Setup(m => m.Claim(evnt, It.IsAny<int>())).ReturnsAsync(eventLock);
			this.eventLockRepositoryMock.Setup(m => m.Release(eventLock)).Returns(Task.Delay(5));
			this.eventRepository.Setup(
				m => m.UpdateAsync(It.Is<Event>(e => e.Id == eventId && e.Status == EventStatus.InProgress))).Returns(Task.Delay(5));

			// Act
			await this.eventHandler.HandleEvent(eventId, eventType);

			// Assert
			this.eventTaskFactory.Verify(m => m.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType), Times.Once);
			this.eventRepository.Verify(m => m.ReadAsync(evnt.Id), Times.Once);
			this.eventLockRepositoryMock.Verify(m => m.Claim(evnt, It.IsAny<int>()), Times.Once);
			this.eventRepository.Verify(m => m.UpdateAsync(It.Is<Event>(e => e.Status == EventStatus.InProgress && e.Id == evnt.Id)), Times.Once);
			this.eventTask.Verify(m => m.ProcessEvent(It.Is<Event>(e => e.Id == evnt.Id)), Times.Once);
			this.eventTask.Verify(m => m.MarkEventResultAsync(eventResult, evnt), Times.Once);
			this.eventLockRepositoryMock.Verify(m => m.Release(eventLock), Times.Once);
			this.eventTask.Verify(m => m.CreateNextEvents(eventResult, It.Is<Event>(e => e.Id == evnt.Id)), Times.Once());
		}



		[Test]
		public async Task HandleEvent_LockFailed()
		{
			var eventId = 1234;
			var eventType = EventSourceType.ScoreCategoryScore;
			var evnt = new Event { Id = eventId, SourceType = eventType, Status = EventStatus.PendingHangfire };
			this.eventTaskFactory.Setup(f => f.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType)).Returns(this.eventTask.Object);
			this.eventRepository.Setup(m => m.ReadAsync(eventId)).ReturnsAsync(evnt);
			var eventResult = new EventResult(evnt.SourceType) { Succeeded = true };
			this.eventTask.Setup(t => t.ProcessEvent(It.IsAny<Event>())).ReturnsAsync(eventResult);
			var eventLock = (EventLock)null;
			this.eventLockRepositoryMock.Setup(m => m.Claim(evnt, It.IsAny<int>())).ReturnsAsync(eventLock);
			this.eventLockRepositoryMock.Setup(m => m.Release(eventLock)).Returns(Task.Delay(5));
			var kernelWrapper = new Mock<IKernelWrapper>();
			var kernel = new Mock<IKernel>();
			kernelWrapper.SetupGet(w => w.Kernel).Returns(kernel.Object);
			this.eventChildKernelFactory.Setup(f => f.CreateChildKernel(evnt))
				.Returns(kernelWrapper.Object);

			// Act
			await this.eventHandler.HandleEvent(eventId, eventType);

			// Assert
			this.eventTaskFactory.Verify(m => m.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType), Times.Never);
			this.eventRepository.Verify(m => m.ReadAsync(evnt.Id), Times.Once);
			this.eventLockRepositoryMock.Verify(m => m.Claim(evnt, It.IsAny<int>()), Times.Once);
			this.eventRepository.Verify(m => m.UpdateAsync(It.Is<Event>(e => e.Status == EventStatus.Duplicate && e.Id == evnt.Id)));
		}

		[Test]
		public async Task HandleEvent_ExceptionWithLock()
		{
			var eventId = 1234;
			var eventType = EventSourceType.ScoreCategoryScore;
			var evnt = new Event { Id = eventId, SourceType = eventType, Status = EventStatus.PendingHangfire };
			this.eventTaskFactory.Setup(f => f.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType)).Returns(this.eventTask.Object);
			this.eventRepository.Setup(m => m.ReadAsync(eventId)).ReturnsAsync(evnt);
			var kernelWrapper = new Mock<IKernelWrapper>();
			var kernel = new Mock<IKernel>();
			kernelWrapper.SetupGet(w => w.Kernel).Returns(kernel.Object);
			this.eventChildKernelFactory.Setup(f => f.CreateChildKernel(evnt))
				.Returns(kernelWrapper.Object);

			var eventLock = new EventLock
			{
				Id = 1,
				EventId = evnt.Id,
				EventTypeId = evnt.SourceTypeId,
				SourceId = evnt.SourceId
			};
			this.eventLockRepositoryMock.Setup(m => m.Claim(evnt, It.IsAny<int>())).ReturnsAsync(eventLock);
			this.eventLockRepositoryMock.Setup(m => m.Release(eventLock)).Returns(Task.Delay(5));

			this.eventRepository.Setup(m => m.UpdateAsync(It.IsAny<Event>())).Returns(Task.Delay(5));

			this.eventTask.Setup(t => t.ProcessEvent(It.IsAny<Event>())).Throws<Exception>();

			// Act
			await this.eventHandler.HandleEvent(eventId, eventType);

			// Assert
			this.eventLockRepositoryMock.Verify(m => m.Claim(evnt, It.IsAny<int>()), Times.Once);
			this.eventTaskFactory.Verify(m => m.GetEventTask(It.IsAny<IKernel>(), evnt.SourceType), Times.Once);
			this.eventRepository.Verify(m => m.ReadAsync(evnt.Id), Times.Exactly(2)); // once for initial read and again to mark for error
			this.eventRepository.Verify(m => m.UpdateAsync(It.Is<Event>(e => e.Status == EventStatus.Error))); // for when it errors
			this.eventTask.Verify(m => m.ProcessEvent(It.Is<Event>(e => e.Id == evnt.Id)), Times.Once);
			this.eventLockRepositoryMock.Verify(m => m.Release(eventLock), Times.Once);
		}

		[Test]
		public async Task HandleEvent_CancelledEvent()
		{
			var eventId = 1234;
			var eventType = EventSourceType.ScoreCategoryScore;
			var evnt = new Event { Id = eventId, SourceType = eventType, Status = EventStatus.Cancelled };
			this.eventRepository.Setup(m => m.ReadAsync(eventId))
				.ReturnsAsync(evnt);
			
			// Act
			await this.eventHandler.HandleEvent(eventId, eventType);

			// Assert
			this.eventRepository.VerifyAll();
		}
	}
}
