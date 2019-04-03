namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Web;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.BISSummary.ViewColumns;
	using kCura.PDB.Core.Models.BISSummary.ViewModels;

	public class BackupDbccService : BestInServiceReportingService
	{
		private readonly IRecoverabilityIntegrityReportReader reportRepository;

		public BackupDbccService(ISqlServerRepository sqlServerRepository, IRecoverabilityIntegrityReportReader reportRepository)
			: base(sqlServerRepository)
		{
			this.reportRepository = reportRepository;
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
		/// </summary>
		/// <returns></returns>
		public static BackupDbccViewModel PopulateGapModelSettings(List<KeyValuePair<string, string>> queryParams)
		{
			return new BackupDbccViewModel
			{
				GridConditions = PopulateGridConditions<BackupDbccViewColumns>(queryParams),
				FilterConditions = PopulateGapFilterConditions(queryParams),
				FilterOperands = PopulateGapFilterOperands(queryParams)
			};
		}

		internal static BackupDbccViewFilterOperands PopulateGapFilterOperands(List<KeyValuePair<string, string>> queryParams)
		{
			var filterOperands = new BackupDbccViewFilterOperands();
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value, out filterOperands.GapSize);
			return filterOperands;
		}

		internal static BackupDbccViewFilterConditions PopulateGapFilterConditions(List<KeyValuePair<string, string>> queryParams)
		{
			//Filter conditions
			var serverFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var databaseFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var typeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var lastActivityFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var resolutionFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var gapSizeFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");

			int gapSize;
			DateTime lastActivity, resolution;
			return new BackupDbccViewFilterConditions
			{
				Server = serverFilter.Value,
				Database = databaseFilter.Value,
				IsBackup = string.IsNullOrEmpty(typeFilter.Value)
					? null
					: (bool?)typeFilter.Value.Equals("Backup", StringComparison.CurrentCultureIgnoreCase),
				LastActivityDate = DateTime.TryParse(HttpUtility.UrlDecode(lastActivityFilter.Value), out lastActivity)
					? lastActivity
					: (DateTime?)null,
				ResolutionDate = DateTime.TryParse(HttpUtility.UrlDecode(resolutionFilter.Value), out resolution)
					? resolution
					: (DateTime?)null,
				GapSize = int.TryParse(gapSizeFilter.Value, out gapSize)
					? gapSize
					: (int?)null
			};
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
		/// </summary>
		/// <returns></returns>
		public static RecoverabilityIntegrityViewModel PopulateRecoverabilityIntegrityHoursModelSettings(List<KeyValuePair<string, string>> queryParams)
		{
			return new RecoverabilityIntegrityViewModel
			{
				GridConditions = PopulateGridConditions<RecoverabilityIntegrityViewColumns>(queryParams),
				FilterConditions = PopulateRecoverabilityIntegrityHoursFilterConditions(queryParams),
				FilterOperands = PopulateRecoverabilityIntegrityHoursFilterOperands(queryParams)
			};
		}

		internal static RecoverabilityIntegrityViewFilterOperands PopulateRecoverabilityIntegrityHoursFilterOperands(List<KeyValuePair<string, string>> queryParams)
		{
			var filterOperands = new RecoverabilityIntegrityViewFilterOperands();

			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_1").Value,
				out filterOperands.RecoverabilityIntegrityScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_2").Value,
				out filterOperands.BackupFrequencyScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value,
				out filterOperands.BackupCoverageScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value,
				out filterOperands.DbccFrequencyScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value,
				out filterOperands.DbccCoverageScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_6").Value,
				out filterOperands.RPOScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_7").Value,
				out filterOperands.RTOScore);

			return filterOperands;
		}

		internal static RecoverabilityIntegrityViewFilterConditions PopulateRecoverabilityIntegrityHoursFilterConditions(List<KeyValuePair<string, string>> queryParams)
		{
			var hourFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var overallScoreFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var backupFrequencyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var backupCoverageFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var dbccFrequencyFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var dbccCoverageFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");
			var rpoFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_6");
			var rtoFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_7");

			int overallScore, backupFrequencyScore, backupCoverageScore,
				dbccFrequencyScore, dbccCoverageScore, rpoScore, rtoScore;
			DateTime hour;
			return new RecoverabilityIntegrityViewFilterConditions
			{
				SummaryDayHour = DateTime.TryParse(System.Web.HttpUtility.UrlDecode(hourFilter.Value), out hour)
					? hour
					: (DateTime?)null,
				RecoverabilityIntegrityScore = int.TryParse(overallScoreFilter.Value, out overallScore)
					? overallScore
					: (int?)null,
				BackupFrequencyScore = int.TryParse(backupFrequencyFilter.Value, out backupFrequencyScore)
					? backupFrequencyScore
					: (int?)null,
				BackupCoverageScore = int.TryParse(backupCoverageFilter.Value, out backupCoverageScore)
					? backupCoverageScore
					: (int?)null,
				DbccFrequencyScore = int.TryParse(dbccFrequencyFilter.Value, out dbccFrequencyScore)
					? dbccFrequencyScore
					: (int?)null,
				DbccCoverageScore = int.TryParse(dbccCoverageFilter.Value, out dbccCoverageScore)
					? dbccCoverageScore
					: (int?)null,
				RPOScore = int.TryParse(rpoFilter.Value, out rpoScore)
					? rpoScore
					: (int?)null,
				RTOScore = int.TryParse(rtoFilter.Value, out rtoScore)
					? rtoScore
					: (int?)null,
			};
		}

		/// <summary>
		/// Pulls query parameters from the request and converts them to grid and filter conditions for the Backup/DBCC View
		/// </summary>
		/// <returns></returns>
		public static RecoveryObjectivesViewModel PopulateRecoveryObjectivesModelSettings(List<KeyValuePair<string, string>> queryParams)
		{
			return new RecoveryObjectivesViewModel
			{
				GridConditions = PopulateGridConditions<RecoveryObjectivesViewColumns>(queryParams),
				FilterConditions = PopulateRecoveryObjectivesFilterConditions(queryParams),
				FilterOperands = PopulateRecoveryObjectivesFilterOperands(queryParams)
			};
		}

		internal static RecoveryObjectivesViewFilterOperands PopulateRecoveryObjectivesFilterOperands(
			List<KeyValuePair<string, string>> queryParams)
		{
			var filterOperands = new RecoveryObjectivesViewFilterOperands();

			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_2").Value,
				out filterOperands.RPOScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_3").Value,
				out filterOperands.RTOScore);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_4").Value,
				out filterOperands.PotentialDataLossMinutes);
			System.Enum.TryParse<FilterOperand>(queryParams.FirstOrDefault(k => k.Key == "sOperand_5").Value,
				out filterOperands.EstimatedTimeToRecoverHours);

			return filterOperands;
		}

		internal static RecoveryObjectivesViewFilterConditions PopulateRecoveryObjectivesFilterConditions(List<KeyValuePair<string, string>> queryParams)
		{
			//Filter conditions
			var serverFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_0");
			var dbFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_1");
			var rpoFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_2");
			var rtoFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_3");
			var dataLossFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_4");
			var ttrFilter = queryParams.FirstOrDefault(k => k.Key == "sSearch_5");

			int rpoScore, rtoScore, dataLoss, ttr;
			return new RecoveryObjectivesViewFilterConditions()
			{
				Server = serverFilter.Value,
				DatabaseName = dbFilter.Value,
				RPOScore = int.TryParse(rpoFilter.Value, out rpoScore)
					? rpoScore
					: (int?)null,
				RTOScore = int.TryParse(rtoFilter.Value, out rtoScore)
					? rtoScore
					: (int?)null,
				PotentialDataLossMinutes = int.TryParse(dataLossFilter.Value, out dataLoss)
					? dataLoss
					: (int?)null,
				EstimatedTimeToRecoverHours = int.TryParse(ttrFilter.Value, out ttr)
					? ttr
					: (int?)null
			};
		}

		internal static GridConditions PopulateGridConditions<T>(List<KeyValuePair<string, string>> queryParams)
		where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			//Grid conditions
			var gridConditions = QueryParamsParsingService.PopulateCommonGridConditions(queryParams);

			gridConditions.SortIndex = QueryParamsParsingService.SortIndex(queryParams);
			gridConditions.SortColumn = QueryParamsParsingService.SortColumn<T>(queryParams);
			gridConditions.SortDirection = QueryParamsParsingService.SortDirection(queryParams);

			return gridConditions;
		}

		// Report methods //
		public virtual BackupDbccViewGrid BackupDbccHistory(GridConditions gridConditions, BackupDbccViewFilterConditions filterConditions, BackupDbccViewFilterOperands filterOperands)
		{
			var grid = new BackupDbccViewGrid();
			var dt = this.reportRepository.GetBackupDbccHistoryDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new BackupDbccGapInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 ServerId = d.Field<int>("ServerArtifactID"),
								 Server = d.Field<string>("ServerName"),
								 Database = d.Field<string>("DatabaseName"),
								 Workspace = d.Field<string>("WorkspaceName"),
								 IsBackup = d.Field<bool?>("IsBackup").GetValueOrDefault(false),
								 LastActivityDate = d.Field<DateTime?>("LastActivityDate"),
								 GapResolutionDate = d.Field<DateTime?>("ResolutionDate"),
								 GapSize = d.Field<int?>("GapSize").GetValueOrDefault(0)
							 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}

		public virtual RecoverabilityIntegrityViewGrid RecoverabilityIntegritySummary(List<KeyValuePair<string, string>> queryParams)
		{
			var gridConditions = PopulateGridConditions<RecoverabilityIntegrityViewColumns>(queryParams);
			var filterConditions = PopulateRecoverabilityIntegrityHoursFilterConditions(queryParams);
			var filterOperands = PopulateRecoverabilityIntegrityHoursFilterOperands(queryParams);
			return this.RecoverabilityIntegritySummary(gridConditions, filterConditions, filterOperands);
		}

		public virtual RecoverabilityIntegrityViewGrid RecoverabilityIntegritySummary(GridConditions gridConditions, RecoverabilityIntegrityViewFilterConditions filterConditions, RecoverabilityIntegrityViewFilterOperands filterOperands)
		{
			var grid = new RecoverabilityIntegrityViewGrid();
			var dt = this.reportRepository.GetRecoverabilityIntegrityDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new RecoverabilityIntegrityScoreInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 SummaryDayHour = d.Field<DateTime?>("SummaryDayHour").GetValueOrDefault(DateTime.UtcNow),
								 RecoverabilityIntegrityScore = d.Field<int?>("RecoverabilityIntegrityScore").GetValueOrDefault(100),
								 BackupFrequencyScore = d.Field<int?>("BackupFrequencyScore").GetValueOrDefault(100),
								 BackupCoverageScore = d.Field<int?>("BackupCoverageScore").GetValueOrDefault(100),
								 DbccFrequencyScore = d.Field<int?>("DbccFrequencyScore").GetValueOrDefault(100),
								 DbccCoverageScore = d.Field<int?>("DbccCoverageScore").GetValueOrDefault(100),
								 RPOScore = d.Field<int?>("RPOScore").GetValueOrDefault(100),
								 RTOScore = d.Field<int?>("RTOScore").GetValueOrDefault(100),
								 WorstRPODatabase = d.Field<string>("WorstRPODatabase"),
								 WorstRTODatabase = d.Field<string>("WorstRTODatabase"),
								 PotentialDataLossMinutes = d.Field<int?>("PotentialDataLossMinutes").GetValueOrDefault(0),
								 EstimatedTimeToRecoverHours = d.Field<int?>("EstimatedTimeToRecoverHours").GetValueOrDefault(0)
							 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}

		public virtual RecoveryObjectivesViewGrid RecoveryObjectivesSummary(List<KeyValuePair<string, string>> queryParams)
		{
			var gridConditions = PopulateGridConditions<RecoveryObjectivesViewColumns>(queryParams);
			var filterConditions = PopulateRecoveryObjectivesFilterConditions(queryParams);
			var filterOperands = PopulateRecoveryObjectivesFilterOperands(queryParams);
			return this.RecoveryObjectivesSummary(gridConditions, filterConditions, filterOperands);
		}

		public virtual RecoveryObjectivesViewGrid RecoveryObjectivesSummary(GridConditions gridConditions, RecoveryObjectivesViewFilterConditions filterConditions, RecoveryObjectivesViewFilterOperands filterOperands)
		{
			var grid = new RecoveryObjectivesViewGrid();
			var dt = this.reportRepository.GetRecoveryObjectivesDetails(gridConditions, filterConditions, filterOperands);
			if (dt.Count > 1)
			{
				var searchUsers = dt[0];
				grid.Data = (from DataRow d in searchUsers.Rows
							 select new RecoveryObjectivesInfo
							 {
								 Index = d.Field<int>("RowNumber"),
								 ServerId = d.Field<int?>("ServerId").GetValueOrDefault(0),
								 ServerName = d.Field<string>("ServerName"),
								 DatabaseName = d.Field<string>("DBName"),
								 RPOScore = d.Field<int?>("RPOScore").GetValueOrDefault(100),
								 RTOScore = d.Field<int?>("RTOScore").GetValueOrDefault(100),
								 PotentialDataLossMinutes = d.Field<int?>("PotentialDataLossMinutes").GetValueOrDefault(0),
								 EstimatedTimeToRecoverHours = d.Field<int?>("EstimatedTimeToRecoverHours").GetValueOrDefault(0)
							 }).AsQueryable();

				var resultInfo = dt[1];
				grid.Count = resultInfo.Rows.Count > 0 ? resultInfo.Rows[0].Field<int?>("FilteredCount").GetValueOrDefault(0) : 0;
			}

			return grid;
		}
	}
}
