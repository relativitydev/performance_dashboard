namespace kCura.PDB.Service.Tests.Testing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Testing;
	using kCura.PDB.Tests.Common.Extensions;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class TestEventPrimerTests
	{
		private TestEventPrimer service;
		private Mock<IHourTestDataRepository> hourTestDataRepositoryMock;
		private Mock<ICategoryRepository> categoryRepositoryMock;
		private Mock<IEventRepository> eventRepositoryMock;
		private Mock<IDatabaseService> databaseServiceMock;

		[SetUp]
		public void Setup()
		{
			this.hourTestDataRepositoryMock = new Mock<IHourTestDataRepository>();
			this.categoryRepositoryMock = new Mock<ICategoryRepository>();
			this.eventRepositoryMock = new Mock<IEventRepository>();
			this.databaseServiceMock = new Mock<IDatabaseService>();
			this.service = new TestEventPrimer(
				this.hourTestDataRepositoryMock.Object,
				this.categoryRepositoryMock.Object,
				this.eventRepositoryMock.Object,
				this.databaseServiceMock.Object);
		}

		[Test]
		public async Task CreateNeededData()
		{
			// Arrange
			var hourA = new Hour { Id = 1 };
			var hours = new List<Hour> { hourA };
			this.hourTestDataRepositoryMock.Setup(m => m.ReadHoursAsync()).ReturnsAsync(hours);

			var categoryA = new Category { HourId = hourA.Id, CategoryType = CategoryType.RecoverabilityIntegrity };
			var createdCategoryA =
				new Category { HourId = hourA.Id, CategoryType = CategoryType.RecoverabilityIntegrity, Id = 2 };
			this.categoryRepositoryMock.Setup(m => m.CreateAsync(It.Is<Category>(c => c.HourId == categoryA.HourId && c.CategoryType == categoryA.CategoryType))).ReturnsAsync(createdCategoryA);
			var eventA = new Event
				             {
					             HourId = createdCategoryA.HourId,
					             SourceId = createdCategoryA.Id,
					             SourceType = EventSourceType.CreateCategoryScoresForCategory
				             };
			var createdEventA = new Event
				                    {
					                    HourId = createdCategoryA.HourId,
					                    SourceId = createdCategoryA.Id,
					                    SourceType = EventSourceType.CreateCategoryScoresForCategory,
										Id = 3,
										Status = EventStatus.Pending
				                    };
			this.eventRepositoryMock
				.Setup(
					m => m.CreateAsync(
						It.Is<Event>(
							e => e.HourId == eventA.HourId && e.SourceId == eventA.SourceId && e.SourceType == eventA.SourceType)))
				.ReturnsAsync(createdEventA);
			this.databaseServiceMock.Setup(m => m.UpdateTrackedDatabasesAsync()).ReturnsAsyncDefault();

			// Act
			await this.service.CreateEventDataAsync();

			// Assert
			this.hourTestDataRepositoryMock.VerifyAll();
			this.categoryRepositoryMock.VerifyAll();
			this.eventRepositoryMock.VerifyAll();
			this.databaseServiceMock.VerifyAll();
		}
	}
}
