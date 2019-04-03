namespace kCura.PDB.Core.Interfaces.Audits
{
	public interface IWorkspaceAuditServiceProvider<T>
		where T : IWorkspaceAuditService
	{
		T GetService();
	}
}
