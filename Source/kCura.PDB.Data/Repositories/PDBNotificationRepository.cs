namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Services;

	public class PDBNotificationRepository : BaseRepository, IPDBNotificationRepository
	{
		public PDBNotificationRepository(IConnectionFactory connectionFactory) :
			base(connectionFactory)
		{
		}

		public DataTable GetFailingProcessControls()
		{
			/*
			TODO -- ExecuteDataset Dapper?
			//*/
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadFaililngProcessControls);
				return ds.Tables[0];
			}
		}

		public DataTable GetAgentsAlert()
		{
			/*
			TODO -- ExecuteDataset Dapper?
			//*/
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsPerformanceConnection())
			{
				var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, Resources.ReadNumberOfEnabledAgents);
				return ds.Tables[0];
			}
		}

		public Int32? GetSecondsSinceLastAgentHistoryRecord()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var seconds = conn.ExecuteScalar<Int32?>(Resources.ReadSecondsSinceLastAgentHistoryRecord);
				return seconds;
			}
		}
	}
}
