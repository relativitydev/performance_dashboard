namespace kCura.PDB.Data.Repositories.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Services;

	public class FileLatencyRepository : BaseRepository, IFileLatencyRepository
	{
		public FileLatencyRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
		}

		public void ExecuteSaveFileLevelLatencyDetails(string targetQoSServer, string eddsPerformanceServerName)
		{
			using (var conn = this.connectionFactory.GetEddsQosConnection(targetQoSServer))
			{
				conn.Execute(Resources.FileLevelLatency, new {currentServerName = targetQoSServer, eddsPerformanceServerName });
			}
		}

		// Reads ALL servers' FLLD
		public DataTable GetFileLevelLatencyDetails(GridConditions gridConditions, Dictionary<FileLatency.Columns, String> filterConditions, Dictionary<FileLatency.Columns, FilterOperand> filterOperands)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var parameters = new SqlParameter[] {
					//Filter conditions
					new SqlParameter { ParameterName = "@serverNameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.ServerName]) },
					new SqlParameter { ParameterName = "@databaseNameFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.DatabaseName]) },
					new SqlParameter { ParameterName = "@scoreFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.Score]) },
					new SqlParameter { ParameterName = "@dataReadFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.DataReadLatency]) },
					new SqlParameter { ParameterName = "@dataWriteFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.DataWriteLatency]) },
					new SqlParameter { ParameterName = "@logReadFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.LogReadLatency]) },
					new SqlParameter { ParameterName = "@logWriteFilter", DbType = DbType.String, Value = GetNullableDBValue(filterConditions[FileLatency.Columns.LogWriteLatency]) },

					//Filter operands
					new SqlParameter { ParameterName = "@scoreOperator", DbType = DbType.Int32, Value = (int)filterOperands[FileLatency.Columns.Score] },
					new SqlParameter { ParameterName = "@dataReadOperator", DbType = DbType.Int32, Value = (int)filterOperands[FileLatency.Columns.DataReadLatency] },
					new SqlParameter { ParameterName = "@dataWriteOperator", DbType = DbType.Int32, Value = (int)filterOperands[FileLatency.Columns.DataWriteLatency] },
					new SqlParameter { ParameterName = "@logReadOperator", DbType = DbType.Int32, Value = (int)filterOperands[FileLatency.Columns.LogReadLatency] },
					new SqlParameter { ParameterName = "@logWriteOperator", DbType = DbType.Int32, Value = (int)filterOperands[FileLatency.Columns.LogWriteLatency] },
				};

				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadFileLatencyDetails, parameters);

				if (false == String.IsNullOrEmpty(gridConditions.SortColumn) && false == String.IsNullOrEmpty(gridConditions.SortDirection))
				{
					String sortExpression = $"{gridConditions.SortColumn} {gridConditions.SortDirection}";
					data.Tables[0].DefaultView.Sort = sortExpression;
					return data.Tables[0].DefaultView.ToTable();
				}

				return data.Tables[0];
			}
		}
	}
}
