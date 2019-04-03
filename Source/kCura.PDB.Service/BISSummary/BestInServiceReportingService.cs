namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.Services;

	/// <summary>
	/// Base service for interfacing models in kCura.PDD.Model with access in kCura.PDB.Web.
	/// Contains shared methods as seen on the Quality of Service page.
	/// </summary>
	public class BestInServiceReportingService : IBestInServiceReportingService
	{
		private readonly IConfigurationRepository configurationRepository;
		private readonly IProcessControlRepository processControlRepository;
		private readonly ISampleHistoryRepository sampleHistoryRepository;
		private readonly IReportRepository reportRepository;
		private readonly ISqlServerRepository sqlServerRepository;

		public BestInServiceReportingService(ISqlServerRepository sqlServerRepository)
		{
			this.configurationRepository = sqlServerRepository.ConfigurationRepository;
			this.processControlRepository = sqlServerRepository.ProcessControlRepository;
			this.sampleHistoryRepository = sqlServerRepository.SampleHistoryRepository;
			this.reportRepository = sqlServerRepository.ReportRepository;
			this.sqlServerRepository = sqlServerRepository;
		}

		public bool LookingGlassHasRun()
		{
			return this.reportRepository.LookingGlassHasRun();
		}

		/// <summary>
		/// Retrieves scores for the Quality of Service page.
		/// </summary>
		/// <param name="depth">the depth to read</param>
		/// <returns>List of QoS service models</returns>
		public virtual List<QualityOfServiceModel> GetQualityOfServiceScores(int depth = int.MaxValue)
		{
			return this.reportRepository.ReadQuality(depth);
		}

		/// <summary>
		/// Retrieves the overall quarterly and weekly scores
		/// </summary>
		/// <returns>The score summary</returns>
		public virtual ScoreSummary GetOverallScores()
		{
			var summary = new ScoreSummary();
			var scores = GetQualityOfServiceScores();
			if (scores != null)
			{
				var userExperienceScore = scores.Min(x => x.UserExperienceScore).GetValueOrDefault(100);
				var weeklyUserExperienceScore = scores.Min(x => x.WeeklyUserExperienceScore).GetValueOrDefault(100);
				var systemLoadScore = scores.Min(x => x.SystemLoadScore).GetValueOrDefault(100);
				var weeklySystemLoadScore = scores.Min(x => x.WeeklySystemLoadScore).GetValueOrDefault(100);
				var integrityScore = scores.Min(x => x.IntegrityScore).GetValueOrDefault(100);
				var weeklyIntegrityScore = scores.Min(x => x.WeeklyIntegrityScore).GetValueOrDefault(100);
				var uptimeScore = scores.Min(x => x.UptimeScore).GetValueOrDefault(100);
				var weeklyUptimeScore = scores.Min(x => x.WeeklyUptimeScore).GetValueOrDefault(100);

				var quarterlyScore = (userExperienceScore + systemLoadScore + integrityScore + uptimeScore) / 4;
				var weeklyScore = (weeklyUserExperienceScore + weeklySystemLoadScore + weeklyIntegrityScore + weeklyUptimeScore) / 4;

				summary.QuarterlyScore = (int)quarterlyScore;
				summary.WeeklyScore = (int)weeklyScore;
				summary.QuarterlyUserExperienceScore = (int)userExperienceScore;
				summary.QuarterlySystemLoadScore = (int)systemLoadScore;
				summary.QuarterlyIntegrityScore = (int)integrityScore;
				summary.QuarterlyUptimeScore = (int)uptimeScore;
				summary.WeeklyUserExperienceScore = (int)weeklyUserExperienceScore;
				summary.WeeklySystemLoadScore = (int)weeklySystemLoadScore;
				summary.WeeklyIntegrityScore = (int)weeklyIntegrityScore;
				summary.WeeklyUptimeScore = (int)weeklyUptimeScore;
			}

			return summary;
		}

		public virtual List<ScoreChartModel> GetScoreHistory(List<KeyValuePair<string, string>> queryParams)
		{
			var startDate = QueryParamsParsingService.StartDate(queryParams);
			var endDate = QueryParamsParsingService.EndDate(queryParams);
			var servers = QueryParamsParsingService.SelectedServers(queryParams);
			return this.reportRepository.ReadScoreHistory(startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), servers);
		}

		public virtual List<ScoreChartModel> GetScoreHistory(DateTime startDate, DateTime endDate, string servers)
		{
			return this.reportRepository.ReadScoreHistory(startDate, endDate, servers);
		}

		/// <summary>
		/// Returns missed backups/DBCC in the last 90 days and statistics for any monitoring failures
		/// </summary>
		/// <returns>Missed backup integrity information</returns>
		public virtual MissedBackupIntegrityInfo SummarizeBackupIntegrityInfo()
		{
			return this.reportRepository.GetSummarizedBackupIntegrityInfo();
		}

		public virtual List<string> ListMonitoringFailedServers()
		{
			return this.reportRepository.ListMonitoringFailedServers();
		}

		public virtual List<string> ListMonitoringFailedDatabases()
		{
			return this.reportRepository.ListMonitoringFailedDatabases();
		}

		/// <summary>
		/// Indicates any databases that are nearing violation for backups or consistency checks (DaysSinceLast >= 7)
		/// </summary>
		/// <returns>Backups and dbccs nearing violations</returns>
		public virtual BackupDBCCNearingViolation DatabasesNearingViolation()
		{
			return this.reportRepository.GetBackupsDbccNearingViolation();
		}

		/// <summary>
		/// When at least one method of DBCC monitoring is enabled, this will return true
		/// </summary>
		/// <returns>true if dbcc monitoring is enabled</returns>
		public virtual bool IsDBCCMonitoringEnabled()
		{
			var viewMonitoring = (this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring) ?? "False")
				.Equals("True", StringComparison.CurrentCultureIgnoreCase);
			var commandMonitoring = (this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccCommandMonitoring) ?? "False")
				.Equals("True", StringComparison.CurrentCultureIgnoreCase);

			return viewMonitoring || commandMonitoring;
		}

		/// <summary>
		/// Retrieves the five worst days of uptime and their scores.
		/// </summary>
		/// <returns>Queryable for the worst uptime days</returns>
		public virtual IQueryable<WorstUptimeDaysModel> ListWorstUptimeDays(int timezoneOffset)
		{
			var days = this.reportRepository.GetUptimeDetail(true, false, timezoneOffset);

			var model = (from DataRow day in days.Rows
						 select new WorstUptimeDaysModel
						 {
							 ScoreDate = day.Field<DateTime?>("SummaryDayHour").GetValueOrDefault(DateTime.Now),
							 HoursDown = day.Field<decimal?>("HoursDown").GetValueOrDefault(0)
						 }).AsQueryable();
			return model;
		}

		public virtual UptimePercentageInfo CalculateUptimePercentages()
		{
			return this.reportRepository.GetUptimePercentages();
		}

		/// <summary>
		/// Lists servers included in the BiS sample set.
		/// </summary>
		/// <returns>All the qos servers</returns>
		public virtual IQueryable<QoSServerInfo> ListAllServers()
		{
			var servers = this.reportRepository.ReadServers();

			var model = (from DataRow server in servers.Rows
						 select new QoSServerInfo
						 {
							 ArtifactId = server.Field<int?>("ArtifactID").GetValueOrDefault(0),
							 Name = server.Field<string>("ServerName")
						 }).AsQueryable();
			return model;
		}

		/// <inheritdoc />
		public virtual SampleHistoryRange GetSampleRange(double timezoneOffset)
		{
			var range = this.sampleHistoryRepository.ReadSampleRange();
			range.MaxSampleDate = range.MaxSampleDate?.AddMinutes(timezoneOffset);
			range.MinSampleDate = range.MinSampleDate?.AddMinutes(timezoneOffset);
			return range;
		}

		public virtual string GetPartnerName()
		{
			return this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.PartnerName);
		}

		public virtual string GetInstanceName()
		{
			return this.sqlServerRepository.ReadInstanceName();
		}

		/// <summary>
		/// Gets the artifact ID of a tab given its name.
		/// </summary>
		/// <param name="tabName">the name of the tab</param>
		/// <returns>the artifact id for the tab</returns>
		public virtual int GetTabArtifactId(string tabName)
		{
			return this.sqlServerRepository.ReadTabArtifactId(tabName);
		}

		/// <summary>
		/// Given a workspace's artifact ID, returns the artifact ID of the DB server on which it resides
		/// </summary>
		/// <param name="caseArtifactId">the case's artifact id</param>
		/// <returns>the server id for the workspace</returns>
		public virtual int GetWorkspaceServerId(int caseArtifactId)
		{
			return this.reportRepository.GetWorkspaceServerId(caseArtifactId).GetValueOrDefault(-1);
		}

		public DateTime ResetProcessControlLastRun(params ProcessControlId[] lastRunProcessControlTypes)
		{
			var processControls = this.processControlRepository.ReadAll().Where(p => lastRunProcessControlTypes.Contains(p.Id)).ToList();
			var minResetDate = DateTime.MaxValue;
			foreach (var processControl in processControls)
			{
				var resetTime = -1 * (processControl.Frequency.HasValue ? processControl.Frequency.Value : 1440);
				var resetDate = new DateTime(1900, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0).AddMinutes(resetTime);
				minResetDate = (resetDate < minResetDate) ? resetDate : minResetDate;
				processControl.LastProcessExecDateTime = resetDate;
				this.processControlRepository.Update(processControl);
			}

			minResetDate = (minResetDate != DateTime.MaxValue) ? minResetDate : DateTime.MinValue;

			return minResetDate;
		}

		public DateTime ProcessControlLastRun(params ProcessControlId[] lastRunProcessControlTypes)
		{
			var processControlLastProcessExecDateTime = this.processControlRepository.ReadAll()
					.Where(p => lastRunProcessControlTypes.Contains(p.Id))
					.Select(p => p.LastProcessExecDateTime)
					.Min();

			return processControlLastProcessExecDateTime;
		}
	}
}
