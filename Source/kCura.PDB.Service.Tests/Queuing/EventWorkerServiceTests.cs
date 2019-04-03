namespace kCura.PDB.Service.Tests.Queuing
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class EventWorkerServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.agentService = new Mock<IAgentService>();
			this.eventWorkerRepository = new Mock<IEventWorkerRepository>();
			this.eventWorkerService = new EventWorkerService(this.agentService.Object, this.eventWorkerRepository.Object);

			this.worker = new EventWorker { Id = 1234, Name = "Test Agent", Type = EventWorkerType.Agent };
			this.agentService.SetupGet(s => s.AgentID).Returns(this.worker.Id);
			this.agentService.SetupGet(s => s.Name).Returns(this.worker.Name);
			this.eventWorkerRepository.Setup(r => r.ReadAsync(this.worker.Id))
				.ReturnsAsync(this.worker);
			this.eventWorkerRepository.Setup(r => r.CreateAsync(It.IsAny<EventWorker>()))
				.ReturnsAsync(this.worker);
			this.eventWorkerRepository.Setup(r => r.DeleteAsync(It.IsAny<EventWorker>()))
				.Returns(Task.Delay(1));
		}

		private Mock<IAgentService> agentService;
		private Mock<IEventWorkerRepository> eventWorkerRepository;
		private EventWorkerService eventWorkerService;
		private EventWorker worker;

		[Test]
		public async Task EventWorkerService_GetCurrentWorker()
		{
			// Act
			var result = await this.eventWorkerService.GetCurrentWorker();

			// Assert
			Assert.That(result.Id, Is.EqualTo(this.worker.Id));
			this.eventWorkerRepository.Verify(r => r.ReadAsync(this.worker.Id));
		}

		[Test]
		public async Task EventWorkerService_CreateWorker()
		{
			// Arrange

			// Act
			var result = await this.eventWorkerService.CreateWorker();

			// Assert
			Assert.That(result.Id, Is.EqualTo(this.worker.Id));
			this.eventWorkerRepository.Setup(r => r.CreateAsync(It.IsAny<EventWorker>()));
		}

		[Test]
		public async Task EventWorkerService_RemoveCurrentWorker()
		{
			// Arrange

			// Act
			await this.eventWorkerService.RemoveCurrentWorker();

			// Assert
			this.eventWorkerRepository.Verify(r => r.DeleteAsync(It.IsAny<EventWorker>()));
		}

	}
}
