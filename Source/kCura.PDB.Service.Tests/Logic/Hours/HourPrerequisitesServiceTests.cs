namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Hours;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class HourPrerequisitesServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.hourRepository = new Mock<IHourRepository>();
			this.eventRepository = new Mock<IEventRepository>();

			hourPrerequisitesService = new HourPrerequisitesService(this.serverRepository.Object, this.eventRepository.Object, this.hourRepository.Object);
		}

		private Mock<IServerRepository> serverRepository;
		private Mock<IHourRepository> hourRepository;
		private Mock<IEventRepository> eventRepository;
		private HourPrerequisitesService hourPrerequisitesService;

		[Test]
		public async Task CheckForPrerequisites_SystemPaused()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync()).ReturnsAsync(EventSystemState.Paused);
			this.eventRepository.Setup(r => r.UpdateEventTypesAsync()).Returns(Task.Delay(10));

			// Act
			var result = await hourPrerequisitesService.CheckForPrerequisites();

			// Assert
			Assert.That(result.Types, Is.Null);
			Assert.That(result.SourceIds, Is.Null);
			Assert.That(result.Succeeded, Is.True);
		}

		[Test]
		public async Task CheckForPrerequisites_NoServers()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync()).ReturnsAsync(EventSystemState.Normal);
			this.serverRepository.Setup(r => r.ReadServerPendingQosDeploymentAsync()).ReturnsAsync(new Server[0]);

			// Act
			var result = await hourPrerequisitesService.CheckForPrerequisites();

			// Assert
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.CreateNextHour));
			Assert.That(result.SourceIds, Is.Null);
			Assert.That(result.Succeeded, Is.True);
		}

		[Test]
		public async Task CheckForPrerequisites_Servers()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync()).ReturnsAsync(EventSystemState.Normal);
			this.serverRepository.Setup(r => r.ReadServerPendingQosDeploymentAsync()).ReturnsAsync(new[] { new Server { ServerId = 1234 }, });
			this.eventRepository.Setup(r => r.UpdateReadEventSystemStateAsync(EventSystemState.Prerequisites)).Returns(Task.Delay(10));

			// Act
			var result = await hourPrerequisitesService.CheckForPrerequisites();

			// Assert
			this.eventRepository.Verify(r => r.UpdateReadEventSystemStateAsync(EventSystemState.Prerequisites));
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.StartPrerequisites));
			Assert.That(result.SourceIds, Is.Null);
			Assert.That(result.Succeeded, Is.True);
		}

		[Test]
		public async Task CheckAllPrerequisitesComplete_Complete()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Prerequisites);
			this.serverRepository.Setup(r => r.ReadServerPendingQosDeploymentAsync())
				.ReturnsAsync(new Server[0]);
			this.hourRepository.Setup(r => r.ReadAnyIncompleteHoursAsync())
				.ReturnsAsync(false);

			// Act
			var result = await hourPrerequisitesService.CheckAllPrerequisitesComplete();

			// Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task CheckAllPrerequisitesComplete_IncompleteQosDeployment()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Prerequisites);
			this.serverRepository.Setup(r => r.ReadServerPendingQosDeploymentAsync())
				.ReturnsAsync(new[] { new Server() });
			this.hourRepository.Setup(r => r.ReadAnyIncompleteHoursAsync())
				.ReturnsAsync(false);

			// Act
			var result = await hourPrerequisitesService.CheckAllPrerequisitesComplete();

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task CheckAllPrerequisitesComplete_IncompleteHours()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Prerequisites);
			this.serverRepository.Setup(r => r.ReadServerPendingQosDeploymentAsync())
				.ReturnsAsync(new[] { new Server() });
			this.hourRepository.Setup(r => r.ReadAnyIncompleteHoursAsync())
				.ReturnsAsync(false);

			// Act
			var result = await hourPrerequisitesService.CheckAllPrerequisitesComplete();

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task CheckAllPrerequisitesComplete_AlreadyComplete()
		{
			// Arrange
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Normal);

			// Act
			var result = await hourPrerequisitesService.CheckAllPrerequisitesComplete();

			// Assert
			Assert.That(result, Is.True);
		}
	}
}
