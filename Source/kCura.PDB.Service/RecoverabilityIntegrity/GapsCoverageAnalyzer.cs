namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using RiDefaults = Core.Constants.Defaults.RecoverabilityIntegrity;

	public class GapsCoverageAnalyzer : IGapsCoverageAnalyzer
	{
		private readonly IDatabaseRepository databaseRepository;
		private readonly IDatabaseGapsRepository databaseGapsRepository;
		private readonly ILogger logger;

		private readonly IGapReporter gapReporter;

		public GapsCoverageAnalyzer(
			IDatabaseRepository databaseRepository,
			IDatabaseGapsRepository databaseGapsRepository,
			ILogger logger,
			IGapReporter gapReporter)
		{
			this.databaseRepository = databaseRepository;
			this.databaseGapsRepository = databaseGapsRepository;
			this.logger = logger
				.WithClassName()
				.WithCategory(Names.LogCategory.RecoverabilityIntegrity)
				.WithCategory(Names.LogCategory.MetricData);
			this.gapReporter = gapReporter;
		}

		public decimal ScoreCoverageData(CoverageMetricData coverageMetricData)
		{
			var noncoveredRatio = ((double)coverageMetricData.TotalDatabases - coverageMetricData.DatabasesCovered)
								/ coverageMetricData.TotalDatabases;

			if (noncoveredRatio < 0.0075)
			{
				return FormulaForNoncoveredRatioLessThan0_75Percent();
			}

			return noncoveredRatio >= 0.1
					? FormulaForNoncoveredRatioMoreThan10Percent()
					: FormulaForNoncoveredRatioBetween0_075And0_1Percent(noncoveredRatio);
		}

		/// <summary>
		/// For values less than 0.75%
		/// </summary>
		/// <returns>Score</returns>
		internal static decimal FormulaForNoncoveredRatioLessThan0_75Percent() => Defaults.Scores.OneHundred;

		/// <summary>
		/// For values greater than 10%
		/// </summary>
		/// <returns>Score</returns>
		internal static decimal FormulaForNoncoveredRatioMoreThan10Percent() => Defaults.Scores.Zero;

		/// <summary>
		/// For values between 0.75% and 10%
		/// Score = "(0.1 - count of databases in QoS_BackSummary / total number of databases)/(0.1 - 0.0075)*100.0"
		/// </summary>
		/// <param name="ratio">Ratio of database that exceeded the recoverability window and the total number of databases</param>
		/// <returns>Score</returns>
		internal static decimal FormulaForNoncoveredRatioBetween0_075And0_1Percent(double ratio) =>
			(decimal)(100.0 * (0.1 - ratio) / 0.0925);

		public async Task<CoverageMetricData> CaptureCoverageData<TGap>(Hour hour, Server server, GapActivityType activityType)
			where TGap : Gap
		{
			var windowInDays = RiDefaults.WindowInDays;
			var windowInSeconds = (int)TimeSpan.FromDays(windowInDays).TotalSeconds;
			var windowExceededByDate = hour.HourTimeStamp.AddDays(-windowInDays);

			// Get databases exceeding the window for unresolved gaps
			var databaseCount = await this.databaseRepository.ReadCountByServerAsync(server);
			var databasesExceedingWindowUnresolved =
				await this.databaseRepository.ReadOutOfDateDatabasesAsync(server, windowExceededByDate, activityType);

			// Get databases exceeding the window for gaps recorded for the hour
			// Since the query is already filtering on gaps with duration greater than the window any database associated with any gap returned is in violation
			var largestGaps =
				await this.databaseGapsRepository.ReadGapsLargerThanForHourAsync<TGap>(server, hour, activityType, windowInSeconds);

			// Union the databases exceeding the window together by db Id and distinct them
			var databasesExceedingWindow = largestGaps
				.Select(g => g.DatabaseId)
				.Union(databasesExceedingWindowUnresolved.Select(db => db.Id))
				.Distinct()
				.ToList();

			var databasesCovered = databaseCount - databasesExceedingWindow.Count;

			// Write gaps exceeding to report table
			await this.gapReporter.CreateGapReport(hour, server, largestGaps, activityType);

			return new CoverageMetricData { TotalDatabases = databaseCount, DatabasesCovered = databasesCovered };
		}
	}
}
