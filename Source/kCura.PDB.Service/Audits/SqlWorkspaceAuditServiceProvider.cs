namespace kCura.PDB.Service.Audits
{
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;

	public class SqlWorkspaceAuditServiceProvider : IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService>
	{
		private readonly ISqlAuditRepository sqlAuditRepository;
		private readonly IHourRepository hourRepository;
		private ISqlWorkspaceAuditService sqlWorkspaceAuditService;

		public SqlWorkspaceAuditServiceProvider(ISqlAuditRepository sqlAuditRepository, IHourRepository hourRepository)
		{
			this.sqlAuditRepository = sqlAuditRepository;
			this.hourRepository = hourRepository;
		}

		public ISqlWorkspaceAuditService GetService()
		{
			if (sqlWorkspaceAuditService == null)
			{
				sqlWorkspaceAuditService = new SqlWorkspaceAuditService(sqlAuditRepository, hourRepository);
			}
			return sqlWorkspaceAuditService;
		}
	}
}
