namespace kCura.PDB.Service.Audits
{
	using kCura.PDB.Core.Interfaces.Audits;
	using Ninject.Modules;

	public class NoDataGridBindings : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>>().ToMethod(c => null);
		}
	}
}
