namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Hours;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class HourMigrationServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.hourRepository = new Mock<IHourRepository>();
			this.eventRepository = new Mock<IEventRepository>();
			this.sampleHistoryRepository = new Mock<ISampleHistoryRepository>();
			this.hourMigrationService = new HourMigrationService(this.hourRepository.Object, this.eventRepository.Object, this.sampleHistoryRepository.Object);
		}

		private Mock<IHourRepository> hourRepository;
		private Mock<IEventRepository> eventRepository;
		private Mock<ISampleHistoryRepository> sampleHistoryRepository;
		private HourMigrationService hourMigrationService;

		[Test]
		public async Task IdentifyIncompleteHours()
		{
			// Arrange
			var list = new[] { 1, 2, 3, 4 };
			this.hourRepository.Setup(r => r.ReadIncompleteHoursAsync())
				.ReturnsAsync(list);

			// Act
			var result = await this.hourMigrationService.IdentifyIncompleteHours();

			// Assert
			Assert.That(result, Is.EqualTo(list));
		}

		[Test]
		public async Task CancelEvents()
		{
			// Arrange
			this.eventRepository.Setup(r => r.CancelEventsAsync(It.IsAny<IList<EventStatus>>(), It.IsAny<IList<EventSourceType>>()))
				.Returns(Task.FromResult(1));

			// Act
			await this.hourMigrationService.CancelEvents();

			// Assert
			this.eventRepository.Verify(r => r.CancelEventsAsync(It.IsAny<IList<EventStatus>>(), It.IsAny<IList<EventSourceType>>()));
		}

		[Test]
		public async Task CancelHour()
		{
			// Arrange
			var hourId = 1234;
			this.hourRepository.Setup(r => r.ReadAsync(hourId))
				.ReturnsAsync(new Hour { Id = hourId, Status = HourStatus.Pending });
			this.hourRepository.Setup(r => r.UpdateAsync(It.IsAny<Hour>()))
				.Returns(Task.FromResult(1));
			this.sampleHistoryRepository.Setup(r => r.RemoveHourFromSampleAsync(hourId))
				.Returns(Task.FromResult(1));

			// Act
			await this.hourMigrationService.CancelHour(hourId);

			// Assert
			this.hourRepository.Verify(r => r.UpdateAsync(It.IsAny<Hour>()));
		}
	}
}
