namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Data;
	using System.Data.Common;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IConnectionFactory
	{
		IDbConnection GetEddsConnection(GenericCredentialInfo credentialInfo = null);

		IDbConnection GetEddsPerformanceConnection();

		IDbConnection GetEddsQosConnection(string server = null);

		Task<IDbConnection> GetWorkspaceConnectionAsync(int workspaceId);

		IDbConnection GetWorkspaceConnection(int workspaceId);

		IDbConnection GetPdbResourceConnection(string server = null);

		IDbConnection GetMasterConnection(string server = null, GenericCredentialInfo credentialInfo = null);

		IDbConnection GetTargetConnection(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null);

		string GetTargetConnectionString(string targetDatabase, string targetServer, GenericCredentialInfo credentialInfo = null);
	}
}
