namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IBestInServiceReportingService
	{
		bool LookingGlassHasRun();

		/// <summary>
		/// Retrieves scores for the Quality of Service page.
		/// </summary>
		/// <returns></returns>
		List<QualityOfServiceModel> GetQualityOfServiceScores(int depth = int.MaxValue);

		/// <summary>
		/// Retrieves the overall quarterly and weekly scores
		/// </summary>
		/// <returns></returns>
		ScoreSummary GetOverallScores();

		List<ScoreChartModel> GetScoreHistory(DateTime startDate, DateTime endDate, string servers);

		/// <summary>
		/// Returns missed backups/DBCC in the last 90 days and statistics for any monitoring failures
		/// </summary>
		/// <returns></returns>
		MissedBackupIntegrityInfo SummarizeBackupIntegrityInfo();

		List<string> ListMonitoringFailedServers();

		List<string> ListMonitoringFailedDatabases();

		/// <summary>
		/// Indicates any databases that are nearing violation for backups or consistency checks (DaysSinceLast >= 7)
		/// </summary>
		/// <returns></returns>
		BackupDBCCNearingViolation DatabasesNearingViolation();

		/// <summary>
		/// When at least one method of DBCC monitoring is enabled, this will return true
		/// </summary>
		/// <returns></returns>
		bool IsDBCCMonitoringEnabled();

		/// <summary>
		/// Retrieves the five worst days of uptime and their scores.
		/// </summary>
		/// <returns></returns>
		IQueryable<WorstUptimeDaysModel> ListWorstUptimeDays(int timezoneOffset);

		UptimePercentageInfo CalculateUptimePercentages();

		/// <summary>
		/// Lists servers included in the BiS sample set.
		/// </summary>
		/// <returns></returns>
		IQueryable<QoSServerInfo> ListAllServers();

		/// <summary>
		/// Gets the current sample date range based on QoS_SampleHistoryUX and Hours
		/// </summary>
		/// <param name="timezoneOffset">Timezone offset from the client</param>
		/// <returns></returns>
		SampleHistoryRange GetSampleRange(double timezoneOffset);

		string GetPartnerName();

		string GetInstanceName();

		/// <summary>
		/// Gets the artifact ID of a tab given its name.
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		int GetTabArtifactId(string tabName);

		/// <summary>
		/// Given a workspace's artifact ID, returns the artifact ID of the DB server on which it resides
		/// </summary>
		/// <param name="caseArtifactId"></param>
		/// <returns></returns>
		int GetWorkspaceServerId(int caseArtifactId);

		DateTime ResetProcessControlLastRun(params ProcessControlId[] lastRunProcessControlTypes);

		DateTime ProcessControlLastRun(params ProcessControlId[] lastRunProcessControlTypes);
	}
}
