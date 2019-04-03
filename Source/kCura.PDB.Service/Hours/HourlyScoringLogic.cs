namespace kCura.PDB.Service.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Toggles;

	public class HourlyScoringLogic : IHourlyScoringLogic
	{
		private readonly ICategoryScoreRepository categoryScoreRepository;
		private readonly ICategoryRepository categoryRepository;
		private readonly IHourRepository hourRepository;
		private readonly ILogService logService;
		private readonly IFcmRepository fcmRepository;
		private readonly IToggleProvider toggleProvider;
		private readonly ILegacyRecoverabilityIntegrityRepository legacyRecoverabilityIntegrityRepository;
		private readonly ILogger logger;

		public HourlyScoringLogic(
			ICategoryScoreRepository categoryScoreRepository,
			ICategoryRepository categoryRepository,
			IHourRepository hourRepository,
			ILogService logService,
			IFcmRepository fcmRepository,
			IToggleProvider toggleProvider,
			ILegacyRecoverabilityIntegrityRepository legacyRecoverabilityIntegrityRepository,
			ILogger logger)
		{
			this.categoryScoreRepository = categoryScoreRepository;
			this.categoryRepository = categoryRepository;
			this.hourRepository = hourRepository;
			this.logService = logService;
			this.fcmRepository = fcmRepository;
			this.toggleProvider = toggleProvider;
			this.legacyRecoverabilityIntegrityRepository = legacyRecoverabilityIntegrityRepository;
			this.logger = logger.WithTypeName(this); // .WithClassName(); -- doesn't work
		}

		public async Task<decimal> ScoreHour(Hour hour)
		{
			var categoryScores = await this.categoryScoreRepository.ReadAsync(hour);
			var categories = await categoryScores.Select(cs => this.categoryRepository.ReadAsync(cs.CategoryId)).WhenAllStreamed();
			categoryScores.ForEach(cs => cs.Category = categories.FirstOrDefault(c => c.Id == cs.CategoryId));
			var hourScore =
				categoryScores
				.Where(cs => MetricConstants.ActiveCategoryTypes.Contains(cs.Category.CategoryType))
				.Select(cs => cs.Score ?? 100)
				.Average();

			// Wrap sql around try catch so that SQL itself doesn't swallow the exceptions
			try
			{
				var logLevel = this.logService.GetLogLevel();
				var enabledLogging = logLevel != LogLevel.Errors && logLevel != LogLevel.NeverLog;

				// Trips FCM if invalid, no return
				await this.logger.LogVerboseAsync($"Checking hour validation for hour {hour.Id} - {hour.HourTimeStamp}");
				await this.fcmRepository.ValidatePreBuildAndRateSample(hour.Id, enabledLogging);
				await this.logger.LogVerboseAsync($"Validated ready to score hour {hour.Id} - {hour.HourTimeStamp}");
				var weekRecoverabilityIntegrityScore = await this.GetWeekRecoverabilityIntegrityScore(
					categoryScores.Where(cs => cs.Category?.CategoryType == CategoryType.RecoverabilityIntegrity).ToList(),
					hour);

				// Pass off to BuildAndRateSample sql script
				await this.hourRepository.ScoreHourWithBuildAndRateSampleAsync(hour, weekRecoverabilityIntegrityScore, enabledLogging);
				await this.fcmRepository.ApplySecondaryHashes();
				return hourScore;
			}
			catch (Exception e)
			{
				throw new Exception($"Exception thrown when executing BuildAndRateSample for hour id {hour.Id} - {hour.HourTimeStamp}", e);
			}
		}

		internal async Task<decimal> GetWeekRecoverabilityIntegrityScore(IList<CategoryScore> riCategoryScores, Hour hour)
		{
			if (!await this.toggleProvider.IsEnabledAsync<RecoverabilityIntegrityMetricSystemToggle>())
			{
				return (await this.legacyRecoverabilityIntegrityRepository.ReadWeekRecoverabilityIntegrityScore(hour)) ?? Defaults.Scores.OneHundred;
			}

			// Take the worst of the category score of the different servers
			return riCategoryScores.Any()
				? riCategoryScores.Select(cs => cs.Score).Min() ?? Defaults.Scores.OneHundred
				: Defaults.Scores.OneHundred;
		}
	}
}
