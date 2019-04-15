namespace kCura.PDB.Service.DataGrid
{
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;

	public class DataGridWorkspaceAuditServiceProvider : IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService>
	{
		private readonly IDataGridSettingsService dataGridSettingsRepository;
		private readonly IArtifactRepository artifactRepository;
		private readonly IDataGridService dataGridService;
		private readonly IHourRepository hourRepository;
		private IDataGridWorkspaceAuditService workspaceAuditService;

		public DataGridWorkspaceAuditServiceProvider(
			IDataGridSettingsService dataGridSettingsRepository,
			IArtifactRepository artifactRepository,
			IDataGridService dataGridService,
			IHourRepository hourRepository)
		{
			this.dataGridSettingsRepository = dataGridSettingsRepository;
			this.artifactRepository = artifactRepository;
			this.dataGridService = dataGridService;
			this.hourRepository = hourRepository;
		}

		public IDataGridWorkspaceAuditService GetService()
		{
			if (workspaceAuditService == null)
			{
				workspaceAuditService = new DataGridWorkspaceAuditService(
					this.dataGridSettingsRepository,
					this.artifactRepository,
					this.dataGridService,
					this.hourRepository);
			}
			return workspaceAuditService;
		}
	}
}
