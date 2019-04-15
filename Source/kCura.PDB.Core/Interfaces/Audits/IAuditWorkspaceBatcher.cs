namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.Audits;

	public interface IAuditWorkspaceBatcher
	{
		/// <summary>
		/// Creates batches for the given hour/server/workspace
		/// </summary>
		/// <param name="serverId">ServerId to create (search) audit batches for.</param>
		/// <param name="workspaceId">WorkspaceId to create (search) audit batches for.</param>
		/// <param name="hourId">HourId to create (search) audit batches for.</param>
		/// <returns>List of SearchAuditBatches to create</returns>
		Task<IList<SearchAuditBatch>> CreateWorkspaceBatches(int serverId, int workspaceId, int hourId);
	}
}
