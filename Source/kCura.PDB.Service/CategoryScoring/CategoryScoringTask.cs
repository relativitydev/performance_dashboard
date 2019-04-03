namespace kCura.PDB.Service.CategoryScoring
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class CategoryScoringTask : ICategoryScoringTask
	{
		public CategoryScoringTask(
			IServiceFactory<ICategoryScoringLogic, CategoryType> categoryScoreFactory,
			ICategoryScoreRepository categoryScoreRepository,
			ICategoryRepository categoryRepository,
			IServerRepository serverRepository,
			IMetricDataService metricDataService,
			IHourRepository hourRepository,
			IEventRepository eventRepository,
			ILogger logger)
		{
			this.categoryScoreFactory = categoryScoreFactory;
			this.categoryScoreRepository = categoryScoreRepository;
			this.categoryRepository = categoryRepository;
			this.serverRepository = serverRepository;
			this.metricDataService = metricDataService;
			this.hourRepository = hourRepository;
			this.eventRepository = eventRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.CategoryScore);
		}

		private readonly IServiceFactory<ICategoryScoringLogic, CategoryType> categoryScoreFactory;
		private readonly ICategoryScoreRepository categoryScoreRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly IServerRepository serverRepository;
		private readonly IMetricDataService metricDataService;
		private readonly IHourRepository hourRepository;
		private readonly IEventRepository eventRepository;
		private readonly ILogger logger;

		public async Task<int> ScoreCategory(int categoryScoreId)
		{
			try
			{
				// Query the related data
				var categoryScore = await this.categoryScoreRepository.ReadAsync(categoryScoreId);
				categoryScore.Category = await this.categoryRepository.ReadAsync(categoryScore.CategoryId);
				var categoryScoringMetricDatas = await this.metricDataService.ReadMetricDatasByCategoryScoreAsync(categoryScore);
				if (categoryScore.ServerId.HasValue)
				{
					categoryScore.Server = await this.serverRepository.ReadAsync(categoryScore.ServerId.Value);
				}

				categoryScoringMetricDatas.Where(md => md.ServerId == categoryScore.ServerId).ForEach(md => md.Server = categoryScore.Server);

				// Get the category logic class
				var logic = this.categoryScoreFactory.GetService(categoryScore.Category.CategoryType);
				if (logic == null && MetricConstants.ActiveCategoryTypes.Contains(categoryScore.Category.CategoryType))
				{
					this.logger.LogError($"Cannot score category, no logic class implemented for category type: {categoryScore.Category.CategoryType} on category score id: {categoryScoreId}");
				}

				// Score category
				var result = Defaults.Scores.OneHundred;
				var scoreMetrics = logic?.ScoreMetrics(categoryScore, categoryScoringMetricDatas);
				if (scoreMetrics != null)
				{
					result = await scoreMetrics;
				}

				// Save result
				await this.logger.LogVerboseAsync($"Scored category {result} for category type: {categoryScore.Category.CategoryType} on category score id: {categoryScoreId}");
				categoryScore.Score = result;

				await this.categoryScoreRepository.UpdateAsync(categoryScore);

				return categoryScore.Category.Id;
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to score category for category score Id: {categoryScoreId}", ex);
				throw;
			}
		}

		public async Task<IList<int>> FindNextCategoriesToScore()
		{
			// Read the next hour that doesn't have a score/rating
			var nextHourToScore = await this.hourRepository.ReadNextHourWithoutRatings();
			if (nextHourToScore == null)
			{
				await this.logger.LogVerboseAsync("Skipping, no hour to score currently.");
				return new int[0];
			}

			await this.logger.LogVerboseAsync($"Found next hour to score: ID = {nextHourToScore.Id} - TimeStamp = {nextHourToScore.HourTimeStamp}");

			// Read the category scores for this hour
			var hourCategoryScores = await this.categoryScoreRepository.ReadAsync(nextHourToScore);
			await this.logger.LogVerboseAsync($"{hourCategoryScores.Count} hourCategoryScores obtained from hour {nextHourToScore.Id}");

			var results = new List<int>();
			foreach (var categoryScore in hourCategoryScores)
			{
				// query if we've already started scoring the category
				var alreadyStarted = await this.eventRepository.ExistsAsync(categoryScore.Id, (int)EventSourceType.ScoreCategoryScore);
				if (alreadyStarted)
				{
					await this.logger.LogVerboseAsync($"Skipping already started scoring for categoryScoreId ({categoryScore?.Id})");
					continue;
				}

				// query the category score's associated metric datas
				var categoryScoreMetricDatas = await this.metricDataService.ReadMetricDatasByCategoryScoreAsync(categoryScore);

				// filter for category scores where all associated metric data's are scored
				if (categoryScoreMetricDatas.Any())
				{
					if (categoryScoreMetricDatas.All(md => md.Score.HasValue))
					{
						await this.logger.LogVerboseAsync($"Queuing categoryScoreId {categoryScore?.Id} for scoring");
						results.Add(categoryScore.Id);
					}
					else
					{
						var metricDatasMissingScores = categoryScoreMetricDatas.Where(md => md.Score.HasValue == false).Select(md => md.Id);
						await this.logger.LogVerboseAsync(
							$"Not all metrics scored for categoryScoreId {categoryScore?.Id} of type {categoryScore?.Category?.CategoryType} for hour ID {nextHourToScore.Id}, metricDataIds: {{{string.Join(",", metricDatasMissingScores)}}}");
					}
				}
				else
				{
					throw new Exception($"No metric data found for categoryScoreId {categoryScore?.Id} of type {categoryScore?.Category?.CategoryType} for hour ID {nextHourToScore.Id}");
				}
			}

			return results;
		}
	}
}
