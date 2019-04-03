namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public interface IReportRepository : IDbRepository
	{
		bool LookingGlassHasRun();

		DataTable GetSystemInformation();

		DataTable ReadServers();

		string LookupWorkspaceName(int caseArtifactId);

		List<QualityOfServiceModel> ReadQuality(int depth = 1);

		List<ScoreChartModel> ReadScoreHistory(DateTime startDate, DateTime endDate, string servers);

		DataTableCollection GetUserExperienceServerDetails(GridConditions gridConditions, ServerViewFilterConditions filterConditions, ServerViewFilterOperands filterOperands);

		DataTableCollection GetUserExperienceHourDetails(GridConditions gridConditions, HoursViewFilterConditions filterConditions, HoursViewFilterOperands filterOperands);

		DataTableCollection GetUserExperienceSearchDetails(GridConditions gridConditions, SearchViewFilterConditions filterConditions, SearchViewFilterOperands filterOperands);

		DataTableCollection GetSystemLoadServerDetails(GridConditions gridConditions, LoadViewFilterConditions filterConditions, LoadViewFilterOperands filterOperands);

		DataTableCollection GetSystemLoadWaitsDetails(GridConditions gridConditions, WaitsViewFilterConditions filterConditions, WaitsViewFilterOperands filterOperands);

		DataTableCollection GetUptimeHours(GridConditions gridConditions, UptimeViewFilterConditions filterConditions, UptimeViewFilterOperands filterOperands);

		MissedBackupIntegrityInfo GetSummarizedBackupIntegrityInfo();

		List<string> ListMonitoringFailedServers();

		List<string> ListMonitoringFailedDatabases();

		BackupDBCCNearingViolation GetBackupsDbccNearingViolation();

		DataTable GetUptimeDetail(bool forDetailReport, bool hourly, int timezoneOffset);

		UptimePercentageInfo GetUptimePercentages();
	}
}