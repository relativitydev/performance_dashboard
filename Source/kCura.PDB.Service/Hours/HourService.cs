namespace kCura.PDB.Service.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class HourService : IHourService
	{
		public HourService(
			IHourRepository hourRepository,
			IEventRepository eventRepository,
			IConfigurationRepository configurationRepository,
			IDatabaseService databaseService,
			ILogger logger)
		{
			this.hourRepository = hourRepository;
			this.eventRepository = eventRepository;
			this.configurationRepository = configurationRepository;
			this.databaseService = databaseService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Hour);
		}

		private readonly IHourRepository hourRepository;
		private readonly IEventRepository eventRepository;
		private readonly IConfigurationRepository configurationRepository;
		private readonly IDatabaseService databaseService;
		private readonly ILogger logger;

		public async Task<IList<int>> CreateNextHours()
		{
			var backFillHours = await this.hourRepository.ReadBackFillHoursAsync();
			return await this.CreateRemainingBackfillHours(backFillHours);
		}

		public async Task<int> StartHour(int hourId)
		{
			var hour = await this.hourRepository.ReadAsync(hourId);
			hour.StartedOn = DateTime.UtcNow;
			hour.Status = HourStatus.Started;
			await this.hourRepository.UpdateAsync(hour);

			var setAuditQueryConfigurationsTask = SetAuditQueryConfigurations();
			var updateTrackedDatabasesTask = this.databaseService.UpdateTrackedDatabasesAsync();
			await Task.WhenAll(setAuditQueryConfigurationsTask, updateTrackedDatabasesTask);

			return hourId;
		}

		public async Task<int> CompleteHour(int hourId)
		{
			var anyRemainingHourEvents =
				await this.eventRepository.ReadAnyRemainingHourEventsAsync(
				hourId,
				EventSourceType.CompleteHour,
				EventStatus.Pending,
				EventStatus.InProgress,
				EventStatus.PendingHangfire);

			if (!anyRemainingHourEvents)
			{
				var hour = await this.hourRepository.ReadAsync(hourId);
				hour.CompletedOn = DateTime.UtcNow;
				hour.Status = HourStatus.Completed;
				await this.hourRepository.UpdateAsync(hour);
			}

			return hourId;
		}

		internal async Task<IList<int>> CreateRemainingBackfillHours(IList<Hour> currentBackfillHours)
		{
			var backFillFirstHour = DateTime.UtcNow.AddDays(Defaults.BackfillDays).NormilizeToHour();

			// Read the latest completed hour or mock one from a year ago
			var latestCompletedHour = (await this.hourRepository.ReadLatestCompletedHourAsync())?.HourTimeStamp ?? backFillFirstHour.AddYears(-1);

			// 24 hours * number of backfill days plus 1 for the current hour
			var hoursToCreate = Enumerable.Range(0, (-24 * Defaults.BackfillDays) + 1)
				.Select(h => backFillFirstHour.AddHours(h))
				.Where(hour => currentBackfillHours.FirstOrDefault(h => h.HourTimeStamp == hour) == null && hour > latestCompletedHour)
				.Select(hour => new Hour { HourTimeStamp = hour }).ToList();
			if (hoursToCreate.Any() == false)
			{
				await this.logger.LogVerboseAsync("No new hours to process.");
				return new int[0];
			}

			await this.logger.LogVerboseAsync($"Creating {hoursToCreate.Count} new hours to process.");
			var hours = this.hourRepository.Create(hoursToCreate);

			return hours.Any() ? hours.Select(h => h.Id).ToArray() : new int[0];
		}

		internal async Task SetAuditQueryConfigurations()
		{
			// Make sure the audit queries are set to true.
			// Note, this will not retroactively set this for the current hour's audits but should ensure it for the next hour.

			// Read the current state of the instance settings
			var auditCountResult = (await this.configurationRepository.ReadEddsConfigurationInfoAsync(
									   ConfigurationKeys.Edds.SectionRelativityCore,
									   ConfigurationKeys.Edds.AuditCountQueries))?.FirstOrDefault()?.Value;

			var auditFullResult = (await this.configurationRepository.ReadEddsConfigurationInfoAsync(
									   ConfigurationKeys.Edds.SectionRelativityCore,
									   ConfigurationKeys.Edds.AuditFullQueries))?.FirstOrDefault()?.Value;

			var auditIdResult = (await this.configurationRepository.ReadEddsConfigurationInfoAsync(
									   ConfigurationKeys.Edds.SectionRelativityCore,
									   ConfigurationKeys.Edds.AuditIdQueries))?.FirstOrDefault()?.Value;

			// Only update if they are not already set to true
			if (!string.Equals(auditCountResult, "True", StringComparison.InvariantCultureIgnoreCase))
			{
				await this.configurationRepository.SetConfigurationValueEdds(
					ConfigurationKeys.Edds.SectionRelativityCore,
					ConfigurationKeys.Edds.AuditCountQueries,
					"True");
			}

			if (!string.Equals(auditFullResult, "True", StringComparison.InvariantCultureIgnoreCase))
			{
				await this.configurationRepository.SetConfigurationValueEdds(
					ConfigurationKeys.Edds.SectionRelativityCore,
					ConfigurationKeys.Edds.AuditFullQueries,
					"True");
			}

			if (!string.Equals(auditIdResult, "True", StringComparison.InvariantCultureIgnoreCase))
			{
				await this.configurationRepository.SetConfigurationValueEdds(
					ConfigurationKeys.Edds.SectionRelativityCore,
					ConfigurationKeys.Edds.AuditIdQueries,
					"True");
			}
		}
	}
}
