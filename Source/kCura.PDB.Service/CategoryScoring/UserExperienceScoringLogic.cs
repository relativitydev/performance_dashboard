namespace kCura.PDB.Service.CategoryScoring
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[CategoryType(CategoryType.UserExperience)]
	public class UserExperienceScoringLogic : ICategoryScoringLogic
	{
		private readonly IUserExperienceRatingsRepository userExperienceRatingsRepository;
		private readonly IHourRepository hourRepository;
		private readonly IUserExperienceSampleService userExperienceSampleService;
		private readonly IMetricDataService metricDataService;
		private readonly IServerAuditReporter serverAuditReporter;
		private readonly ILogger logger;

		public UserExperienceScoringLogic(
			IUserExperienceRatingsRepository userExperienceRatingsRepository,
			IHourRepository hourRepository,
			IServerAuditReporter serverAuditReporter,
			IUserExperienceSampleService userExperienceSampleService,
			IMetricDataService metricDataService,
			ILogger logger)
		{
			this.userExperienceRatingsRepository = userExperienceRatingsRepository;
			this.hourRepository = hourRepository;
			this.serverAuditReporter = serverAuditReporter;
			this.userExperienceSampleService = userExperienceSampleService;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.UserExperience);
		}

		public async Task<decimal> ScoreMetrics(CategoryScore categoryScore, IList<MetricData> metricDatas)
		{
			// Grab hour to work with
			Hour hour;
			if (metricDatas.Any() == false)
			{
				hour = await this.hourRepository.ReadAsync(categoryScore.Category.HourId);
			}
			else
			{
				hour = metricDatas.First().Metric.Hour;
			}

			// Grab server id to work with
			var serverId = categoryScore.ServerId ?? categoryScore.Server.ServerId;

			// Grab sample
			var sample = await this.userExperienceSampleService.CalculateSample(serverId, hour.Id);

			// Calculate the score components for the gathered sample
			var uxArrivalRateScore =
				await
					this.ScoreSampleData(
						sample.ArrivalRateHours.Where(h => h.IsActiveArrivalRateSample).Select(h => h.HourId).ToList(), serverId);

			var uxConcurrencyScore =
				await
					this.ScoreSampleData(
						sample.ConcurrencyHours.Where(h => h.IsActiveArrivalRateSample && h.IsActiveConcurrencySample)
							.Select(h => h.HourId)
							.ToList(), serverId);

			// Write out each of the components to the ratings table
			var createRatingsForServerHour = this.userExperienceRatingsRepository.CreateAsync(categoryScore.Server.ArtifactId.Value, uxArrivalRateScore, uxConcurrencyScore, hour.Id);
			
			// Update current sample for reports/final hour scoring
			await this.userExperienceSampleService.UpdateCurrentSample(sample);

			// Await the writing to the UserExperienceRatings table
			await createRatingsForServerHour;

			// Average out the scores for the final score for the server
			var finalCategoryScore = (uxArrivalRateScore + uxConcurrencyScore) / 2;

			// Update report data for hour/server
			await Task.WhenAll(
				this.serverAuditReporter.ReportServerAudits(categoryScore.Server, hour, finalCategoryScore),
				this.serverAuditReporter.FinalizeServerReports(categoryScore.Server, hour))
				.ContinueWith(t => this.serverAuditReporter.DeleteServerTempReportData(categoryScore.Server, hour));

			return finalCategoryScore;
		}

		internal async Task<decimal> ScoreSampleData(IList<int> hourIds, int serverId)
		{
			// If there is no data given, return default score
			if (!hourIds.Any())
			{
				return Defaults.Scores.UserExperience;
			}

			// Get data
			var hoursData =
				await this.metricDataService.GetMetricData(
					hourIds,
					serverId,
					CategoryType.UserExperience);

			// Score data
			return hoursData.Any()
				? hoursData.Average(data => data.Score ?? Defaults.Scores.UserExperience)
				: Defaults.Scores.UserExperience;
		}
	}
}