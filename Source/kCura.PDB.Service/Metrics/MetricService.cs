namespace kCura.PDB.Service.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;

	public class MetricService : IMetricService
	{
		public MetricService(
			IServerRepository serverRepository,
			IMetricRepository metricRepository,
			IMetricDataRepository metricDataRepository,
			ICategoryRepository categoryRepository,
			ICategoryScoreRepository categoryScoreRepository)
		{
			this.serverRepository = serverRepository;
			this.metricRepository = metricRepository;
			this.metricDataRepository = metricDataRepository;
			this.categoryRepository = categoryRepository;
			this.categoryScoreRepository = categoryScoreRepository;
		}

		private readonly IMetricRepository metricRepository;
		private readonly IMetricDataRepository metricDataRepository;
		private readonly ICategoryScoreRepository categoryScoreRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly IServerRepository serverRepository;

		public async Task<IList<int>> CreateMetricDatasForCategoryScores(int categoryScoreId)
		{
			var categoryScore = await this.categoryScoreRepository.ReadAsync(categoryScoreId);
			var category = await this.categoryRepository.ReadAsync(categoryScore.CategoryId);

			// Create the metrics
			var metrics = await this.CreateMetricsForHour(category.HourId, category.CategoryType);

			// Read the metrics that don't have metric datas/scores created for them
			if (categoryScore.ServerId.HasValue)
			{
				var server = await this.serverRepository.ReadAsync(categoryScore.ServerId.Value);
				return await metrics
					.Where(m => ServerTypeMapper.GetServerTypes(m.MetricType).Contains(server.ServerType))
					.Select(m => this.CreateMetricDatasForMetric(m.Id, server.ServerId))
					.WhenAllStreamed();
			}
			else
			{
				return await metrics
					.Select(m => this.CreateMetricDatasForMetric(m.Id))
					.WhenAllStreamed();
			}
		}

		internal async Task<int> CreateMetricDatasForMetric(int metricId, int? serverId = null)
		{
			var metricData = await this.metricDataRepository.CreateAsync(new MetricData { MetricId = metricId, ServerId = serverId });
			return metricData.Id;
		}

		internal async Task<IList<Metric>> CreateMetricsForHour(int hourId, CategoryType categoryType)
		{
			var metrics = await MetricConstants.ActiveMetricTypes
				.Where(mt => MetricConstants.CategoryTypesToMetricTypes[categoryType].Contains(mt))
				.Select(mt => new Metric { HourId = hourId, MetricType = mt })
				.Select(m => this.metricRepository.CreateAsync(m))
				.WhenAllStreamed(2);
			return metrics.ToList();
		}
	}
}
