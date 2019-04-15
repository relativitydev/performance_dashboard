namespace kCura.PDB.Data.Tests
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class LocalDbConnectionFactory : IConnectionFactory
	{
		private readonly string eddsPerformanceMdf;
		private readonly string dataSource;
		private readonly string databaseSuffix;
		public LocalDbConnectionFactory(string dataSource, string eddsPerformanceMdf, string databaseSuffix)
		{
			this.eddsPerformanceMdf = eddsPerformanceMdf;
			this.dataSource = dataSource;
			this.databaseSuffix = databaseSuffix;
		}

		public IDbConnection GetEddsConnection(GenericCredentialInfo credentialInfo = null)
		{
			throw new NotImplementedException();
		}

		public IDbConnection GetEddsPerformanceConnection() =>
			GetAttachDbFileConnectionStringBuilder(Names.Database.EddsPerformance)
			.ToString()
			.ToDbConnection();

		public IDbConnection GetEddsQosConnection(string server = null)
		{
			throw new NotImplementedException();
		}

		public Task<IDbConnection> GetWorkspaceConnectionAsync(int workspaceId)
		{
			throw new NotImplementedException();
		}

		public IDbConnection GetWorkspaceConnection(int workspaceId)
		{
			throw new NotImplementedException();
		}

		public IDbConnection GetPdbResourceConnection(string server = null)
		{
			throw new NotImplementedException();
		}

		public IDbConnection GetMasterConnection(string server = null, GenericCredentialInfo credentialInfo = null) =>
			GetConnectionStringBuilder()
				.ToString()
				.ToDbConnection();

		public IDbConnection GetTargetConnection(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null) =>
			GetAttachDbFileConnectionStringBuilder(Names.Database.EddsPerformance)
			.ToString()
			.ToDbConnection();

		public string GetTargetConnectionString(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null) =>
			GetConnectionStringBuilder().ToString();

		public bool TestDataSource()
		{
			try
			{
				using (var conn = this.GetMasterConnection())
				{
					conn.Open();
					return true;
				}
			}
			catch (SqlException ex)
			{
				if (ex.Message.Contains(
						@"A network-related or instance-specific error occurred while establishing a connection to SQL Server"))
				{
					return false;
				}
				throw;
			}
		}

		private SqlConnectionStringBuilder GetAttachDbFileConnectionStringBuilder(string database) =>
			new SqlConnectionStringBuilder($@"Data Source=(LocalDb)\{dataSource};
				AttachDbFilename = {this.eddsPerformanceMdf};
				Initial Catalog={database + this.databaseSuffix};
				Integrated Security = True;
				Connect Timeout = 3;");

		private SqlConnectionStringBuilder GetConnectionStringBuilder() =>
			new SqlConnectionStringBuilder($@"Data Source=(LocalDb)\{dataSource};
				Integrated Security = True;
				Connect Timeout = 3;");
	}
}
