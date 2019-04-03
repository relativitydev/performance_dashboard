namespace kCura.PDB.Service.CategoryScoring
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[CategoryType(CategoryType.Uptime)]
	public class UptimeScoringLogic : ICategoryScoringLogic
	{
		public UptimeScoringLogic(IUptimeRatingsRepository uptimeRatingsRepository, IMaintenanceWindowRepository maintenanceWindowRepository, ILogger logger)
		{
			this.uptimeRatingsRepository = uptimeRatingsRepository;
			this.maintenanceWindowRepository = maintenanceWindowRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Category);
		}

		private readonly IUptimeRatingsRepository uptimeRatingsRepository;
		private readonly IMaintenanceWindowRepository maintenanceWindowRepository;
		private readonly ILogger logger;

		public async Task<decimal> ScoreMetrics(CategoryScore categoryScore, IList<MetricData> metricDatas)
		{
			// Gather metric data if possible
			if (metricDatas.Any() == false)
			{
				throw new Exception("Cannot score empty list of metric data.");
			}

			var webUptimeMetricData = metricDatas.FirstOrDefault(md => md.Metric.MetricType == MetricType.WebUptime);
			var webUptime = webUptimeMetricData?.Score;

			var agentUptimeMetricData = metricDatas.FirstOrDefault(md => md.Metric.MetricType == MetricType.AgentUptime);
			var agentUptime = agentUptimeMetricData?.Score;

			// if both metrics are null then we give 100 and don't save anything to the qos uptime ratings
			if (webUptimeMetricData == null && agentUptimeMetricData == null)
			{
				this.logger.LogWarning("Both agent uptime and web uptime were not supplied");
				return Defaults.Scores.Uptime;
			}

			var metricData = webUptimeMetricData ?? agentUptimeMetricData; // Use Web Metric data by default, but fallback to Agent if needed
			var summaryDayHour = metricData.Metric.Hour.HourTimeStamp;
			var hoursUp = this.GetUptimePercentage(webUptime, agentUptime); // If both exist, use the lowest.  Else use whatever's not null.  100 if both are null (shouldn't happen?)
			var uptimeScore = this.UptimeScoreFromUptimePercentage(hoursUp); // Use formula to calcuate score from percentage

			// If we have a value, and it's less than perfect, the web was the one that was down...
			// ...unless the agent server also had a value, and was worse
			var isWebDowntime = webUptime.HasValue && webUptime < Defaults.Scores.OneHundred && (!agentUptime.HasValue || agentUptime > webUptime);

			var hourInScheduledDowntime = await this.maintenanceWindowRepository.HourIsInMaintenanceWindowAsync(metricData.Metric.Hour);

			// save uptime score to qos uptime ratings
			await this.uptimeRatingsRepository.Create(1 - (hoursUp / 100.0m), summaryDayHour, isWebDowntime, hourInScheduledDowntime);
			await this.uptimeRatingsRepository.UpdateWeeklyScores();
			await this.uptimeRatingsRepository.UpdateQuartlyScores(summaryDayHour);

			return uptimeScore;
		}

		public decimal GetUptimePercentage(decimal? webUptime, decimal? agentUptime)
		{
			if (webUptime.HasValue == false && agentUptime.HasValue == false)
				return Defaults.Scores.OneHundred;
			if (webUptime.HasValue == false)
				return agentUptime.Value;
			if (agentUptime.HasValue == false)
				return webUptime.Value;
			return webUptime < agentUptime ? webUptime.Value : agentUptime.Value;
		}

		public decimal UptimeScoreFromUptimePercentage(decimal uptimePercentage)
		{
			// http://www.wolframalpha.com/input/?i=(20+%2F+1.99)+*+(x)+%2B+(80-(1960%2F1.99))+from+97+to+100
			if (uptimePercentage >= 99.99m)
			{
				return Defaults.Scores.OneHundred;
			}

			var score = (20m * uptimePercentage / 1.99m) + (80m - (1960m / 1.99m));
			return score >= 0 ? score : Defaults.Scores.Zero;
		}
	}
}