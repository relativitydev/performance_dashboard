namespace kCura.PDB.Service.Tests.Logic.Hours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Models;
	using Moq;
	using NUnit.Framework;
	using Core.Interfaces.Services;
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Service.Hours;
	using kCura.PDB.Tests.Common;
	using kCura.PDB.Tests.Common.Extensions;

	[TestFixture, Category("Unit")]
	public class HourlyScoringLogicTests
	{
		[SetUp]
		public void Setup()
		{
			this.categoryScoreRepository = new Mock<ICategoryScoreRepository>();
			this.categoryRepository = new Mock<ICategoryRepository>();
			this.hourRepository = new Mock<IHourRepository>();
			this.logService = new Mock<ILogService>();
			this.fcmRepository = new Mock<IFcmRepository>();
			this.loggerMock = TestUtilities.GetMockLogger();
			this.toggleProvider = new Mock<IToggleProvider>();
			this.legacyRecoverabilityIntegrityRepository = new Mock<ILegacyRecoverabilityIntegrityRepository>();
			this.hourlyScoringLogic = new HourlyScoringLogic(
				categoryScoreRepository.Object,
				categoryRepository.Object,
				hourRepository.Object,
				logService.Object,
				fcmRepository.Object,
				toggleProvider.Object,
				legacyRecoverabilityIntegrityRepository.Object,
				this.loggerMock.Object);
		}

		private Mock<ICategoryScoreRepository> categoryScoreRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private Mock<IHourRepository> hourRepository;
		private Mock<ILogService> logService;
		private Mock<IFcmRepository> fcmRepository;
		private Mock<ILogger> loggerMock;
		private Mock<IToggleProvider> toggleProvider;
		private Mock<ILegacyRecoverabilityIntegrityRepository> legacyRecoverabilityIntegrityRepository;
		private HourlyScoringLogic hourlyScoringLogic;

		[Test]
		public async Task HourlyScoringLogic_ScoreHour()
		{
			// Arrange
			var hour = new Hour();
			var categoryScores = new[]
			{
				new CategoryScore { CategoryId = 111, Score = 98 },
				new CategoryScore { CategoryId = 222, Score = 94.0m, Category = new Category { CategoryType = CategoryType.RecoverabilityIntegrity } }
			};
			this.categoryScoreRepository.Setup(r => r.ReadAsync(hour)).ReturnsAsync(categoryScores);
			this.categoryRepository.Setup(r => r.ReadAsync(111)).ReturnsAsync(new Category { Id = 111, CategoryType = CategoryType.Uptime });
			this.categoryRepository.Setup(r => r.ReadAsync(222)).ReturnsAsync(new Category { Id = 222, CategoryType = CategoryType.UserExperience });
			this.logService.Setup(s => s.GetLogLevel()).Returns(LogLevel.Verbose);

			this.toggleProvider.Setup(p => p.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>())
				.ReturnsAsync(true);

			this.hourRepository.Setup(r => r.ScoreHourWithBuildAndRateSampleAsync(hour, 94.0m, true)).ReturnsAsyncDefault();

			// Act
			var result = await this.hourlyScoringLogic.ScoreHour(hour);

			// Assert
			Assert.That(result, Is.EqualTo(96));
		}

		[Test]
		public async Task HourlyScoringLogic_GetWeekRecoverabilityIntegrityScore()
		{
			// Arrange
			var hour = new Hour();
			var categoryScores = new[]
			{
				new CategoryScore { Score = 98.0m },
				new CategoryScore { Score = 94.0m },
				new CategoryScore { Score = 87.0m },
			};
			this.toggleProvider.Setup(p => p.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>())
				.ReturnsAsync(true);

			// Act
			var result = await this.hourlyScoringLogic.GetWeekRecoverabilityIntegrityScore(categoryScores, hour);

			// Assert
			Assert.That(result, Is.EqualTo(87.0m));
		}

		[Test]
		public async Task HourlyScoringLogic_GetWeekRecoverabilityIntegrityScore_Legacy()
		{
			// Arrange
			var hour = new Hour();
	
			// this score value should not be used since the value should be read from the legacy recoverability integrity repository
			var categoryScores = new[] { new CategoryScore { Score = 30.0m } };

			this.toggleProvider.Setup(p => p.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>())
				.ReturnsAsync(false);

			this.legacyRecoverabilityIntegrityRepository.Setup(r => r.ReadWeekRecoverabilityIntegrityScore(hour))
				.ReturnsAsync(93.0m);

			// Act
			var result = await this.hourlyScoringLogic.GetWeekRecoverabilityIntegrityScore(categoryScores, hour);

			// Assert
			Assert.That(result, Is.EqualTo(93.0m));
		}
	}
}
