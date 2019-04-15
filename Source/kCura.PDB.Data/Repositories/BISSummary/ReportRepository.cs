namespace kCura.PDB.Data.Repositories.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using Core.Interfaces.Services;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;

	/// <summary>
	/// Repository for Best in Service data access.
	/// </summary>
	public class ReportRepository : BaseRepository, IReportRepository
	{
		public ReportRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		#region Public Methods
		/// <summary>
		/// Checks whether Looking Glass has run on this environment
		/// </summary>
		/// <returns></returns>
		public bool LookingGlassHasRun()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.ExecuteScalar<bool>(Resources.CheckLookingGlassHistory);
			}
		}
		
		/// <summary>
		/// Retrieves a model containing Quality of Service scores for:
		///  * Overall
		///     * User Experience
		///     * System Load
		///     * Backup and DBCC Checks
		///     * Uptime
		/// </summary>
		/// <returns></returns>
		public List<QualityOfServiceModel> ReadQuality(int depth = 1)
		{
			var servers = new List<QualityOfServiceModel>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using(var reader = conn.ExecuteReader("eddsdbo.QoS_Quality", new{ depth}, commandType:CommandType.StoredProcedure))
				{
					while (reader.Read())
					{
						var server = new QualityOfServiceModel();
						server.ServerArtifactId = reader.GetInt32(0);
						server.ServerName = reader.GetString(1);
						server.OverallScore = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3);
						server.WeeklyScore = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4);
						server.UserExperienceScore = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
						server.SystemLoadScore = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6);
						server.IntegrityScore = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7);
						server.UptimeScore = reader.IsDBNull(8) ? (decimal?)null : reader.GetDecimal(8);
						server.WeeklyUserExperienceScore = reader.IsDBNull(9) ? (decimal?)null : reader.GetDecimal(9);
						server.WeeklySystemLoadScore = reader.IsDBNull(10) ? (decimal?)null : reader.GetDecimal(10);
						server.WeeklyIntegrityScore = reader.IsDBNull(11) ? (decimal?)null : reader.GetDecimal(11);
						server.WeeklyUptimeScore = reader.IsDBNull(12) ? (decimal?)null : reader.GetDecimal(12);
						servers.Add(server);
					}
				}
			}
			return servers;
		}

		public List<ScoreChartModel> ReadScoreHistory(DateTime startDate, DateTime endDate, string servers)
		{
			var scores = new List<ScoreChartModel>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader("eddsdbo.QoS_ScoreHistory", new {startDate, endDate, servers},
					commandType: CommandType.StoredProcedure))
				{
					while (reader.Read())
					{
						var score = new ScoreChartModel();
						score.ServerArtifactId = reader.GetInt32(0);
						score.ServerName = reader.GetString(1);
						score.SummaryDayHour = reader.GetDateTime(2);
						score.UserExperienceScore = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3);
						score.SystemLoadScore = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4);
						score.BackupDbccScore = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
						score.UptimeScore = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6);
						score.IsSample = reader.GetBoolean(7);
						scores.Add(score);
					}
				}
			}

			return scores;
		}

		/// <summary>
		/// Retrieves information about all servers in the BiS sample set
		/// </summary>
		/// <returns></returns>
		public DataTable ReadServers()
		{
			return GetReportDetail(DrillMode.Servers, 0);
		}

		/// <summary>
		/// Given a workspace ArtifactID, returns its friendly name (or DELETED: EDDS####### if it no longer exists)
		/// </summary>
		/// <param name="caseArtifactId"></param>
		/// <returns></returns>
		public string LookupWorkspaceName(int caseArtifactId)
		{
			using (var connection = (SqlConnection)this.connectionFactory.GetEddsConnection())
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = Resources.LookupWorkspaceName;

					var artifactId = command.CreateParameter();
					artifactId.DbType = DbType.Int32;
					artifactId.ParameterName = "@CaseArtifactID";
					artifactId.Value = caseArtifactId;

					command.Parameters.Add(artifactId);
					command.Connection.Open();
					return command.ExecuteScalar() as string;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the User Experience Server View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetUserExperienceServerDetails(GridConditions gridConditions, ServerViewFilterConditions filterConditions, ServerViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@Server", DbType = DbType.String, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@Workspace", DbType = DbType.String, Value = filterConditions.Workspace },
						new SqlParameter { ParameterName = "@TotalUsers", DbType = DbType.Int32, Value = filterConditions.TotalUsers },
						new SqlParameter { ParameterName = "@TotalLongRunning", DbType = DbType.Int32, Value = filterConditions.TotalLongRunning },
						new SqlParameter { ParameterName = "@TotalSearchAudits", DbType = DbType.Int32, Value = filterConditions.TotalSearchAudits },
						new SqlParameter { ParameterName = "@TotalNonSearchAudits", DbType = DbType.Int32, Value = filterConditions.TotalNonSearchAudits },
						new SqlParameter { ParameterName = "@TotalAudits", DbType = DbType.Int32, Value = filterConditions.TotalAudits },
						new SqlParameter { ParameterName = "@TotalExecutionTime", DbType = DbType.Int64, Value = filterConditions.TotalExecutionTime },
						new SqlParameter { ParameterName = "@Score", DbType = DbType.Int32, Value = filterConditions.Score },
						new SqlParameter { ParameterName = "@IsActiveArrivalRateSample", DbType = DbType.Boolean, Value = filterConditions.IsActiveWeeklySample },
						//Filter operands  IsActiveArrivalRateSample
						new SqlParameter { ParameterName = "@ScoreOperand", DbType = DbType.String, Value = filterOperands.Score.GetSqlOperation() },
						new SqlParameter { ParameterName = "@TotalUsersOperand", DbType = DbType.String, Value = filterOperands.TotalUsers.GetSqlOperation()  },
						new SqlParameter { ParameterName = "@TotalSearchOperand", DbType = DbType.String, Value = filterOperands.TotalSearchAudits.GetSqlOperation()  },
						new SqlParameter { ParameterName = "@TotalNonSearchOperand", DbType = DbType.String, Value = filterOperands.TotalNonSearchAudits.GetSqlOperation()  },
						new SqlParameter { ParameterName = "@TotalLongRunningOperand", DbType = DbType.String, Value = filterOperands.TotalLongRunning.GetSqlOperation()  },
						new SqlParameter { ParameterName = "@TotalExecutionTimeOperand", DbType = DbType.String, Value = filterOperands.TotalExecutionTime.GetSqlOperation()  },
						new SqlParameter { ParameterName = "@TotalAuditsOperand", DbType = DbType.String, Value = filterOperands.TotalAudits.GetSqlOperation()  },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_UserExperienceServerReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the User Experience Hours View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetUserExperienceHourDetails(GridConditions gridConditions, HoursViewFilterConditions filterConditions, HoursViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@Server", DbType = DbType.Int32, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@Workspace", DbType = DbType.String, Value = filterConditions.Workspace },
						new SqlParameter { ParameterName = "@Search", DbType = DbType.String, Value = filterConditions.Search },
						new SqlParameter { ParameterName = "@TotalRunTime", DbType = DbType.Int64, Value = filterConditions.TotalRunTime },
						new SqlParameter { ParameterName = "@AverageRunTime", DbType = DbType.Int32, Value = filterConditions.AverageRunTime },
						new SqlParameter { ParameterName = "@TotalRuns", DbType = DbType.Int32, Value = filterConditions.NumberOfRuns },
						new SqlParameter { ParameterName = "@IsComplex", DbType = DbType.Boolean, Value = filterConditions.IsComplex },
						new SqlParameter { ParameterName = "@IsActiveArrivalRateSample", DbType = DbType.Boolean, Value = filterConditions.IsActiveWeeklySample },
						//Filter operands
						new SqlParameter { ParameterName = "@TotalRunTimeOperand", DbType = DbType.String, Value = filterOperands.TotalRunTime.GetSqlOperation() },
						new SqlParameter { ParameterName = "@AverageRunTimeOperand", DbType = DbType.String, Value = filterOperands.AverageRunTime.GetSqlOperation() },
						new SqlParameter { ParameterName = "@TotalRunsOperand", DbType = DbType.String, Value = filterOperands.TotalRuns.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_UserExperienceWorkspaceReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the User Experience Search View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetUserExperienceSearchDetails(GridConditions gridConditions, SearchViewFilterConditions filterConditions, SearchViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@CaseArtifactId", DbType = DbType.Int32, Value = filterConditions.CaseArtifactId },
						new SqlParameter { ParameterName = "@SummaryDayHour", DbType = DbType.DateTime, Value = filterConditions.SummaryDayHour },
						new SqlParameter { ParameterName = "@Search", DbType = DbType.String, Value = filterConditions.Search },
						new SqlParameter { ParameterName = "@User", DbType = DbType.String, Value = filterConditions.User },
						new SqlParameter { ParameterName = "@PercentLongRunning", DbType = DbType.Int32, Value = filterConditions.PercentLongRunning },
						new SqlParameter { ParameterName = "@IsComplex", DbType = DbType.Boolean, Value = filterConditions.IsComplex },
						new SqlParameter { ParameterName = "@TotalRunTime", DbType = DbType.Int64, Value = filterConditions.TotalRunTime },
						new SqlParameter { ParameterName = "@AverageRunTime", DbType = DbType.Int32, Value = filterConditions.AverageRunTime },
						new SqlParameter { ParameterName = "@TotalRuns", DbType = DbType.Int32, Value = filterConditions.NumberOfRuns },
						new SqlParameter { ParameterName = "@QoSHourID", DbType = DbType.Int64, Value = filterConditions.QoSHourID },
						new SqlParameter { ParameterName = "@IsActiveArrivalRateSample", DbType = DbType.Boolean, Value = filterConditions.IsActiveWeeklySample },
						//Filter operands
						new SqlParameter { ParameterName = "@PercentLongRunningOperand", DbType = DbType.String, Value = filterOperands.PercentLongRunning.GetSqlOperation() },
						new SqlParameter { ParameterName = "@TotalRunTimeOperand", DbType = DbType.String, Value = filterOperands.TotalRunTime.GetSqlOperation() },
						new SqlParameter { ParameterName = "@AverageRunTimeOperand", DbType = DbType.String, Value = filterOperands.AverageRunTime.GetSqlOperation() },
						new SqlParameter { ParameterName = "@TotalRunsOperand", DbType = DbType.String, Value = filterOperands.TotalRuns.GetSqlOperation() },
						new SqlParameter { ParameterName = "@QoSHourIDOperand", DbType = DbType.String, Value = filterOperands.QoSHourID.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_UserExperienceSearchReport", parameters);
					return data.Tables;
				}
			}
		}


		/// <summary>
		/// Retrieves data for the System Load Server View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetSystemLoadServerDetails(GridConditions gridConditions, LoadViewFilterConditions filterConditions, LoadViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@Server", DbType = DbType.String, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@ServerType", DbType = DbType.String, Value = filterConditions.ServerType },
						new SqlParameter { ParameterName = "@Score", DbType = DbType.Int32, Value = filterConditions.OverallScore },
						new SqlParameter { ParameterName = "@CPUScore", DbType = DbType.Int64, Value = filterConditions.CPUScore },
						new SqlParameter { ParameterName = "@RAMScore", DbType = DbType.Int32, Value = filterConditions.RAMScore },
						new SqlParameter { ParameterName = "@MemorySignalStateScore", DbType = DbType.Int32, Value = filterConditions.MemorySignalScore },
                        new SqlParameter { ParameterName = "@WaitsScore", DbType = DbType.Int32, Value = filterConditions.WaitsScore },
                        new SqlParameter { ParameterName = "@VirtualLogFilesScore", DbType = DbType.Int32, Value = filterConditions.VirtualLogFilesScore },
                        new SqlParameter { ParameterName = "@LatencyScore", DbType = DbType.Int32, Value = filterConditions.LatencyScore },
						new SqlParameter { ParameterName = "@IsActiveArrivalRateSample", DbType = DbType.Boolean, Value = filterConditions.IsActiveWeeklySample },
						//Filter operands
						new SqlParameter { ParameterName = "@ScoreOperand", DbType = DbType.String, Value = filterOperands.OverallScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@CPUScoreOperand", DbType = DbType.String, Value = filterOperands.CPUUtilizationScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@RAMScoreOperand", DbType = DbType.String, Value = filterOperands.RAMUtilizationScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@MemorySignalStateScoreOperand", DbType = DbType.String, Value = filterOperands.MemorySignalScore.GetSqlOperation() },
                        new SqlParameter { ParameterName = "@WaitsScoreOperand", DbType = DbType.String, Value = filterOperands.WaitsScore.GetSqlOperation() },
                        new SqlParameter { ParameterName = "@VirtualLogFilesScoreOperand", DbType = DbType.String, Value = filterOperands.VirtualLogFilesScore.GetSqlOperation() },
                        new SqlParameter { ParameterName = "@LatencyScoreOperand", DbType = DbType.String, Value = filterOperands.LatencyScore.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_SystemLoadServerReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the System Load Waits View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetSystemLoadWaitsDetails(GridConditions gridConditions, WaitsViewFilterConditions filterConditions, WaitsViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@Server", DbType = DbType.String, Value = filterConditions.Server },
						new SqlParameter { ParameterName = "@Score", DbType = DbType.Int32, Value = filterConditions.OverallScore },
						new SqlParameter { ParameterName = "@SignalWaitsRatio", DbType = DbType.Int32, Value = filterConditions.SignalWaitsRatio },
						new SqlParameter { ParameterName = "@WaitType", DbType = DbType.String, Value = filterConditions.WaitType },
						new SqlParameter { ParameterName = "@SignalWaitTime", DbType = DbType.Int64, Value = filterConditions.SignalWaitTime },
						new SqlParameter { ParameterName = "@TotalWaitTime", DbType = DbType.Int64, Value = filterConditions.TotalWaitTime },
                        new SqlParameter { ParameterName = "@IsPoisonWait", DbType = DbType.Boolean, Value = filterConditions.IsPoisonWait },
						new SqlParameter { ParameterName = "@IsActiveArrivalRateSample", DbType = DbType.Boolean, Value = filterConditions.IsActiveWeeklySample },
                        new SqlParameter { ParameterName = "@PercentOfCPUThreshold", DbType = DbType.Decimal, Value = filterConditions.PercentOfCPUThreshold },
                        new SqlParameter { ParameterName = "@DifferentialWaitingTasksCount", DbType = DbType.Int64, Value = filterConditions.DifferentialWaitingTasksCount },
						//Filter operands
						new SqlParameter { ParameterName = "@ScoreOperand", DbType = DbType.String, Value = filterOperands.OverallScore.GetSqlOperation() },
						new SqlParameter { ParameterName = "@SignalWaitsRatioOperand", DbType = DbType.String, Value = filterOperands.SignalWaitsRatio.GetSqlOperation() },
						new SqlParameter { ParameterName = "@SignalWaitTimeOperand", DbType = DbType.String, Value = filterOperands.SignalWaitTime.GetSqlOperation() },
						new SqlParameter { ParameterName = "@TotalWaitTimeOperand", DbType = DbType.String, Value = filterOperands.TotalWaitTime.GetSqlOperation() },
                        new SqlParameter { ParameterName = "@PercentOfCPUThresholdOperand", DbType = DbType.String, Value = filterOperands.PercentOfCPUThreshold.GetSqlOperation() },
                        new SqlParameter { ParameterName = "@DifferentialWaitingTasksCountOperand", DbType = DbType.String, Value = filterOperands.DifferentialWaitingTasksCount.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }


					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_SystemLoadWaitsReport", parameters);
					return data.Tables;
				}
			}
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

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_RecoverabilityIntegrityReport", parameters);
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

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_RecoveryObjectivesReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves data for the Uptime Report View
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTableCollection GetUptimeHours(GridConditions gridConditions, UptimeViewFilterConditions filterConditions, UptimeViewFilterOperands filterOperands)
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
						new SqlParameter { ParameterName = "@Score", DbType = DbType.Int32, Value = filterConditions.Score },
                        new SqlParameter { ParameterName = "@Status", DbType = DbType.String, Value = filterConditions.Status },
                        new SqlParameter { ParameterName = "@Uptime", DbType = DbType.Decimal, Value = filterConditions.Uptime },
						//Filter operands
						new SqlParameter { ParameterName = "@ScoreOperand", DbType = DbType.String, Value = filterOperands.Score.GetSqlOperation() },
						new SqlParameter { ParameterName = "@UptimeOperand", DbType = DbType.String, Value = filterOperands.Uptime.GetSqlOperation() },
						//Page-level filters
						new SqlParameter { ParameterName = "@StartHour", DbType = DbType.DateTime, Value = gridConditions.StartDate },
						new SqlParameter { ParameterName = "@EndHour", DbType = DbType.DateTime, Value = gridConditions.EndDate }
					};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "eddsdbo.QoS_UptimeReport", parameters);
					return data.Tables;
				}
			}
		}

		/// <summary>
		/// Retrieves information about the CPU name, total memory, and free disk space for all active web and SQL servers
		/// </summary>
		/// <returns></returns>
		public DataTable GetSystemInformation()
		{
			return GetReportDetail(DrillMode.ServerInfo, int.MaxValue);
		}

		/// <summary>
		/// Retrieves the number of missed backups and the number of missed integrity checks in the last 90 days,
		///		the number of servers that failed to be monitored, and the number of databases that failed to be monitored.
		/// </summary>
		/// <returns></returns>
		public MissedBackupIntegrityInfo GetSummarizedBackupIntegrityInfo()
		{
			var info = new MissedBackupIntegrityInfo();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.CountMissedBackupsAndIntegrityChecks))
				{
					while (reader.Read())
					{
						info.MissedBackups = reader.GetInt32(0);
						info.MissedIntegrityChecks = reader.GetInt32(1);
						info.FailedServers = reader.GetInt32(2);
						info.FailedDatabases = reader.GetInt32(3);
						info.BackupFrequencyScore = (int)reader.GetDecimal(4);
						info.BackupCoverageScore = (int)reader.GetDecimal(5);
						info.DbccFrequencyScore = (int)reader.GetDecimal(6);
						info.DbccCoverageScore = (int)reader.GetDecimal(7);
						info.MaxDataLossMinutes = reader.GetInt32(8);
						info.TimeToRecoverHours = (int)reader.GetDecimal(9);
						info.RPOScore = (int)reader.GetDecimal(10);
						info.RTOScore = (int)reader.GetDecimal(11);
					}
				}
			}

			return info;
		}

		public List<string> ListMonitoringFailedServers()
		{
			var failedServers = new List<string>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.ListMonitoringFailedServerNames))
				{
					while (reader.Read())
					{
						failedServers.Add(reader.GetString(0));
					}
				}
			}

			return failedServers;
		}

		public List<string> ListMonitoringFailedDatabases()
		{
			var failedDatabases = new List<string>();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.ListMonitoringFailedDatabases))
				{
					while (reader.Read())
					{
						failedDatabases.Add(reader.GetString(0));
					}
				}
			}
			return failedDatabases;
		}

		public BackupDBCCNearingViolation GetBackupsDbccNearingViolation()
		{
			var info = new BackupDBCCNearingViolation
			{
				Backups = new List<DatabaseNearingViolation>(),
				DBCC = new List<DatabaseNearingViolation>()
			};

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var reader = conn.ExecuteReader(Resources.BackupsNearingViolation))
				{
					while (reader.Read())
					{
						var violator = new DatabaseNearingViolation();
						violator.Database = reader.GetString(0);
						violator.DaysMissed = reader.GetInt32(1);
						info.Backups.Add(violator);
					}
				}
				using (var reader = conn.ExecuteReader(Resources.ConsistencyChecksNearingViolation))
				{
					while (reader.Read())
					{
						var violator = new DatabaseNearingViolation();
						violator.Database = reader.GetString(0);
						violator.DaysMissed = reader.GetInt32(1);
						info.DBCC.Add(violator);
					}
				}
			}
			return info;
		}

		/// <summary>
		/// Retrieves outages for the Relativity instance
		/// </summary>
		/// <returns></returns>
		public DataTable GetUptimeDetail(bool forDetailReport, bool hourly, int timezoneOffset)
		{
			var depth = forDetailReport ? int.MaxValue : 5;
			return hourly
				? GetReportDetail(DrillMode.UptimeDetail, depth, timezoneOffset)
				: GetReportDetail(DrillMode.Uptime, depth, timezoneOffset);
		}

		/// <summary>
		/// Determines quarterly and weekly uptime percentages from QoS_UptimeRatings data
		/// </summary>
		/// <returns></returns>
		public UptimePercentageInfo GetUptimePercentages()
		{
			var model = new UptimePercentageInfo();
			using (DataTable data = GetReportDetail(DrillMode.UptimePercent, 1))
			{
				foreach (DataRow row in data.Rows)
				{
					model.QuarterlyUptimePercent = (double)row.Field<decimal?>(0).GetValueOrDefault(100);
					model.WeeklyUptimePercent = (double)row.Field<decimal?>(1).GetValueOrDefault(100);
				}
			}
			return model;
		}



		#endregion

		#region Private Methods
		/// <summary>
		/// Returns the data for each of the four detail reports based on a drill mode, filtered by server
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="depth"></param>
		/// <param name="serverArtifactId"></param>
		/// <param name="timezoneOffset">Timezone offset from the client</param>
		/// <returns></returns>
		private DataTable GetReportDetail(string mode, int depth, int timezoneOffset = 0, int serverArtifactId = -1)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				using (var command = new SqlCommand())
				{
					var server = command.CreateParameter();
					server.ParameterName = "@serverArtifactID";
					server.DbType = DbType.Int32;
					server.Value = serverArtifactId;

					var drillMode = command.CreateParameter();
					drillMode.ParameterName = "@drillmo";
					drillMode.DbType = DbType.String;
					drillMode.Value = mode;

					var depthParam = command.CreateParameter();
					depthParam.ParameterName = "@depth";
					depthParam.DbType = DbType.Int32;
					depthParam.Value = depth;

					var timezoneParam = command.CreateParameter();
					timezoneParam.ParameterName = "@timezoneOffset";
					timezoneParam.DbType = DbType.Int32;
					timezoneParam.Value = timezoneOffset;

					var parameters = new[] {
											server,
											drillMode,
											depthParam,
											timezoneParam
										};

					var data = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "EDDSPerformance.eddsdbo.QoS_ReportDrill", parameters);
					return data.Tables.Count > 0
										 ? data.Tables[0]
										 : new DataTable();
				}
			}
		}
		#endregion
	}
}