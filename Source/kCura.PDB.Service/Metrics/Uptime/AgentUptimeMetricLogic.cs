namespace kCura.PDB.Service.Metrics.Uptime
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	[MetricType(MetricType.AgentUptime)]
	public class AgentUptimeMetricLogic : IMetricLogic
	{
		private readonly IAgentHistoryRepository agentHistoryRepository;
		private readonly IMetricDataService metricDataService;
		private readonly ILogger logger;

		public AgentUptimeMetricLogic(IAgentHistoryRepository agentHistoryRepository, IMetricDataService metricDataService, ILogger logger)
		{
			this.agentHistoryRepository = agentHistoryRepository;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Metric);
		}

		public async Task<decimal> ScoreMetric(MetricData metricData)
		{
			await this.logger.LogVerboseAsync($"Scoring metric data for Id {metricData.Id}");

			// check that if the hour we're score is before the earliest agent history (in case of backfilling after an install) then we give them a hundred.
			// if there is no earliest agent history than there are no agent histories at all and we give them a zero because they don't have the sql agent turned on.
			var earliestAgentHistory = await this.agentHistoryRepository.ReadEarliestAsync();
			if (earliestAgentHistory.TimeStamp.NormilizeToHour() >= metricData.Metric.Hour.HourTimeStamp) return Defaults.Scores.OneHundred;
			var agentUptime = this.metricDataService.GetData<AgentUptime>(metricData);
			if (agentUptime == null) return Defaults.Scores.Uptime;
			if ((decimal)agentUptime.TotalSamples <= 0) return Defaults.Scores.Zero;
			var percentUp = 100.0m * (decimal)agentUptime.SuccessfulSamples / (decimal)agentUptime.TotalSamples;
			return percentUp;
		}

		public async Task<object> CollectMetricData(MetricData metricData)
		{
			await this.logger.LogVerboseAsync($"Collecting metric data for Id {metricData.Id}");
			var agentHistoriesForHourTask = await this.agentHistoryRepository.ReadByHourAsync(metricData.Metric.Hour);
			var agentUptime = this.metricDataService.GetData<AgentUptime>(metricData) ?? new AgentUptime();
			var agentHistories = agentHistoriesForHourTask.ToList();
			agentUptime.SuccessfulSamples += agentHistories.Count(ah => ah.Successful);
			agentUptime.TotalSamples += agentHistories.Count();
			return (object)agentUptime;
		}
	}
}
