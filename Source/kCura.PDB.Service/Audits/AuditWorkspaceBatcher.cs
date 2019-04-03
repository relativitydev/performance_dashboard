namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;

	public class AuditWorkspaceBatcher : IAuditWorkspaceBatcher
	{
		private readonly IWorkspaceAuditServiceFactory workspaceAuditServiceFactory;
		private readonly ILogger logger;

		public AuditWorkspaceBatcher(IWorkspaceAuditServiceFactory workspaceAuditServiceFactory, ILogger logger)
		{
			this.workspaceAuditServiceFactory = workspaceAuditServiceFactory;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.UserExperience);
		}

		/// <inheritdoc />
		public async Task<IList<SearchAuditBatch>> CreateWorkspaceBatches(int serverId, int workspaceId, int hourId)
		{
			// Get the batch size
			var batchSize = AuditConstants.BatchSize; // Replace with config value?

			// Query the workspace for the total number of search audits
			var repo = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, hourId);
			long totalAudits = 0;
			try
			{
				totalAudits = await repo.ReadTotalAuditsForHourAsync(
					workspaceId,
					hourId,
					new List<AuditActionId> { AuditActionId.DocumentQuery });
			}
			catch (Exception ex)
			{
				// We weren't able to connect to this workspace.  Could be a bad workspace.
				this.logger.LogError($"Unable to connect to workspace {workspaceId}", ex);
			}

			return totalAudits == 0 ? new List<SearchAuditBatch>() : CreateBatches(totalAudits, batchSize, serverId, workspaceId, hourId);
		}

		internal static IList<SearchAuditBatch> CreateBatches(long totalAudits, int batchSize, int serverId, int workspaceId, int hourId)
		{
			var batches = new List<SearchAuditBatch>();
			var remainder = totalAudits % batchSize;
			var numberOfBatches = totalAudits / batchSize;
			for (var i = 0; i < numberOfBatches; ++i)
			{
				batches.Add(new SearchAuditBatch
				{
					ServerId = serverId,
					WorkspaceId = workspaceId,
					HourId = hourId,
					BatchSize = batchSize,
					BatchStart = i * batchSize
				});
			}

			if (remainder > 0)
			{
				batches.Add(new SearchAuditBatch
				{
					ServerId = serverId,
					WorkspaceId = workspaceId,
					HourId = hourId,
					BatchSize = (int)remainder,
					BatchStart = numberOfBatches * batchSize
				});
			}

			return batches;
		}
	}
}
