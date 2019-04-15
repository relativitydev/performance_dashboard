namespace kCura.PDB.Data.Repositories.BISSummary
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using Core.Constants;
	using Core.Extensions;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models.BISSummary.Grids;
	using Core.Models.BISSummary.Models;
	using Core.Models.ScriptInstallation;
	using Dapper;
	using Properties;
	using Repositories;
	using Services;

	public class EnvironmentCheckRepository : BaseRepository, IEnvironmentCheckRepository
	{
		public EnvironmentCheckRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
		}

		/// <summary>
		/// Retrieves data for environment check system recommendations
		/// </summary>
		/// <param name="gridConditions"></param>
		/// <param name="filterConditions"></param>
		/// <returns></returns>
		public DataTable GetRecomendations(GridConditions gridConditions, EnvironmentCheckRecommendationFilterConditions filterConditions)
		{
			var sortColumn = gridConditions.SortColumn.ToLower() == "status" ? "Severity" : gridConditions.SortColumn;

			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var parameters = new[] {
						//Filter conditions
						new SqlParameter { ParameterName = "@scopeFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Scope) },
						new SqlParameter { ParameterName = "@nameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Name) },
						new SqlParameter { ParameterName = "@descriptionFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Description) },
						new SqlParameter { ParameterName = "@statusFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Status) },
						new SqlParameter { ParameterName = "@recommendationFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Recommendation) },
						new SqlParameter { ParameterName = "@valueFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Value) },
						new SqlParameter { ParameterName = "@sectionFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Section) },
					};

				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Properties.Resources.ReadEnvironmentCheckRecommendation, parameters);

				if (false == String.IsNullOrEmpty(sortColumn) && false == String.IsNullOrEmpty(gridConditions.SortDirection))
				{
					String sortExpression = String.Format("{0} {1}", sortColumn, gridConditions.SortDirection);
					data.Tables[0].DefaultView.Sort = sortExpression;
					return data.Tables[0].DefaultView.ToTable();
				}

				return data.Tables[0];
			}
		}



		public void SaveServerDetails(EnvironmentCheckServerDetails serverDetails)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				conn.Execute(
					Resources.InsertEnvironmentCheckSeverDetails,
					new
					{
						serverDetails.ServerName,
						serverDetails.Hyperthreaded,
						serverDetails.LogicalProcessors,
						serverDetails.OSName,
						serverDetails.OSVersion,
						serverDetails.ServerIPAddress
					});
			}
		}



		public void SaveRecommendation(Guid id, String scope, String value)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				conn.Execute(Resources.InsertEnvCheckRecommendations_ById, new { id, scope, value });
			}
		}

		public void ExecuteCollectDatabaseDetails(string targetQoSServer)
		{
			DataSet serverDetails;
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsQosConnection(targetQoSServer))
			{
				serverDetails = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadEnvironmentCheckServerDatabaseDetails);
			}
			var r = serverDetails.Tables[0].Rows[0];

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var serverName = ((SqlConnection)conn).DataSource;
				conn.Execute(Resources.SaveEnvironmentCheckServerDetails, new
				{
					serverName,
					sqlversion = r.GetField<string>("SQLVersion"),
					adhocWorkloads = r.GetField<int?>("AdHocWorkLoad"),
					maxServerMemory = r.GetField<int?>("MaxServerMemory"),
					maxDegreeOfParallelism = r.GetField<int?>("MaxDegreeOfParallelism"),
					tempDbFileCount = r.GetField<int?>("TempDBDataFiles"),
					lastSqlRestart = r.GetField<DateTime?>("LastSQLRestart")
				});
			}
		}

		public DataTable GetServerDetails(GridConditions gridConditions, EnvironmentCheckServerFilterConditions filterConditions, EnvironmentCheckServerFilterOperands filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var parameters = new[] {
						//Grid conditions

						//Filter conditions
						new SqlParameter { ParameterName = "@serverNameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.ServerName) },
						new SqlParameter { ParameterName = "@osnameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.OSName) },
						new SqlParameter { ParameterName = "@osversionFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.OSVersion) },
						new SqlParameter { ParameterName = "@logicalProcessorsFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.LogicalProcessors) },
						new SqlParameter { ParameterName = "@hypterthreadedFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.Hyperthreaded) },
						//Filter operands
						new SqlParameter { ParameterName = "@logicalProcessorsOperator", DbType = DbType.Int32, Value = (int)filterOperands.LogicalProcessors },
						//Page-level filters
					};

				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Properties.Resources.ReadEnvironmentCheckServerDetails, parameters);

				if (false == String.IsNullOrEmpty(gridConditions.SortColumn) && false == String.IsNullOrEmpty(gridConditions.SortDirection))
				{
					String sortExpression = String.Format("{0} {1}", gridConditions.SortColumn, gridConditions.SortDirection);
					data.Tables[0].DefaultView.Sort = sortExpression;
					return data.Tables[0].DefaultView.ToTable();
				}

				return data.Tables[0];
			}
		}

		public DataTable GetDatabaseDetails(GridConditions gridConditions, EnvironmentCheckDatabaseFilterConditions filterConditions, EnvironmentCheckDatabaseFilterOperands filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var parameters = new[] {
					//Grid conditions

					//Filter conditions
					new SqlParameter { ParameterName = "@serverNameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.ServerName) },
					new SqlParameter { ParameterName = "@sqlVersionFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.SQLVersion) },
					new SqlParameter { ParameterName = "@adhocWorkloadFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.AdHocWorkload) },
					new SqlParameter { ParameterName = "@maxServerMemoryFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.MaxServerMemory) },
					new SqlParameter { ParameterName = "@maxdegreeOfParallelismFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.MaxDegreeOfParallelism) },
					new SqlParameter { ParameterName = "@tempDBDataFilesFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.tempDBDataFiles) },
					new SqlParameter { ParameterName = "@lastSQLRestartFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions.LastSqlRestart) },
					//Filter operands
					new SqlParameter { ParameterName = "@adhocWorkloadOperator", DbType = DbType.Int32, Value = (int)filterOperands.AdHocWorkload },
					new SqlParameter { ParameterName = "@maxServerMemoryOperator", DbType = DbType.Int32, Value = (int)filterOperands.MaxServerMemory },
					new SqlParameter { ParameterName = "@maxDegreeOfParallelismOperator", DbType = DbType.Int32, Value = (int)filterOperands.MaxDegreeOfParallelism },
					new SqlParameter { ParameterName = "@tempDBDataFilesOperator", DbType = DbType.Int32, Value = (int)filterOperands.TempDBDataFiles },
					new SqlParameter { ParameterName = "@lastSQLRestartOperator", DbType = DbType.Int32, Value = (int)filterOperands.LastSQLRestart },
					//Page-level filters
				};

				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadEnvironmentCheckDatabaseDetails, parameters);

				if (false == String.IsNullOrEmpty(gridConditions.SortColumn) && false == String.IsNullOrEmpty(gridConditions.SortDirection))
				{
					String sortExpression = $"{gridConditions.SortColumn} {gridConditions.SortDirection}";
					data.Tables[0].DefaultView.Sort = sortExpression;
					return data.Tables[0].DefaultView.ToTable();
				}

				return data.Tables[0];
			}
		}

		public void ExecuteTuningForkRelativity()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(
					"[eddsdbo].[TuningForkRel]",
					commandType: CommandType.StoredProcedure,
					commandTimeout: Defaults.Database.TuningForkTimeout);
			}

		}

		public DataTable ExecuteTuningForkSystem(string targetQoSServer)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsQosConnection(targetQoSServer))
			{
				var parameters = new[]
				{
					new SqlParameter {ParameterName = "@currentServerName", DbType = DbType.String, Value = conn.DataSource}
				};

				var data = SqlHelper.ExecuteDataset(conn,
					CommandType.StoredProcedure,
					"[EDDSQoS].[eddsdbo].[TuningForkSys]",
					parameters);
				return data.Tables[0];
			}
		}

		public void SaveTuningForkSystemData(string serverName, DataTable data)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.DeleteServerEnvironmentCheckRecommendations, new { ServerName = serverName });

				//TODO: USE SqlBulkCopy !!!! See SqlHelper.BulkCopy
				foreach (DataRow row in data.Rows)
				{
					conn.Execute(Resources.CreateEnvironmentCheckRecommendations, new
					{
						Scope = row["Scope"],
						name = row["name"],
						description = row["description"],
						Status = row["Status"],
						Recommendation = row["Recommendation"],
						Value = row["Value"],
						Section = row["Section"],
						Severity = row["Severity"]
					});
				}
			}
		}

		public bool? ReadCheckIFISettings(DatabaseDirectoryInfo mdfldfDirs)
		{
			using (var conn = this.connectionFactory.GetPdbResourceConnection())
			{
				var dynamicParam = new DynamicParameters();
				dynamicParam.Add("@enabled", dbType:DbType.Boolean, direction: ParameterDirection.Output);
				dynamicParam.Add("@mdfPath", mdfldfDirs.MdfPath);
				dynamicParam.Add("@ldfPath", mdfldfDirs.LdfPath);
				conn.Execute(Names.Database.PdbResource + ".eddsdbo.CheckInstantFileInitialization", dynamicParam, commandType:CommandType.StoredProcedure);
				return dynamicParam.Get<bool>("@enabled");
			}
		}
	}
}