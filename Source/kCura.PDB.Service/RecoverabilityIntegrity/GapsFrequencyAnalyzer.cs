namespace kCura.PDB.Service.RecoverabilityIntegrity
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Models;
	using Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using RiDefaults = Core.Constants.Defaults.RecoverabilityIntegrity;

	public class GapsFrequencyAnalyzer : IGapsFrequencyAnalyzer
	{
		private readonly IDatabaseRepository databaseRepository;
		private readonly IDatabaseGapsRepository databaseGapsRepository;
		private readonly ILogger logger;

		public GapsFrequencyAnalyzer(
			IDatabaseRepository databaseRepository,
			IDatabaseGapsRepository databaseGapsRepository,
			ILogger logger)
		{
			this.databaseRepository = databaseRepository;
			this.databaseGapsRepository = databaseGapsRepository;
			this.logger = logger
				.WithClassName()
				.WithCategory(Names.LogCategory.RecoverabilityIntegrity)
				.WithCategory(Names.LogCategory.MetricData);
		}

		public decimal ScoreFrequencyData(FrequencyMetricData frequencyMetricData)
		{
			var windowExceededBy = frequencyMetricData?.WindowExceededBy;

			if (!windowExceededBy.HasValue)
			{
				return Defaults.Scores.OneHundred;
			}

			return windowExceededBy >= 20
				? FormulaForWindowExceededByMoreThan20Days()
				: FormulaForWindowExceededLessThan20Days(windowExceededBy.Value);
		}

		internal static decimal FormulaForWindowExceededByMoreThan20Days() => Defaults.Scores.Zero;
		internal static decimal FormulaForWindowExceededLessThan20Days(int windowExceededBy) => 100 - (windowExceededBy * 5);

		public async Task<FrequencyMetricData> CaptureFrequencyData<TGap>(Hour hour, Server server, GapActivityType activityType)
			where TGap : Gap
		{
			var endOfCurrentHour = hour.HourTimeStamp.AddHours(1);
			var windowInDays = RiDefaults.WindowInDays;

			// Get the largest unresolved gap for all the databases for the server
			var mostOutOfDateActivityForServer = await this.databaseRepository.ReadMostOutOfDateActivityByServerAsync(server, activityType);
			var largestUnresolvedGap = mostOutOfDateActivityForServer.HasValue ? (double?)(endOfCurrentHour - mostOutOfDateActivityForServer.Value).TotalDays : null;

			// Get the largest gap for gaps recorded for the hour
			var largestGap =
				await this.databaseGapsRepository.ReadLargestGapsForHourAsync<TGap>(server, hour, activityType);

			var largestGapInDays = largestGap?.Duration.HasValue ?? false ? (double?)TimeSpan.FromSeconds(largestGap.Duration.Value).TotalDays : null;

			await this.logger.LogVerboseAsync($"Window exceeded by largest {activityType} gap: {largestGapInDays} and for unresolved {activityType} gaps: {largestUnresolvedGap}. Hour: {hour.Id}-{hour.HourTimeStamp} Server: {server.ServerName}");

			var worstWindowExceededBy = new[] { largestGapInDays, largestUnresolvedGap }
				.Select(gap => gap > windowInDays ? gap - windowInDays : null) // subtract the allowed 9 day window if the window is greater than 9 days. Else, the window has not been exceeded.
				.Max(); // Take the largest windowExceededBy

			return new FrequencyMetricData
			{
				WindowExceededBy = worstWindowExceededBy.HasValue ? (int?)worstWindowExceededBy : null
			};
		}
	}
}
