namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IServerRepository : IDbRepository
	{
		Task<Server> ReadAsync(int serverId);

		Task<IList<Server>> ReadAllActiveAsync();

		IList<Server> ReadAllActive();

		Task UpdateAsync(Server server);

		Task DeleteAsync(Server server);

		/// <summary>
		/// Interacts with EDDS.eddsdbo.Case table to grab all workspace ids
		/// </summary>
		/// <param name="serverId">The server id to grab all workspace ids from</param>
		/// <returns>WorkspaceIds for a given server id</returns>
		Task<IList<int>> ReadServerWorkspaceIdsAsync(int serverId);

		/// <summary>
		/// reads servers where IsQoSDeployed is false
		/// </summary>
		/// <returns>List of servers where IsQoSDeployed is false</returns>
		Task<IList<Server>> ReadServerPendingQosDeploymentAsync();

		/// <summary>
		/// Sets IsQoSDeployed to false for all active servers
		/// </summary>
		/// <returns>Task</returns>
		Task UpdateActiveServersPendingQosDeploymentAsync();

		/// <summary>
		/// Reads value of an EDDS Primary Standalone server
		/// </summary>
		/// <returns>ArtifactID of primary server with no associated cases</returns>
		Task<int?> ReadPrimaryStandaloneAsync();

		Task<bool> ReadWorkspaceExistsAsync(int workspaceId);
	}
}
