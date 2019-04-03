namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class DatabaseDeploymentServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.eventRepository = new Mock<IEventRepository>();

			this.databaseDeploymentService = new DatabaseDeploymentService(this.eventRepository.Object, this.serverRepository.Object);
		}

		private Mock<IServerRepository> serverRepository;
		private Mock<IEventRepository> eventRepository;
		private DatabaseDeploymentService databaseDeploymentService;

		[Test]
		public async Task FindAnyFailedDeployments()
		{
			// Arrange
			var servers = new[] { new Server { ServerId = 123, ServerType = ServerType.Database }, new Server { ServerId = 345, ServerType = ServerType.Database } };
			this.serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(servers);
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(EventSourceType.DeployServerDatabases, It.IsAny<int>()))
				.Returns<EventSourceType, int>((t, sid) => Task.FromResult(new Event { Status = EventStatus.Error, SourceId = sid, SourceType = EventSourceType.DeployServerDatabases }));

			// Act
			var result = await databaseDeploymentService.FindAnyFailedDeployments();

			// Assert
			Assert.That(result, Is.True);
		}
	}
}
