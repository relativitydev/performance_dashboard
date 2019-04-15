namespace kCura.PDB.Service.DataGrid.Interfaces
{
	using kCura.AuditUI2.Services.ExternalAuditLog;

	public interface IAuditLogObjectManagerFactory
	{
		IExternalAuditLogObjectManager GetManager();
	}
}
