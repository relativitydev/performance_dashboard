namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Data.SqlClient;
	using Core.Interfaces.Services;
	using Dapper;
	using Properties;

	public abstract class BaseRepository
	{
		#region Properties & Constructors
		protected readonly IConnectionFactory connectionFactory;

		protected BaseRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		protected SqlConnectionStringBuilder _builder;
		#endregion


		/// <summary>
		/// Given a workspace's artifact ID, returns the artifact ID of the DB server on which it resides
		/// </summary>
		/// <param name="caseArtifactId"></param>
		/// <returns></returns>
		public int? GetWorkspaceServerId(int caseArtifactId)
		{
			using (var conn = connectionFactory.GetEddsConnection())
			{
				return conn.ExecuteScalar<int?>(Resources.GetServerForWorkspace, new {CaseArtifactID = caseArtifactId});
			}
		}
		
		protected object GetNullableDBValue(string val) =>
			!string.IsNullOrEmpty(val) ? (object)val : (object)DBNull.Value;

		protected object GetNullableDBValue<T>(T? val) where T : struct
		{
			return val.HasValue ? (object)val.Value : (object)DBNull.Value;
		}
	}
}
