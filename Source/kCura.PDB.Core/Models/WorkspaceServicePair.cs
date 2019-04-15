namespace kCura.PDB.Core.Models
{
	using kCura.PDB.Core.Interfaces.Audits;

	public class WorkspaceServicePair
	{
		public int WorkspaceId { get; set; }
		public IWorkspaceAuditService WorkspaceAuditService { get; set; }
	}
}
