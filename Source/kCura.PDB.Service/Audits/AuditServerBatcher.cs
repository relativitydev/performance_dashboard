namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Transactions;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models.Audits;

	public class AuditServerBatcher : IAuditServerBatcher
	{
		private readonly IAuditWorkspaceBatcher auditWorkspaceBatcher;
		private readonly IWorkspaceService workspaceService;
		private readonly ISearchAuditBatchRepository searchAuditBatchRepository;
		private readonly IMetricDataService metricDataService;
		private readonly ILogger logger;

		public AuditServerBatcher(
			IAuditWorkspaceBatcher auditWorkspaceBatcher,
			IWorkspaceService workspaceService,
			ISearchAuditBatchRepository searchAuditBatchRepository,
			IMetricDataService metricDataService,
			ILogger logger)
		{
			this.auditWorkspaceBatcher = auditWorkspaceBatcher;
			this.workspaceService = workspaceService;
			this.searchAuditBatchRepository = searchAuditBatchRepository;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Audit);
		}

		/// <inheritdoc />
		public async Task<IList<int>> CreateServerBatches(int metricDataId)
		{
			var metricData = await this.metricDataService.GetMetricData(metricDataId);

			if (!metricData.ServerId.HasValue || metricData.Server == null)
			{
				throw new Exception($"Cannot create batches, metric data {metricDataId} does not have an associated server");
			}

			// Read all the workspaces for the server
			var workspaceIds = await this.workspaceService.ReadAvailableWorkspaceIdsAsync(metricData.Server.ServerId);

			await this.logger.LogVerboseAsync($"Creating batches for {workspaceIds.Count} workspaces for server {metricData.Server.ServerId}");

			// Create Batches across all workspaces
			var workspaceBatches = (await workspaceIds
				.Select(id => this.auditWorkspaceBatcher.CreateWorkspaceBatches(metricData.ServerId.Value, id, metricData.Metric.HourId))
					.WhenAllStreamed(2))
				.SelectMany(r => r)
				.ToList();

			await this.logger.LogVerboseAsync($"Creating {workspaceBatches.Count} batches for {workspaceIds.Count} workspaces for server {metricData.Server.ServerId}");

			try
			{
                // Create the HourSearchAuditBatch
				var hourSearchAuditBatchId = await this.searchAuditBatchRepository.CreateHourSearchAuditBatch(
					metricData.Metric.HourId,
					metricData.ServerId.Value,
					workspaceBatches.Count);

                // Delete any existing searchAuditBatches for hourSearchAuditBatchId
			    await this.searchAuditBatchRepository.DeleteAllBatchesAsync(hourSearchAuditBatchId);

                if (workspaceBatches.Any())
				{
                    // set the HourSearchAuditBatchId created above
				    workspaceBatches.ForEach(b => b.HourSearchAuditBatchId = hourSearchAuditBatchId);
					await this.searchAuditBatchRepository.CreateBatches(workspaceBatches);
				}
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to create audit batches for server {metricData.Server.ServerId}. Attempted to create {workspaceBatches.Count} batches for {workspaceIds.Count} workspaces", ex);
				throw;
			}

			var finalSearchAuditBatches = await this.searchAuditBatchRepository.ReadBatchesByHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value);

			if (workspaceBatches.Count != finalSearchAuditBatches.Count)
			{
				throw new Exception($"Didn't create correct number of batches for {metricDataId} -- W:{workspaceBatches.Count} != F:{finalSearchAuditBatches.Count}");
			}

			return finalSearchAuditBatches
				.Select(b => b.Id)
				.Distinct()
				.ToList();
		}

		internal bool BatchAlreadyExists(IList<SearchAuditBatch> existingBatches, SearchAuditBatch batch) =>
			existingBatches.Any(batchExists =>
				batchExists.HourId == batch.HourId
				&& batchExists.WorkspaceId == batch.WorkspaceId
				&& batchExists.BatchStart == batch.BatchStart
				&& batchExists.BatchSize == batch.BatchSize);
	}
}
