namespace kCura.PDB.Service.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Testing;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class IntegrationTestEventSourceServiceTests
	{
		private IntegrationTestEventSourceService service;
		private Mock<ITestEventTypeProvider> testEventTypeProviderMock;
		private Mock<IEventRepository> eventRepositoryMock;
		private Mock<IConfigurationRepository> configurationRepositoryMock;
		private Mock<ICategoryRepository> categoryRepositoryMock;

		[SetUp]
		public void Setup()
		{
			this.testEventTypeProviderMock = new Mock<ITestEventTypeProvider>();
			this.eventRepositoryMock = new Mock<IEventRepository>();
			this.configurationRepositoryMock = new Mock<IConfigurationRepository>();
			this.categoryRepositoryMock = new Mock<ICategoryRepository>();
			this.service = new IntegrationTestEventSourceService(
				this.testEventTypeProviderMock.Object,
				this.eventRepositoryMock.Object,
				new MockQueuingService(), 
				this.configurationRepositoryMock.Object,
				this.categoryRepositoryMock.Object);
		}

		[Test]
		public async Task CreateHourProcessingEvents()
		{
			// Arrange
			var testTask = Task.FromResult(new Event());
			this.eventRepositoryMock
				.Setup(m => m.CreateAsync(It.Is<Event>(e => e.SourceType == EventSourceType.FindNextCategoriesToScore)))
				.Returns(testTask);

			// Act
			var result = this.service.CreateHourProcessingEvents();

			// Assert
			Assert.That(result, Is.EqualTo(testTask));
			this.testEventTypeProviderMock.VerifyAll();
			this.eventRepositoryMock.VerifyAll();
			this.configurationRepositoryMock.VerifyAll();
			this.categoryRepositoryMock.VerifyAll();
		}

		[Test]
		public async Task EnqueueTasksForPendingEvents()
		{
			var typesToEnqueue = new List<EventSourceType>();
			this.testEventTypeProviderMock.Setup(m => m.GetEventTypesToEnqueue()).Returns(typesToEnqueue);
			var typesToInclude = EventConstants.AllEventTypes.Except(typesToEnqueue).ToList();
			var eventsIdsToReturn = new List<long>();
			this.eventRepositoryMock
				.Setup(
					m => m.ReadIdsByStatusAsync(
						EventStatus.Pending,
						5000,
						It.Is<List<EventSourceType>>(l => l.Count == typesToInclude.Count))).ReturnsAsync(eventsIdsToReturn);

			var eventToReturnA = new Event { SourceId = 1, SourceType = EventSourceType.ScoreCategoryScore };

			var eventsToReturn = new List<Event> { eventToReturnA };
			this.eventRepositoryMock.Setup(m => m.UpdateStatusAsync(eventsIdsToReturn, EventStatus.PendingHangfire))
				.ReturnsAsync(eventsToReturn);

			// Act
			await this.service.EnqueueTasksForPendingEvents();

			// Assert
			this.testEventTypeProviderMock.VerifyAll();
			this.eventRepositoryMock.VerifyAll();
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
