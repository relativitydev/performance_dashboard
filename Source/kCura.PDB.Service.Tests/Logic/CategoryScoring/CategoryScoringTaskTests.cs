namespace kCura.PDB.Service.Tests.Logic.CategoryScoring
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Service.CategoryScoring;
	using Moq;
	using NUnit.Framework;
	using kCura.PDB.Tests.Common;

	[TestFixture]
	[Category("Unit")]
	public class CategoryScoringTaskTests
	{
		[SetUp]
		public void Setup()
		{
			categoryScoreFactory = new Mock<IServiceFactory<ICategoryScoringLogic, CategoryType>>();
			categoryScoreRepository = new Mock<ICategoryScoreRepository>();
			categoryRepository = new Mock<ICategoryRepository>();
			metricDataService = new Mock<IMetricDataService>();
			serverRepository = new Mock<IServerRepository>();
			mockCategoryScoringLogic = new Mock<ICategoryScoringLogic>();
			logger = TestUtilities.GetMockLogger();

			// Arrange Category Data
			var categoryScore = new CategoryScore { CategoryId = CategoryScoreId };
			var category = new Category { HourId = HourId, CategoryType = CategoryType.Uptime, Id = CategoryId };
			var metricDatas = new List<MetricData>() { new MetricData { MetricId = 888 }, new MetricData { MetricId = 888 } };
			categoryScoreRepository.Setup(csr => csr.ReadAsync(CategoryId)).ReturnsAsync(categoryScore);
			categoryScoreRepository.Setup(r => r.ReadAsync(CategoryId)).ReturnsAsync(categoryScore);
			categoryRepository.Setup(r => r.ReadAsync(CategoryScoreId)).ReturnsAsync(category);
			mockCategoryScoringLogic.Setup(l => l.ScoreMetrics(It.IsAny<CategoryScore>(), It.IsAny<IList<MetricData>>())).ReturnsAsync(89.0m);
			categoryScoreRepository.Setup(r => r.UpdateAsync(It.Is<CategoryScore>(cs => cs.Score == 89.0m))).Returns(Task.Delay(50));
			metricDataService.Setup(s => s.ReadMetricDatasByCategoryScoreAsync(It.IsAny<CategoryScore>())).ReturnsAsync(metricDatas);
			hourRepository = new Mock<IHourRepository>();
			eventRepository = new Mock<IEventRepository>();
			categoryScoringTask = new CategoryScoringTask(
				categoryScoreFactory.Object,
				categoryScoreRepository.Object,
				categoryRepository.Object,
				serverRepository.Object,
				metricDataService.Object,
				hourRepository.Object,
				eventRepository.Object,
				logger.Object);
		}

		private Mock<IServiceFactory<ICategoryScoringLogic, CategoryType>> categoryScoreFactory;
		private Mock<ICategoryScoreRepository> categoryScoreRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private Mock<IServerRepository> serverRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<ICategoryScoringLogic> mockCategoryScoringLogic;
		private Mock<IHourRepository> hourRepository;
		private Mock<IEventRepository> eventRepository;
		private Mock<ILogger> logger;
		private CategoryScoringTask categoryScoringTask;

	    private const int HourId = 555;
        private const int CategoryScoreId = 444;
        private const int CategoryId = 123;

		[Test]
		public async Task CategoryScoringTask_ScoreCategory()
		{
			// Arrange
			this.categoryScoreFactory.Setup(mf => mf.GetService(CategoryType.Uptime)).Returns(mockCategoryScoringLogic.Object);

			// Act
			var result = await categoryScoringTask.ScoreCategory(CategoryId);

			// Assert
			this.categoryScoreRepository.Verify(r => r.UpdateAsync(It.Is<CategoryScore>(cs => cs.Score == 89.0m)));
			Assert.That(result, Is.EqualTo(CategoryId));
		}

		[Test]
		public void CategoryScoringTask_ScoreCategory_Error()
		{
			// Arrange
			this.categoryScoreRepository.Setup(r => r.ReadAsync(HourId)).Throws(new Exception(" test exception"));

			// Act & Assert
			Assert.ThrowsAsync<Exception>(() => categoryScoringTask.ScoreCategory(HourId));
		}

		[Test]
		public async Task CategoryScoringTask_ScoreCategory_NoImplementation()
		{
			// Arrange
			this.categoryScoreFactory.Setup(mf => mf.GetService(CategoryType.Uptime)).Returns((ICategoryScoringLogic)null);

			//Act
			var result = await categoryScoringTask.ScoreCategory(CategoryId);

			//Assert
			this.logger.Verify(l => l.LogVerboseAsync(It.IsAny<string>(), It.IsAny<List<string>>()));
			mockCategoryScoringLogic.Verify(cl => cl.ScoreMetrics(It.IsAny<CategoryScore>(), It.IsAny<IList<MetricData>>()), Times.Never());
			Assert.That(result, Is.EqualTo(CategoryId));
		}

		[Test]
		public async Task CategoryScoringTask_FindNextCategoriesToScore()
		{
			// Arrange
			this.hourRepository.Setup(r => r.ReadNextHourWithoutRatings())
				.ReturnsAsync(new Hour());
			this.categoryScoreRepository.Setup(r => r.ReadAsync(It.IsAny<Hour>()))
				.ReturnsAsync(new[] { new CategoryScore(), new CategoryScore() });
			this.eventRepository.Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
			this.metricDataService.Setup(r => r.ReadMetricDatasByCategoryScoreAsync(It.IsAny<CategoryScore>()))
				.ReturnsAsync(new[] { new MetricData { Score = 99 }, new MetricData { Score = 99 } });

			//Act
			var result = await categoryScoringTask.FindNextCategoriesToScore();

			//Assert
			Assert.That(result.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task CategoryScoringTask_FindNextCategoriesToScore_MetricDatasNotScored()
		{
			// Arrange
			this.hourRepository.Setup(r => r.ReadNextHourWithoutRatings())
				.ReturnsAsync(new Hour());
			this.categoryScoreRepository.Setup(r => r.ReadAsync(It.IsAny<Hour>()))
				.ReturnsAsync(new[] { new CategoryScore(), new CategoryScore() });
			this.eventRepository.Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
			this.metricDataService.Setup(r => r.ReadMetricDatasByCategoryScoreAsync(It.IsAny<CategoryScore>()))
				.ReturnsAsync(new[] { new MetricData { Score = null }, new MetricData { Score = null } });

			//Act
			var result = await categoryScoringTask.FindNextCategoriesToScore();

			//Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task CategoryScoringTask_FindNextCategoriesToScore_AlreadyStartedScoring()
		{
			// Arrange
			this.hourRepository.Setup(r => r.ReadNextHourWithoutRatings())
				.ReturnsAsync(new Hour());
			this.categoryScoreRepository.Setup(r => r.ReadAsync(It.IsAny<Hour>()))
				.ReturnsAsync(new[] { new CategoryScore(), new CategoryScore() });
			this.eventRepository.Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
			this.metricDataService.Setup(r => r.ReadMetricDatasByCategoryScoreAsync(It.IsAny<CategoryScore>()))
				.ReturnsAsync(new[] { new MetricData { Score = null }, new MetricData { Score = null } });

			//Act
			var result = await categoryScoringTask.FindNextCategoriesToScore();

			//Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task CategoryScoringTask_FindNextCategoriesToScore_NoHour()
		{
			// Arrange
			this.hourRepository.Setup(r => r.ReadNextHourWithoutRatings())
				.ReturnsAsync((Hour)null);
			this.categoryScoreRepository.Setup(r => r.ReadAsync(It.IsAny<Hour>()))
				.ReturnsAsync(new[] { new CategoryScore(), new CategoryScore() });
			this.eventRepository.Setup(m => m.ExistsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
			this.metricDataService.Setup(r => r.ReadMetricDatasByCategoryScoreAsync(It.IsAny<CategoryScore>()))
				.ReturnsAsync(new[] { new MetricData { Score = null }, new MetricData { Score = null } });

			//Act
			var result = await categoryScoringTask.FindNextCategoriesToScore();

			//Assert
			Assert.That(result, Is.Empty);
		}
	}
}
