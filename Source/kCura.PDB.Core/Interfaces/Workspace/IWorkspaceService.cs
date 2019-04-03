namespace kCura.PDB.Core.Interfaces.Workspace
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IWorkspaceService
	{
		/// <summary>
		/// Reads all the workspaces on the server and checks that the database is available for queries
		/// </summary>
		/// <param name="serverId">The server to read all the workspaces for</param>
		/// <returns>List of workspace ids that are available</returns>
		Task<IList<int>> ReadAvailableWorkspaceIdsAsync(int serverId);

		/// <summary>
		/// Tests if a workspace is available for queries
		/// </summary>
		/// <param name="workspaceId">The workspace to test</param>
		/// <returns>True if the workspace is available</returns>
		Task<bool> WorkspaceIsAvailableAsync(int workspaceId);
	}
}
