namespace kCura.PDB.Data.Services
{
	using System.Data;
	using System.Threading.Tasks;
	using Core.Constants;
	using Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Data;

	public abstract class GenericConnectionFactory : IConnectionFactory
	{
		private readonly IWorkspaceServerProvider workspaceServerProvider;

		public GenericConnectionFactory(IWorkspaceServerProvider workspaceServerProvider)
		{
			this.workspaceServerProvider = new CachedWorkspaceServerFactory(workspaceServerProvider);
		}

		public virtual IDbConnection GetEddsConnection(GenericCredentialInfo credentialInfo = null) =>
			this.GetDatabaseConnection(Names.Database.Edds, credentialInfo: credentialInfo);

		public virtual IDbConnection GetEddsPerformanceConnection() =>
			this.GetDatabaseConnection(Names.Database.EddsPerformance);

		public virtual IDbConnection GetEddsQosConnection(string server = null) =>
			this.GetDatabaseConnection(Names.Database.EddsQoS, server);

		public virtual IDbConnection GetPdbResourceConnection(string server = null) =>
			this.GetDatabaseConnection(Names.Database.PdbResource, server);

		public virtual IDbConnection GetMasterConnection(string server = null, GenericCredentialInfo credentialInfo = null) =>
			this.GetDatabaseConnection(Names.Database.Master, server, credentialInfo);

		public virtual Task<IDbConnection> GetWorkspaceConnectionAsync(int workspaceId)
		{
			var server = this.workspaceServerProvider.GetWorkspaceServer(workspaceId);
			var initialCatalog = string.Format(Names.Database.EddsWorkspacePrefix, workspaceId);
			return this.GetServerConnection(server).OpenAsync(initialCatalog);
		}

		public virtual IDbConnection GetWorkspaceConnection(int workspaceId)
		{
			var server = this.workspaceServerProvider.GetWorkspaceServer(workspaceId);
			var initialCatalog = string.Format(Names.Database.EddsWorkspacePrefix, workspaceId);
			return this.GetServerConnection(server).Open(initialCatalog);
		}

		/// <summary>
		/// Use only when necessary.
		/// </summary>
		/// <param name="targetDatabase"></param>
		/// <param name="targetServer"></param>
		/// <param name="credentialInfo"></param>
		/// <returns></returns>
		public virtual IDbConnection GetTargetConnection(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null) =>
			this.GetDatabaseConnection(targetDatabase, targetServer, credentialInfo).Open(targetDatabase);

		public virtual string GetTargetConnectionString(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null)
		{
			using (var conn = this.GetDatabaseConnection(targetDatabase, targetServer, credentialInfo))
			{
				return conn.ConnectionString;
			}
		}
			

		protected abstract IDbConnection GetServerConnection(string server = null, GenericCredentialInfo credentialInfo = null);

		protected abstract IDbConnection GetDatabaseConnection(string initialCatalog, string server = null, GenericCredentialInfo credentialInfo = null);

		/// <summary>
		///  Returns default kCura (Relativity) connection string
		/// </summary>
		/// <returns>Connection string from kCura.Data.RowDataGateway.Context()</returns>
		public static string GetDefaultConnectionString()
		{
			using (var conn = new kCura.Data.RowDataGateway.Context().GetConnection(false))
			{
				return conn.ConnectionString;
			}
		}
	}
}