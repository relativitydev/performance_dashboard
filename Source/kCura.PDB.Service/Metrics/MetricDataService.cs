namespace kCura.PDB.Service.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class MetricDataService : IMetricDataService
	{
		public MetricDataService(IMetricRepository metricRepository, IMetricDataRepository metricDataRepository, IHourRepository hourRepository, IServerRepository serverRepository)
		{
			this.metricRepository = metricRepository;
			this.metricDataRepository = metricDataRepository;
			this.hourRepository = hourRepository;
			this.serverRepository = serverRepository;
		}

		private readonly IMetricRepository metricRepository;
		private readonly IMetricDataRepository metricDataRepository;
		private readonly IHourRepository hourRepository;
		private readonly IServerRepository serverRepository;

		public async Task<IList<MetricData>> ReadMetricDatasByCategoryScoreAsync(CategoryScore categoryScore)
		{
			var categoryScoringMetricDatas = await this.metricDataRepository.ReadByCategoryScoreAsync(categoryScore);

			// Query the metrics
			var categoryScoringMetrics = await categoryScoringMetricDatas.Select(md => this.metricRepository.ReadAsync(md.MetricId)).WhenAllStreamed();

			// populate the metrics on the metric datas
			categoryScoringMetricDatas
				.Where(md => categoryScoringMetrics.Any(m => m.Id == md.MetricId))
				.ForEach(md => md.Metric = categoryScoringMetrics.First(m => m.Id == md.MetricId));

			// Query the hours
			var hours = await categoryScoringMetricDatas
				.Where(md => md.Metric != null)
				.GroupBy(md => md.Metric.HourId)
				.Select(mdg => this.hourRepository.ReadAsync(mdg.Key))
				.WhenAllStreamed();

			// populate hours on metrics
			categoryScoringMetricDatas
				.Where(md => md.Metric != null)
				.Where(md => hours.Any(h => h.Id == md.Metric.HourId))
				.ForEach(md => md.Metric.Hour = hours.First(h => h.Id == md.Metric.HourId));

			// Query the servers
			var servers = await categoryScoringMetricDatas
				.Where(md => md.ServerId.HasValue)
				.GroupBy(md => md.ServerId)
				.Select(mdg => this.serverRepository.ReadAsync(mdg.Key.Value))
				.WhenAllStreamed();

			// Populate servers on metric datas
			categoryScoringMetricDatas
				.Where(md => servers.Any(s => s.ServerId == md.ServerId))
				.ForEach(md => md.Server = servers.First(s => s.ServerId == md.ServerId));

			return categoryScoringMetricDatas;
		}

		public async Task<MetricData> GetMetricData(int metricDataId)
		{
			var metricData = await this.metricDataRepository.ReadAsync(metricDataId);
			metricData.Metric = await this.metricRepository.ReadAsync(metricData.MetricId);
			metricData.Metric.Hour = await this.hourRepository.ReadAsync(metricData.Metric.HourId);
			if (metricData.ServerId.HasValue)
			{
				metricData.Server = await this.serverRepository.ReadAsync(metricData.ServerId.Value);
			}

			return metricData;
		}

		public async Task<IList<MetricData>> GetMetricData(IList<int> hourIds, int serverId, CategoryType categoryType)
		{
			var metricServer = await this.serverRepository.ReadAsync(serverId);
			var metricDatas = await this.metricDataRepository.ReadByCategoryTypeAndServerIdAsync(categoryType, serverId, hourIds);
			foreach (var data in metricDatas)
			{
				data.Metric = await this.metricRepository.ReadAsync(data.MetricId);
				data.Metric.Hour = await this.hourRepository.ReadAsync(data.Metric.HourId);
				data.Server = metricServer;
			}

			return metricDatas;
		}

		public T GetData<T>(MetricData metricData)
		{
			return metricData.Data == null
				? default(T)
				: Newtonsoft.Json.JsonConvert.DeserializeObject<T>(metricData.Data);
		}

		public void SetData<T>(MetricData metricData, T data)
		{
			if (data != null)
				metricData.Data = data.ToJson();
		}
	}
}
