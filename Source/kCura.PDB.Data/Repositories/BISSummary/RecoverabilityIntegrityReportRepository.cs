namespace kCura.PDB.Data.Repositories.BISSummary
{
	using System.Data;
	using System.Data.SqlClient;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Services;

	public class RecoverabilityIntegrityReportRepository : IRecoverabilityIntegrityReportReader, IRecoverabilityIntegrityReportWriter
	{
		private readonly IConnectionFactory connectionFactory;

		public RecoverabilityIntegrityReportRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		/// <summary>
		/// Retrieves data for the Recoverability/Integrity View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetRecoverabilityIntegrityDetails(GridConditions gridConditions, RecoverabilityIntegrityViewFilterConditions filterConditions, RecoverabilityIntegrityViewFilterOperands filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var command = new SqlCommand())
				{
					var parameters = new SqlParameter[] {
						//Grid conditions
						new SqlParameter { ParameterName = "@SortColumn", DbType = DbType.String, Value = gridConditions.SortColumn },
						new SqlParameter { ParameterName = "@SortDirection", DbType = DbType.String, Value = gridConditions.SortDirection },
						new SqlParameter { ParameterName = "@TimezoneOffset", DbType = DbType.Int32, Value = gridConditions.TimezoneOffset },
						new SqlParameter { ParameterName = "@StartRow", DbType = DbType.Int32, Value = gridConditions.StartRow },
						new SqlParameter { ParameterName = "@EndRow", DbType = DbType.Int32, Value = gridConditions.EndRow },
						//Filter conditions
						new SqlParameter { ParameterName = "@SummaryDayHour", DbType = DbType.DateTime, Value = filterConditions.SummaryDayHour },
						new SqlParameter { ParameterName = "@RecoverabilityIntegrityScore", DbType = DbType.Int32, Value = filterConditions.RecoverabilityIntegrityScore },
						new SqlParameter { ParameterName = "@BackupFrequencyScore", DbType = DbType.Int32, Value = filterConditions.BackupFrequencyScore },
						new SqlParameter { ParameterName = "@BackupCoverageScore", DbType = DbType.Int32, Value = filterConditions.BackupCoverageScore },
						new SqlParameter { ParameterName = "@DbccFrequencyScore", DbType = DbType.Int32, Value = filterConditions.DbccFrequencyScore },
						new SqlParameter { ParameterName = "@DbccCoverageScore", DbType = DbType.Int32, Value = filterConditions.DbccCoverageScore },
						new SqlParameter { ParameterName = "@RPOScore", DbType = DbType.Int32, Value = filterConditions.RPOScore },
						new SqlParameter { ParameterName = "@RTOScore", DbType = DbType.Int32, Value = filterConditions.RTOScore },
						//Filter operands
						new SqlParameter { ParameterName = "@RecoverabilityIntegrityScoreOperand", DbType = DbType.String, Value = filterOperands.RecoverabilityIntegrityScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@BackupFrequencyScoreOperand", DbType = DbType.String, Value = filterOperands.BackupFrequencyScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@BackupCoverageScoreOperand", DbType = DbType.String, Value = filterOperands.BackupCoverageScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@DbccFrequencyScoreOperand", DbType = DbType.String, Value = filterOperands.DbccFrequencyScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@DbccCoverageScoreOperand", DbType = DbType.String, Value = filterOperands.DbccCoverageScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@RPOScoreOperand", DbType = DbType.String, Value = filterOperands.RPOScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@RTOScoreOperand", DbType = DbType.String, Value = filterOperands.RTOScore.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.RecoverabilityIntegritySummaryReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the Recovery Objectives View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetRecoveryObjectivesDetails(GridConditions gridConditions, RecoveryObjectivesViewFilterConditions filterConditions, RecoveryObjectivesViewFilterOperands filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var command = new SqlCommand())
				{
					var parameters = new SqlParameter[] {
						//Grid conditions
						new SqlParameter { ParameterName = "@SortColumn", DbType = DbType.String, Value = gridConditions.SortColumn },
						new SqlParameter { ParameterName = "@SortDirection", DbType = DbType.String, Value = gridConditions.SortDirection },
						new SqlParameter { ParameterName = "@StartRow", DbType = DbType.Int32, Value = gridConditions.StartRow },
						new SqlParameter { ParameterName = "@EndRow", DbType = DbType.Int32, Value = gridConditions.EndRow },
						//Filter conditions
                        new SqlParameter { ParameterName = "@Server", DbType = DbType.String, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@DBName", DbType = DbType.String, Value = filterConditions.DatabaseName },
						new SqlParameter { ParameterName = "@RPOScore", DbType = DbType.Int32, Value = filterConditions.RPOScore },
						new SqlParameter { ParameterName = "@RTOScore", DbType = DbType.Int32, Value = filterConditions.RTOScore },
						new SqlParameter { ParameterName = "@PotentialDataLossMinutes", DbType = DbType.Int32, Value = filterConditions.PotentialDataLossMinutes },
						new SqlParameter { ParameterName = "@EstimatedTimeToRecoverHours", DbType = DbType.Int32, Value = filterConditions.EstimatedTimeToRecoverHours },
						//Filter operands
						new SqlParameter { ParameterName = "@RPOScoreOperand", DbType = DbType.String, Value = filterOperands.RPOScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@RTOScoreOperand", DbType = DbType.String, Value = filterOperands.RTOScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@PotentialDataLossMinutesOperand", DbType = DbType.String, Value = filterOperands.PotentialDataLossMinutes.GetSqlOperation() },
						new SqlParameter { ParameterName = "@EstimatedTimeToRecoverHoursOperand", DbType = DbType.String, Value = filterOperands.EstimatedTimeToRecoverHours.GetSqlOperation() }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.RecoverabilityObjectivesReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the Backup/DBCC History View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetBackupDbccHistoryDetails(GridConditions gridConditions, BackupDbccViewFilterConditions filterConditions, BackupDbccViewFilterOperands filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var command = new SqlCommand())
				{
					var parameters = new SqlParameter[] {
						//Grid conditions
						new SqlParameter { ParameterName = "@SortColumn", DbType = DbType.String, Value = gridConditions.SortColumn },
						new SqlParameter { ParameterName = "@SortDirection", DbType = DbType.String, Value = gridConditions.SortDirection },
						new SqlParameter { ParameterName = "@TimezoneOffset", DbType = DbType.Int32, Value = gridConditions.TimezoneOffset },
						new SqlParameter { ParameterName = "@StartRow", DbType = DbType.Int32, Value = gridConditions.StartRow },
						new SqlParameter { ParameterName = "@EndRow", DbType = DbType.Int32, Value = gridConditions.EndRow },
						//Filter conditions
						new SqlParameter { ParameterName = "@Server", DbType = DbType.String, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@Database", DbType = DbType.String, Value = filterConditions.Database },
						new SqlParameter { ParameterName = "@LastActivityDate", DbType = DbType.DateTime, Value = filterConditions.LastActivityDate },
						new SqlParameter { ParameterName = "@ActivityType", DbType = DbType.Boolean, Value = filterConditions.IsBackup },
						new SqlParameter { ParameterName = "@ResolutionDate", DbType = DbType.DateTime, Value = filterConditions.ResolutionDate },
						new SqlParameter { ParameterName = "@GapSize", DbType = DbType.Int32, Value = filterConditions.GapSize },
						//Filter operands
						new SqlParameter { ParameterName = "@GapSizeOperand", DbType = DbType.String, Value = filterOperands.GapSize.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.RecoverabilityGapsReport", parameters);
					return data.Tables;
				}
			}
		}

		public async Task UpdateRecoveryObjectivesRpoReport(DatabaseRpoScoreData rpoScoreData)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				// Try updating first
				var updatedRecords = await conn.ExecuteAsync(
										 Resources.Reports_RecoveryObjectives_Rpo_Update,
										 new { rpoScoreData.DatabaseId, RpoMaxDataLoss = rpoScoreData.PotentialDataLoss, rpoScoreData.RpoScore });
				if (updatedRecords == 0)
				{
					await conn.ExecuteAsync(
						Resources.Reports_RecoveryObjectives_Rpo_Create,
						new { rpoScoreData.DatabaseId, RpoMaxDataLoss = rpoScoreData.PotentialDataLoss, rpoScoreData.RpoScore });
				}
			}
		}

		public async Task UpdateRecoveryObjectivesRtoReport(DatabaseRtoScoreData rtoScoreData)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var updatedRecords = await conn.ExecuteAsync(
					                     Resources.Reports_RecoveryObjectives_Rto_Update,
					                     new { rtoScoreData.DatabaseId, RtoTimeToRecover = rtoScoreData.TimeToRecoverHours, rtoScoreData.RtoScore });
				if (updatedRecords == 0)
				{
					await conn.ExecuteAsync(
						Resources.Reports_RecoveryObjectives_Rto_Create,
						new { rtoScoreData.DatabaseId, RtoTimeToRecover = rtoScoreData.TimeToRecoverHours, rtoScoreData.RtoScore });
				}
			}
		}

		// Might not be needed
		public async Task CreateGapReportData(GapReportEntry gapReportEntry)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_Gap_Create, gapReportEntry);
			}
		}

		public async Task ClearUnresolvedGapReportData(int serverId, GapActivityType gapActivityType)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_Gap_ClearUnresolvedActivityType, new { ActivityType = gapActivityType, serverId });
			}
		}


		public async Task CreateRecoverabilityIntegrityReportData(RecoverabilityIntegrityReportEntry recoverabilityIntegrityReportEntry)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_RecoverabilityIntegrity_Create, recoverabilityIntegrityReportEntry);
			}
		}

		public async Task CreateRecoverabilityIntegrityRpoReport(WorstRpoReportEntry worstRpoEntry)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_RecoverabilityIntegrityRpo_Create, worstRpoEntry);
			}
		}

		public async Task CreateRecoverabilityIntegrityRtoReport(WorstRtoReportEntry worstRtoEntry)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_RecoverabilityIntegrityRto_Create, worstRtoEntry);
			}
		}
	}
}
