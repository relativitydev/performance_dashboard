namespace kCura.PDB.Service.DataGrid
{
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Service.DataGrid.Interfaces;
	using Ninject.Modules;

	public class DataGridBindings : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IDataGridService>().To<DataGridService>();
			this.Bind<IDataGridResponseAuditMapper>().To<DataGridResponseAuditMapper>();
			this.Bind<IAuditLogObjectManagerFactory>().To<AuditLogObjectManagerFactory>();
			this.Bind<IDataGridWorkspaceAuditService>().To<DataGridWorkspaceAuditService>();
			this.Bind<IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>>().To<DataGridWorkspaceAuditServiceProvider>();
		}
	}
}
