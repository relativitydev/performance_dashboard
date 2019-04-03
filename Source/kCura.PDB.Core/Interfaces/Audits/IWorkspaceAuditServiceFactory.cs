namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Threading.Tasks;

	public interface IWorkspaceAuditServiceFactory
	{
		Task<IWorkspaceAuditService> GetAuditService(int workspaceId, int hourId);
	}
}
