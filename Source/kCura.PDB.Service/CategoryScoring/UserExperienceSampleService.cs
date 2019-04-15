namespace kCura.PDB.Service.CategoryScoring
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.MetricDataSources;

	public class UserExperienceSampleService : IUserExperienceSampleService
	{
		private readonly IUserExperienceService userExperienceService;
		private readonly IHourRepository hourRepository;
		private readonly ILogger logger;
		private readonly IUserExperienceCacheRepository userExperienceCacheRepository;
		private readonly ISampleHistoryRepository sampleHistoryRepository;
		private readonly IServerRepository serverRepository;

		public UserExperienceSampleService(
			IUserExperienceService userExperienceService,
			IHourRepository hourRepository,
			ILogger logger,
			IUserExperienceCacheRepository userExperienceCacheRepository,
			ISampleHistoryRepository sampleHistoryRepository,
			IServerRepository serverRepository)
		{
			this.userExperienceService = userExperienceService;
			this.hourRepository = hourRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.UserExperience);
			this.userExperienceCacheRepository = userExperienceCacheRepository;
			this.sampleHistoryRepository = sampleHistoryRepository;
			this.serverRepository = serverRepository;
		}

		public async Task<PastWeekEligibleSample> CalculateSample(int serverId, int hourId)
		{
			var hour = await this.hourRepository.ReadAsync(hourId);

			// Get Past week's data and create sample hours for this server
			var pastWeekUserExperience = await this.GetPastWeekUserExperienceMetricDataAsync(serverId, hour);
			await this.logger.LogVerboseAsync(
				$"Past week's hours - {string.Join(" | ", pastWeekUserExperience.Select(p => $"HourID: {p.HourId} - ActiveUsers: {p.ActiveUsers} - ArrivalRate: {p.ArrivalRate}"))}");
			var eligibleArrivalRateHours = this.DetermineEligibleArrivalRateSample(pastWeekUserExperience);
			await this.logger.LogVerboseAsync($"EligibleArrivalRate hours - {eligibleArrivalRateHours.Count} -- {{ {string.Join(" | ", eligibleArrivalRateHours.Select(h => $"ServerID: {h.ServerId} - HourID: {h.HourId}"))} }}");
			var eligibleConcurrencyHours = this.DetermineEligibleConcurrencySample(pastWeekUserExperience);
			await this.logger.LogVerboseAsync($"EligibleConcurrency hours - {eligibleConcurrencyHours.Count} -- {{ {string.Join(" | ", eligibleConcurrencyHours.Select(h => $"ServerID: {h.ServerId} - HourID: {h.HourId}"))} }}");

			// Filter before score
			var ineligibleArrivalRateHours = eligibleArrivalRateHours.Count < Defaults.Scores.ArrivalRateCountThreshold
				? eligibleArrivalRateHours
				: new List<UserExperience>();
			var ineligibleConcurrencyHours = eligibleConcurrencyHours.Count < Defaults.Scores.ConcurrencyCountThreshold
				? eligibleConcurrencyHours
				: new List<UserExperience>();

			// Create sample hours for the table even if they are ineligible due to count currently.
			// This allows data to appear from the sample history and let them know which hours will possibly be considered for the sample.
			var sampleArrivalRateHours = eligibleArrivalRateHours.Select(
				h =>
					new SampleHistory
					{
						HourId = h.HourId,
						ServerId = serverId,
						IsActiveArrivalRateSample = !ineligibleArrivalRateHours.Contains(h)
					}).ToList();
			await this.logger.LogVerboseAsync($"Sample ArrivalRateHours to be written - {sampleArrivalRateHours.Count} -- {{ {string.Join(" | ", sampleArrivalRateHours.Select(h => $"ID: {h.Id} - HourID: {h.HourId}"))} }}");

			var sampleConcurrencyHours = eligibleConcurrencyHours.Select(
				h =>
					new SampleHistory
					{
						HourId = h.HourId,
						ServerId = serverId,
						IsActiveArrivalRateSample = !ineligibleArrivalRateHours.Contains(h),
						IsActiveConcurrencySample = !ineligibleConcurrencyHours.Contains(h)
					}).ToList();
			await this.logger.LogVerboseAsync($"Sample ConcurrencyHours to be written - {sampleConcurrencyHours.Count} -- {{ {string.Join(" | ", sampleConcurrencyHours.Select(h => $"ID: {h.Id} - HourID: {h.HourId}"))} }}");

			return new PastWeekEligibleSample
			{
				HourId = hourId,
				ServerId = serverId,
				ArrivalRateHours = sampleArrivalRateHours,
				ConcurrencyHours = sampleConcurrencyHours
			};
		}

		public async Task UpdateCurrentSample(PastWeekEligibleSample sample)
		{
			var standAloneServerTask = this.serverRepository.ReadPrimaryStandaloneAsync();

			// Reset Sample
			await this.sampleHistoryRepository.ResetCurrentSampleAsync(sample.ServerId);

			// Add the Arrival Rate sample hours to the table
			await this.sampleHistoryRepository.AddToCurrentSampleAsync(sample.ArrivalRateHours);

			// Add the Concurrency sample hours to the table
			await this.sampleHistoryRepository.AddToCurrentSampleAsync(sample.ConcurrencyHours);

			// Determine if we need to add standalone edds to sample history
			var standAloneServer = await standAloneServerTask;
			if (standAloneServer.HasValue)
			{
				var standAloneServerHours = sample.ArrivalRateHours.Select(h => new SampleHistory
				{
					HourId = h.HourId,
					ServerId = standAloneServer.Value,
					IsActiveArrivalRateSample = h.IsActiveArrivalRateSample
				}).ToList();
				await this.logger.LogVerboseAsync($"Sample Standalone server hours to be added - {standAloneServerHours.Count} -- {{ {string.Join(" | ", standAloneServerHours.Select(h => $"ID: {h.Id} - HourID: {h.HourId}"))} }}");
				await this.sampleHistoryRepository.AddToCurrentSampleAsync(standAloneServerHours);
			}
		}

		/// <summary> 
		/// Returns List of server/hour sample data for the past week. 
		/// </summary> 
		/// <param name="serverId">serverId to retrieve data for.</param> 
		/// <param name="hour">End hour to retrieve past week data for.</param> 
		/// <returns>List of server/hour sample data for the past week.</returns> 
		internal async Task<IList<UserExperience>> GetPastWeekUserExperienceMetricDataAsync(int serverId, Hour hour)
		{
			// Get the past week's hours
			var pastWeekHours = (await this.hourRepository.ReadPastWeekHoursAsync(hour)).OrderBy(h => h.HourTimeStamp).ToList();

			if (!pastWeekHours.Any())
			{
				this.logger.LogWarning($"No hours returned for the past week.");
				return new List<UserExperience>();
			}

			var startHour = pastWeekHours.First();
			var endHour = pastWeekHours.Last();

			// Performance logging
			var testTime = DateTime.UtcNow;
			await this.logger.LogVerboseAsync($"Start PastWeekUserExperienceMetricData for hour {hour.Id} - {hour.HourTimeStamp}");

			// Get the UserExperience data
			var userExperienceList = await this.userExperienceCacheRepository.ReadAsync(serverId, startHour.HourTimeStamp, endHour.HourTimeStamp);

			// Determine the missing data from the cache
			IList<Hour> hoursToQuery;
			if (userExperienceList == null)
			{
				userExperienceList = new List<UserExperience>();
				hoursToQuery = pastWeekHours.ToList();
			}
			else if (!userExperienceList.Any())
			{
				hoursToQuery = pastWeekHours.ToList();
			}
			else
			{
				// Diff and get any missing hours
				var diffIds = new HashSet<int>(userExperienceList.Select(ux => ux.HourId));
				hoursToQuery = pastWeekHours.Where(h => !diffIds.Contains(h.Id)).ToList();
			}

			if (hoursToQuery.Any())
			{
				// Query for the missing data
				var missingData = await hoursToQuery.Select(async h =>
				{
					var ux = await this.userExperienceService.BuildUserExperienceModel(serverId, h);

					// Cache this data
					return await this.userExperienceCacheRepository.CreateAsync(ux);
				}).WhenAllStreamed();

				// Add it to the list of data to return
				userExperienceList = userExperienceList.Concat(missingData).ToList();
			}

			await this.logger.LogVerboseAsync($"End PastWeekUserExperienceMetricData for hour {hour.Id} - {hour.HourTimeStamp} - ElapsedTime: {DateTime.UtcNow - testTime}");

			return userExperienceList;
		}

		/// <summary> 
		/// Determines Eligibility for the ArrivalRate sample among the given data (Assumes past week's data) 
		/// </summary> 
		/// <param name="pastWeekData">Window of data in which to determine sample eligibility</param> 
		/// <returns>Eligible ArrivalRate sample data</returns> 
		internal IList<UserExperience> DetermineEligibleArrivalRateSample(IList<UserExperience> pastWeekData)
		{
			// Calculate Arrival Rate sample
			return pastWeekData.OrderByDescending(data => data.ArrivalRate)
				.Where((data, i) =>
					(data.HasPoisonWaits || i < 33)
					&& data.ArrivalRate > 0.05m
					&& data.ActiveUsers > 1).ToList();
		}

		/// <summary> 
		/// Determines Eligibility for the ConcurrencyRate sample among the given data (Assumes past week's data) 
		/// </summary> 
		/// <param name="pastWeekData">Window of data in which to determine sample eligibility</param> 
		/// <returns>Eligible ConcurrencyRate sample data</returns>
		internal IList<UserExperience> DetermineEligibleConcurrencySample(IList<UserExperience> pastWeekData)
		{
			// Calculate arrival rate + concurrency sample
			return pastWeekData
					.OrderByDescending(data => data.ArrivalRate)
					.Where((data, i) =>
						(data.HasPoisonWaits || i < 33)
						&& data.ArrivalRate > 0.05m
						&& data.ActiveUsers > 1)
					.OrderByDescending(data => data.Concurrency)
					.Take(33)
					.ToList();
		}
	}
}
