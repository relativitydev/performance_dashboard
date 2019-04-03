using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.PDB.Core.Constants;
using kCura.PDB.Core.Interfaces.Repositories;
using kCura.PDB.Core.Models;
using Moq;
using NUnit.Framework;

namespace kCura.PDB.Service.Tests.Logic.Metrics
{
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Metrics;
	using kCura.PDB.Tests.Common;


	[TestFixture, Category("Integration")]
	public class MetricServiceIntTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			hourRepository = new HourRepository(this.connectionFactory);

			this.metricRepository = new MetricRepository(this.connectionFactory);
			this.serverRepository = new ServerRepository(this.connectionFactory);
			this.metricDataRepository = new MetricDataRepository(this.connectionFactory);
			this.categoryScoreRepository = new CategoryScoreRepository(this.connectionFactory);
			this.categoryRepository = new CategoryRepository(this.connectionFactory);

			this.metricService = new MetricService(this.serverRepository, this.metricRepository, this.metricDataRepository, this.categoryRepository, this.categoryScoreRepository);
		}

		private IConnectionFactory connectionFactory;
		private MetricRepository metricRepository;
		private HourRepository hourRepository;
		private ServerRepository serverRepository;
		private MetricDataRepository metricDataRepository;
		private CategoryScoreRepository categoryScoreRepository;
		private CategoryRepository categoryRepository;
		private MetricService metricService;

		[Test]
		public async Task CreateHours()
		{
			var hourCount = 168;
			var hours = await Enumerable.Range(0, hourCount)
				.Select(CreateHour)
				.WhenAllStreamed();

			//var metrics = (await hours.Select(h => this.metricService.CreateMetricsForHour(h.Id))
			//	.WhenAllStreamed(hourCount)).SelectMany(m => m);

			//Assert.That(metrics.Count, Is.EqualTo(hourCount * MetricConstants.ActiveMetricTypes.Length));

			Assert.Fail("Not Implemented");
		}

		internal Task<Hour> CreateHour(int hour) =>
			this.hourRepository.CreateAsync(new Hour { HourTimeStamp = DateTime.UtcNow.NormilizeToHour().AddHours(-hour) });
	}

	[TestFixture, Category("Unit")]
	public class MetricServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.serverRepository = new Mock<IServerRepository>();
			this.metricRepository = new Mock<IMetricRepository>();
			this.metricDataRepository = new Mock<IMetricDataRepository>();
			this.categoryScoreRepository = new Mock<ICategoryScoreRepository>();
			this.categoryRepository = new Mock<ICategoryRepository>();
			this.metricService = new MetricService(
				this.serverRepository.Object,
				this.metricRepository.Object,
				this.metricDataRepository.Object,
				this.categoryRepository.Object,
				this.categoryScoreRepository.Object);
		}

		private Mock<IServerRepository> serverRepository;
		private Mock<IMetricRepository> metricRepository;
		private Mock<IMetricDataRepository> metricDataRepository;
		private Mock<ICategoryScoreRepository> categoryScoreRepository;
		private Mock<ICategoryRepository> categoryRepository;
		private MetricService metricService;

		[Test]
		public async Task CreateCategoriesForHour()
		{
			// Arrange
			var metricTypeCount = MetricConstants.CategoryTypesToMetricTypes[CategoryType.Uptime].Count();
			this.metricRepository.Setup(r => r.CreateAsync(It.IsAny<Metric>())).ReturnsAsync(new Metric { Id = 444 });

			// Act
			var results = await this.metricService.CreateMetricsForHour(123, CategoryType.Uptime);

			// Assert
			this.metricRepository.Verify(mr => mr.CreateAsync(It.IsAny<Metric>()), Times.Exactly(metricTypeCount));
			Assert.That(results.Count, Is.EqualTo(metricTypeCount));
		}

		[Test]
		public async Task CreateMetricDatasForCategoryScores_ServerCategory()
		{
			// Arrange
			this.serverRepository.Setup(s => s.ReadAsync(321)).ReturnsAsync(new Server { ServerId = 321, ServerType = ServerType.Database });
			this.metricRepository.Setup(r => r.CreateAsync(It.IsAny<Metric>())).ReturnsAsync(new Metric { Id = 444, MetricType = MetricType.Ram });
			this.metricDataRepository.Setup(r => r.CreateAsync(It.IsAny<MetricData>())).ReturnsAsync(new MetricData { Id = 555 });
			this.categoryScoreRepository.Setup(r => r.ReadAsync(888)).ReturnsAsync(new CategoryScore { CategoryId = 1234, ServerId = 321 });
			this.categoryRepository.Setup(r => r.ReadAsync(1234)).ReturnsAsync(new Category { Id = 1234, HourId = 999, CategoryType = CategoryType.InfrastructurePerformance});

			// Act
			var results = await this.metricService.CreateMetricDatasForCategoryScores(888);

			//Assert
			Assert.That(results.Count, Is.EqualTo(
				MetricConstants.ActiveMetricTypes
				.Intersect(MetricConstants.CategoryTypesToMetricTypes[CategoryType.InfrastructurePerformance])
				.Count()));
		}

		[Test]
		public async Task CreateMetricDatasForCategoryScores_ServerlessCategory()
		{
			// Arrange
			this.metricRepository.Setup(r => r.CreateAsync(It.IsAny<Metric>()))
				.ReturnsAsync(new Metric { Id = 444, MetricType = MetricType.AgentUptime });
			this.metricDataRepository.Setup(r => r.CreateAsync(It.IsAny<MetricData>()))
				.ReturnsAsync(new MetricData { Id = 555 });
			this.categoryScoreRepository.Setup(r => r.ReadAsync(888))
				.ReturnsAsync(new CategoryScore { CategoryId = 1234, ServerId = null });
			this.categoryRepository.Setup(r => r.ReadAsync(1234))
				.ReturnsAsync(new Category { Id = 1234, HourId = 999, CategoryType = CategoryType.Uptime });

			// Act
			var results = await this.metricService.CreateMetricDatasForCategoryScores(888);

			//Assert
			Assert.That(results.Count, Is.EqualTo(
				MetricConstants.ActiveMetricTypes
				.Intersect(MetricConstants.CategoryTypesToMetricTypes[CategoryType.Uptime])
				.Count()));
		}
	}
}
