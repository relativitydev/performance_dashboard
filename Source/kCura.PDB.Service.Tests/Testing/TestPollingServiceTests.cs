namespace kCura.PDB.Service.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Testing;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class TestPollingServiceTests
	{
		private TestPollingService service;

		private Mock<IEventRepository> eventRepositoryMock;
		private Mock<IHourTestDataRepository> hourTestDataRepositoryMock;
		private Mock<ITestEventTypeProvider> testEventTypeProviderMock;

		[SetUp]
		public void Setup()
		{
			this.eventRepositoryMock = new Mock<IEventRepository>();
			this.hourTestDataRepositoryMock = new Mock<IHourTestDataRepository>();
			this.testEventTypeProviderMock = new Mock<ITestEventTypeProvider>();
			this.service = new TestPollingService(
				this.eventRepositoryMock.Object,
				this.hourTestDataRepositoryMock.Object,
				this.testEventTypeProviderMock.Object);
		}

		[Test]
		[Ignore("Takes too long for TeamCity")]
		public async Task GetWaitUntilEventCompletionTask()
		{
			var testCancellationToken = new CancellationToken();
			var eventSourceTypeToWatch = EventSourceType.CompleteCategory;
			this.testEventTypeProviderMock.Setup(m => m.GetEventTypeToComplete()).Returns(eventSourceTypeToWatch);

			var hourA = new Hour { Id = 2, HourTimeStamp = DateTime.UtcNow.NormilizeToHour() };
			var hours = new List<Hour> { hourA };
			this.hourTestDataRepositoryMock.Setup(m => m.ReadHoursAsync()).ReturnsAsync(hours);
			this.eventRepositoryMock
				.SetupSequence(m => m.ExistsAsync(It.Is<int>(i => i == hourA.Id), eventSourceTypeToWatch, EventStatus.Completed))
				.ReturnsAsync(false).ReturnsAsync(true);

			// Act
			await this.service.WaitUntilEventCompletionAsync(testCancellationToken);

			// Assert
			this.testEventTypeProviderMock.VerifyAll();
			this.hourTestDataRepositoryMock.VerifyAll();
			this.eventRepositoryMock.VerifyAll();
		}
	}
}
