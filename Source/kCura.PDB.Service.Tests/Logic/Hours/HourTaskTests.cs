namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using Moq;
	using NUnit.Framework;
	using Core.Interfaces.Services;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Service.Hours;

	[TestFixture]
	[Category("Unit")]
	public class HourTaskTests
	{
		[SetUp]
		public void Setup()
		{
			this.hourRepository = new Mock<IHourRepository>();
			this.hourlyScoringLogic = new Mock<IHourlyScoringLogic>();
			this.ratingsRepository = new Mock<IRatingsRepository>();
			this.eventRepository = new Mock<IEventRepository>();
			this.hourTask = new HourTask(hourRepository.Object, hourlyScoringLogic.Object, this.ratingsRepository.Object, this.eventRepository.Object);
		}

		private Mock<IHourRepository> hourRepository;
		private Mock<IHourlyScoringLogic> hourlyScoringLogic;
		private Mock<ILogger> logger;
		private Mock<IRatingsRepository> ratingsRepository;
		private Mock<IEventRepository> eventRepository;
		private HourTask hourTask;

		[Test]
		public async Task MetricHourTask_ScoreHour()
		{
			// Arrange
			var hourId = 444;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			var hourScore = 98m;
			this.ratingsRepository.Setup(m => m.Exists(hourId)).ReturnsAsync(false);
			hourRepository.Setup(r => r.ReadAsync(hourId)).ReturnsAsync(hour);
			hourlyScoringLogic.Setup(l => l.ScoreHour(hour)).ReturnsAsync(hourScore);
			hourRepository.Setup(r => r.UpdateAsync(It.Is<Hour>(h=>h.Id == hour.Id && h.HourTimeStamp == hour.HourTimeStamp && h.Score == hourScore))).Returns(Task.Delay(100));

			// Act
			await this.hourTask.ScoreHour(hourId);

			// Assert
			hourRepository.Verify(r => r.UpdateAsync(It.Is<Hour>(h => h.Score == 98)));
		}

		[Test]
		public async Task HourTask_CheckIfHourReadyToScore()
		{
			// Arrange
			var hourId = 444;
			var hour = new Hour {Id = hourId, HourTimeStamp = DateTime.UtcNow};
			this.ratingsRepository.Setup(m => m.Exists(hourId)).ReturnsAsync(false);
			this.hourRepository.Setup(r => r.ReadHourReadyForScoringAsync(hourId)).ReturnsAsync(hour);

			// Act
			var result = await this.hourTask.CheckIfHourReadyToScore(hourId);

			// Assert
			Assert.That(result.Types.First(), Is.EqualTo(EventSourceType.ScoreHour));
		}

		[Test]
		public async Task HourTask_CheckIfHourReadyToScore_NotReady()
		{
			// Arrange
			var hourId = 444;
			this.ratingsRepository.Setup(m => m.Exists(hourId)).ReturnsAsync(false);
			this.hourRepository.Setup(r => r.ReadHourReadyForScoringAsync(hourId)).ReturnsAsync((Hour)null);

			// Act
			var result = await this.hourTask.CheckIfHourReadyToScore(hourId);

			// Assert
			Assert.That(result.ShouldContinue, Is.False);
		}

		[Test]
		public void HourTask_CheckIfHourReadyToScore_RatingsException()
		{
			// Arrange
			var hourId = 444;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			this.hourRepository.Setup(r => r.ReadHourReadyForScoringAsync(hourId)).ReturnsAsync(hour);
			this.ratingsRepository.Setup(m => m.Exists(hourId)).ReturnsAsync(true);

			// Act
			// Assert
			var exception = Assert.ThrowsAsync<Exception>(() => this.hourTask.CheckIfHourReadyToScore(hourId));
			Assert.That(exception.Message.StartsWith("Ratings"));
		}

		[Test]
		public void HourTask_VerifyPrerequisites_RatingsException()
		{
			// Arrange
			var hourId = 444;
			var hour = new Hour { Id = hourId, HourTimeStamp = DateTime.UtcNow };
			this.hourRepository.Setup(r => r.ReadHourReadyForScoringAsync(hourId)).ReturnsAsync(hour);
			this.ratingsRepository.Setup(m => m.Exists(hourId)).ReturnsAsync(true);

			// Act
			// Assert
			var exception = Assert.ThrowsAsync<Exception>(() => this.hourTask.CheckIfHourReadyToScore(hourId));
			Assert.That(exception.Message.StartsWith("Ratings"));
		}
	}
}
