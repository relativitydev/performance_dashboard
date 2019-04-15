namespace kCura.PDB.Service.Tests.Logic.CategoryScoring
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::Relativity.Toggles;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Toggles;
	using kCura.PDB.Service.CategoryScoring;

	using Moq;

	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class CategoryServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.categoryRepository = new Mock<ICategoryRepository>();
			this.categoryScoreRepository = new Mock<ICategoryScoreRepository>();
			this.toggleProviderMock = new Mock<IToggleProvider>();
			this.service = new CategoryService(
				this.serverRepository.Object,
				this.categoryRepository.Object,
				this.categoryScoreRepository.Object,
				this.toggleProviderMock.Object);
			this.servers = new List<Server>()
			{
				new Server { ServerIpAddress = "123", ServerId = 123, ServerType = ServerType.Database },
				new Server { ServerIpAddress = "234", ServerId = 234, ServerType = ServerType.Database }
			};
		}

		private Mock<IServerRepository> serverRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private Mock<ICategoryScoreRepository> categoryScoreRepository;
		private Mock<IToggleProvider> toggleProviderMock;

		private CategoryService service;
		private List<Server> servers;

		[Test]
		public async Task CreateCategoriesForHour_ToggleOn()
		{
			// Arrange
			var categoryTypeCount = MetricConstants.ActiveCategoryTypes.Count();
			this.categoryRepository.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(new Category { Id = 444 });
			this.toggleProviderMock.Setup(m => m.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>()).ReturnsAsync(true);

			// Act
			var results = await this.service.CreateCategoriesForHour(123);

			// Assert
			this.categoryRepository.Verify(mr => mr.CreateAsync(It.IsAny<Category>()), Times.Exactly(categoryTypeCount));
			Assert.That(results.Count, Is.EqualTo(categoryTypeCount));
		}

		[Test]
		public async Task CreateCategoriesForHour_ToggleOff()
		{
			// Arrange
			var categoryTypeCount = MetricConstants.ActiveCategoryTypes.Count() - 1;
			this.categoryRepository.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(new Category { Id = 444 });
			this.toggleProviderMock.Setup(m => m.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>()).ReturnsAsync(false);

			// Act
			var results = await this.service.CreateCategoriesForHour(123);

			// Assert
			this.categoryRepository.Verify(mr => mr.CreateAsync(It.IsAny<Category>()), Times.Exactly(categoryTypeCount));
			Assert.That(results.Count, Is.EqualTo(categoryTypeCount));
		}

		[Test]
		public async Task CreateCategoryScoresForCategory_ServerCategory()
		{
			// Arrange
			this.categoryRepository.Setup(r => r.ReadAsync(444)).ReturnsAsync(new Category { Id = 444, CategoryType = CategoryType.UserExperience });
			this.serverRepository.Setup(s => s.ReadAllActiveAsync()).ReturnsAsync(servers);
			this.categoryScoreRepository.Setup(r => r.CreateAsync(It.IsAny<CategoryScore>())).ReturnsAsync(new CategoryScore { Id = 555 });

			// Act
			await this.service.CreateCategoryScoresForCategory(444);

			// Assert
			this.serverRepository.Verify(s => s.ReadAllActiveAsync(), Times.Once);
			this.categoryScoreRepository.Verify(r => r.CreateAsync(It.IsAny<CategoryScore>()), Times.Exactly(servers.Count));
		}

		[Test]
		public async Task CreateCategoryScoresForCategory_ServerlessCategory()
		{
			// Arrange
			this.categoryRepository.Setup(r => r.ReadAsync(444)).ReturnsAsync(new Category { Id = 444, CategoryType = CategoryType.Uptime });
			this.serverRepository.Setup(s => s.ReadAllActiveAsync()).ReturnsAsync(servers);
			this.categoryScoreRepository.Setup(r => r.CreateAsync(It.IsAny<CategoryScore>())).ReturnsAsync(new CategoryScore { Id = 555 });

			// Act
			await this.service.CreateCategoryScoresForCategory(444);

			// Assert
			this.serverRepository.Verify(s => s.ReadAllActiveAsync(), Times.Never);
			this.categoryScoreRepository.Verify(r => r.CreateAsync(It.IsAny<CategoryScore>()), Times.Once);
		}
	}
}
