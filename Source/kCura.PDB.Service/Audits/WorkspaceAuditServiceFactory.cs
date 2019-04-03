namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Threading.Tasks;
	using global::Relativity.Toggles;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Toggles;
	using Ninject;

	public class WorkspaceAuditServiceFactory : IWorkspaceAuditServiceFactory
	{
		private readonly IRelativityApplicationRepository relativityApplicationRepository;
		private readonly IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService> sqlWorkspaceAuditServiceProvider;
		private readonly IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService> datagridWorkspaceAuditServiceProvider;
		private readonly IToggleProvider toggleProvider;
		private readonly ILogger logger;
		private readonly IDataGridCacheRepository dataGridCacheRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkspaceAuditServiceFactory"/> class.
		/// Constructs the WorkspaceAuditServiceFactory
		/// </summary>
		/// <param name="relativityApplicationRepository">relativity app repo</param>
		/// <param name="sqlWorkspaceAuditServiceProvider">provides the ISqlWorkspaceAuditService</param>
		/// <param name="datagridWorkspaceAuditServiceProvider"> *** This will be null if the data grid references can't be loaded *** </param>
		/// <param name="toggleProvider">toggle provider</param>
		/// <param name="logger">logger</param>
		/// <param name="dataGridCacheRepository">data grid cache repo</param>
		public WorkspaceAuditServiceFactory(
			IRelativityApplicationRepository relativityApplicationRepository,
			IWorkspaceAuditServiceProvider<ISqlWorkspaceAuditService> sqlWorkspaceAuditServiceProvider,
			IWorkspaceAuditServiceProvider<IDataGridWorkspaceAuditService> datagridWorkspaceAuditServiceProvider,
			IToggleProvider toggleProvider,
			ILogger logger,
			IDataGridCacheRepository dataGridCacheRepository)
		{
			this.relativityApplicationRepository = relativityApplicationRepository;
			this.sqlWorkspaceAuditServiceProvider = sqlWorkspaceAuditServiceProvider;
			this.datagridWorkspaceAuditServiceProvider = datagridWorkspaceAuditServiceProvider;
			this.toggleProvider = toggleProvider;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Audit);
			this.dataGridCacheRepository = dataGridCacheRepository;
		}

		public async Task<IWorkspaceAuditService> GetAuditService(int workspaceId, int hourId)
		{
			var dataGridAuditAnalysisEnabled = await this.toggleProvider.IsEnabledAsync<DataGridAuditAnalysisToggle>();

			if (dataGridAuditAnalysisEnabled == false)
			{
				await this.logger.LogVerboseAsync($"Using ISqlWorkspaceAuditService from toggle");
				return this.sqlWorkspaceAuditServiceProvider.GetService();
			}

			if (await this.MeetsDataGridRequirements(workspaceId, hourId))
			{
				await this.logger.LogVerboseAsync($"Using IDataGridWorkspaceAuditService");
				return this.datagridWorkspaceAuditServiceProvider.GetService();
			}

			await this.logger.LogVerboseAsync($"Using ISqlWorkspaceAuditService.");
			return this.sqlWorkspaceAuditServiceProvider.GetService();
		}

		internal async Task<bool> MeetsDataGridRequirements(int workspaceId, int hourId)
		{
			return this.DataGridMeetsVersionRequirements(workspaceId) &&
				   (await this.UseDataGridCache(workspaceId, hourId) ||
					await this.HasDataGridAudits(workspaceId, hourId));
		}

		internal Task<bool> UseDataGridCache(int workspaceId, int hourId)
		{
			// Query cache value for Workspace
			return this.dataGridCacheRepository.ReadUseDataGrid(workspaceId, hourId);
		}

		internal async Task<bool> HasDataGridAudits(int workspaceId, int hourId)
		{
			// load the assembly dynamically and check if there are any audits
			try
			{
				var hasAudits = await this.datagridWorkspaceAuditServiceProvider.GetService()
					.ReadAnyAuditsAsync(workspaceId, hourId, AuditConstants.RelevantAuditActionIds);
				if (hasAudits)
				{
					await this.dataGridCacheRepository.UpdateDataGridCache(workspaceId, hourId);
				}

				return hasAudits;
			}
			catch (Exception ex)
			{
				await this.logger.LogVerboseAsync($"Failed to call DataGrid with error: {ex}, fallback to SQL");
			}

			return false;
		}

		internal bool DataGridMeetsVersionRequirements(int workspaceId)
		{
			var dataGridAuditVersionString = this.relativityApplicationRepository.GetApplicationVersion(workspaceId, Guids.Application.DataGridForAudit);

			if (string.IsNullOrEmpty(dataGridAuditVersionString))
			{
				return false;
			}

			var dataGridAuditVersion = new Version(dataGridAuditVersionString);
			var minVersion = new Version(DataGridResourceConstants.DataGridAuditMinVersion);

			return dataGridAuditVersion >= minVersion;
		}
	}
}
