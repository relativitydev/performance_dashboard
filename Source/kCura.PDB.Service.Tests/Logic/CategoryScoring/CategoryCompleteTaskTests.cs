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
	public class CategoryCompleteTaskTests
	{
		[SetUp]
		public void Setup()
		{
			categoryCompleteFactory = new Mock<IServiceFactory<ICategoryCompleteLogic, CategoryType>>();
			categoryScoreRepository = new Mock<ICategoryScoreRepository>();
			categoryRepository = new Mock<ICategoryRepository>();
			logger = TestUtilities.GetMockLogger();
			categoryCompleteTask = new CategoryCompleteTask(
				categoryScoreRepository.Object,
				categoryRepository.Object,
				categoryCompleteFactory.Object,
				logger.Object);
		}

		private Mock<IServiceFactory<ICategoryCompleteLogic, CategoryType>> categoryCompleteFactory;
		private Mock<ICategoryScoreRepository> categoryScoreRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<ILogger> logger;
		private CategoryCompleteTask categoryCompleteTask;

		[Test]
		public async Task CategoryCompleteTask_CompleteCategory_AllScored()
		{
			// Arrange
			var categoryScoreId = 123;
			var categoryId = 444;
			var categoryScore = new CategoryScore { CategoryId = categoryId, Score = 100, Id = categoryScoreId };
			var categoryScores = new[] { categoryScore };
			var category = new Category { Id = categoryId, CategoryType = CategoryType.Uptime };
			categoryScoreRepository.Setup(csr => csr.ReadAsync(category)).ReturnsAsync(categoryScores);
			categoryRepository.Setup(r => r.ReadAsync(categoryScore.CategoryId)).ReturnsAsync(category);
			var mockCategoryCompleteLogic = new Mock<ICategoryCompleteLogic>();
			mockCategoryCompleteLogic.Setup(l => l.CompleteCategory(category)).Returns(Task.FromResult(0));
			this.categoryCompleteFactory.Setup(mf => mf.GetService(CategoryType.Uptime)).Returns(mockCategoryCompleteLogic.Object);

			// Act
			var result = await categoryCompleteTask.CompleteCategory(categoryId);

			// Assert
			this.categoryScoreRepository.VerifyAll();
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task CategoryCompleteTask_CompleteCategory_NotAllScored()
		{
			// Arrange
			var categoryScoreId = 123;
			var categoryId = 444;
			var categoryScore = new CategoryScore { CategoryId = categoryId, Score = null, Id = categoryScoreId };
			var categoryScores = new[] { categoryScore };
			var category = new Category { Id = categoryId };
			categoryScoreRepository.Setup(csr => csr.ReadAsync(category)).ReturnsAsync(categoryScores);
			categoryRepository.Setup(r => r.ReadAsync(categoryScore.CategoryId)).ReturnsAsync(category);

			// Act
			var result = await categoryCompleteTask.CompleteCategory(categoryId);

			// Assert
			this.categoryScoreRepository.VerifyAll();
			this.categoryCompleteFactory.VerifyAll();
			Assert.That(result, Is.False);
		}

		[Test]
		public void CategoryCompleteTask_CompleteCategory_Error()
		{
			// Arrange// Arrange
			var categoryScoreId = 123;
			var categoryId = 444;
			var categoryScore = new CategoryScore { CategoryId = categoryId, Score = 100, Id = categoryScoreId };
			var categoryScores = new[] { categoryScore };
			var category = new Category { Id = categoryId, CategoryType = CategoryType.Uptime };
			categoryScoreRepository.Setup(csr => csr.ReadAsync(category)).ReturnsAsync(categoryScores);
			categoryRepository.Setup(r => r.ReadAsync(categoryScore.CategoryId)).ReturnsAsync(category);
			var mockCategoryCompleteLogic = new Mock<ICategoryCompleteLogic>();
			var testException = new Exception("TestException");
			mockCategoryCompleteLogic.Setup(l => l.CompleteCategory(category)).Throws(testException);
			this.categoryCompleteFactory.Setup(mf => mf.GetService(CategoryType.Uptime)).Returns(mockCategoryCompleteLogic.Object);

			// Act
			var result = Assert.ThrowsAsync<Exception>(() => categoryCompleteTask.CompleteCategory(categoryId));

			// Assert
			this.categoryScoreRepository.VerifyAll();
			Assert.That(result, Is.EqualTo(testException));
		}

		[Test]
		public async Task CategoryCompleteTask_CompleteCategory_NoImplementation()
		{
			// Arrange
			var categoryScoreId = 123;
			var categoryId = 444;
			var categoryScore = new CategoryScore { CategoryId = categoryId, Score = 100, Id = categoryScoreId };
			var categoryScores = new[] { categoryScore };
			var category = new Category { Id = categoryId, CategoryType = CategoryType.Uptime };
			categoryScoreRepository.Setup(csr => csr.ReadAsync(category)).ReturnsAsync(categoryScores);
			categoryRepository.Setup(r => r.ReadAsync(categoryScore.CategoryId)).ReturnsAsync(category);
			this.categoryCompleteFactory.Setup(mf => mf.GetService(CategoryType.Uptime)).Returns((ICategoryCompleteLogic)null);

			// Act
			var result = await categoryCompleteTask.CompleteCategory(categoryId);

			// Assert
			this.categoryScoreRepository.VerifyAll();
			this.logger.Verify(l => l.LogError(It.IsAny<string>(), (string)null), Times.Once);
			Assert.That(result, Is.True);
		}
	}
}
