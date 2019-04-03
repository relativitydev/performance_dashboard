namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;

	public class ServerAuditService : IServerAuditService
	{
		private readonly IWorkspaceService workspaceService;
		private readonly IWorkspaceAuditServiceFactory workspaceAuditServiceFactory;
		private readonly ILogger logger;

		public ServerAuditService(
			IWorkspaceService workspaceService,
			IWorkspaceAuditServiceFactory workspaceAuditServiceFactory,
			ILogger logger)
		{
			this.workspaceService = workspaceService;
			this.workspaceAuditServiceFactory = workspaceAuditServiceFactory;
			this.logger = logger.WithClassName();
		}

		/// <inheritdoc />
		public async Task<long> ReadTotalAuditsForHourAsync(int serverId, int hourId, IList<AuditActionId> actionTypes)
		{
			// Grab the workspaceIds for the server
			var workspaceIds = await this.workspaceService.ReadAvailableWorkspaceIdsAsync(serverId);

			// Grab the total number of audits we're going to need to analyze so that we may correctly batch
			var totalAudits = (long)0;
			foreach (var workspaceId in workspaceIds)
			{
				try
				{
					// Grab the correct audit repository (Sql/DataGrid)
					var repo = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, hourId);

					totalAudits += await repo.ReadTotalAuditsForHourAsync(
						workspaceId,
						hourId,
						actionTypes);
				}
				catch (KeyNotFoundException ex)
				{
					// The workspace doesn't exist anymore.  Log the error, but swallow it.
					await this.logger.LogWarningAsync(
						$"Exception thrown when trying to ReadTotalAudits for workspaceId {workspaceId}, serverId {serverId}, hourId {hourId}, details: {ex}");
				}
			}

			return totalAudits;
		}

		public async Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int serverId, int hourId, IList<AuditActionId> actionTypes)
		{
			// Grab the workspaceIds for the server
			var workspaceIds = await this.workspaceService.ReadAvailableWorkspaceIdsAsync(serverId);

			// Grab the total number of unique users so that we know if the hour was active ( > 1 )
			var totalUniqueUsers = new List<int>();
			foreach (var workspaceId in workspaceIds)
			{
				try
				{
					// Grab the correct audit repository (Sql/DataGrid)
					var repo = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, hourId);

					totalUniqueUsers.AddRange(await repo.ReadUniqueUsersForHourAuditsAsync(
						workspaceId,
						hourId,
						actionTypes));
				}
				catch (KeyNotFoundException ex)
				{
					// The workspace doesn't exist anymore.  Log the error, but swallow it.
					await this.logger.LogWarningAsync(
						$"Exception thrown when trying to ReadUniqueUsers for workspaceId {workspaceId}, serverId {serverId}, hourId {hourId}, details: {ex}");
				}
			}

			return totalUniqueUsers.Distinct().Count();
		}

		public Task<long> ReadTotalAuditExecutionTimeForHourAsync(int serverId, int hourId, IList<AuditActionId> actionTypes)
		{
			// NOTE: DO NOT USE, as the Data Grid side has not been implemented yet
			throw new NotImplementedException("Data Grid Audit API has not implemented, cannot use reliably");

			/*
						// Grab the workspaceIds for the server
			var workspaceIds = await this.GetWorkspacesForServerId(serverId);

			// Grab the total number of unique users so that we know if the hour was active ( > 1 )
			var totalExecutionTime = (long)0;
			foreach (var workspaceId in workspaceIds)
			{
				try
				{
					// Grab the correct audit repository (Sql/DataGrid)
					var repo = await this.workspaceAuditServiceFactory.GetAuditService(workspaceId, hourId);

					totalExecutionTime += await repo.ReadTotalAuditExecutionTimeForHourAsync(
						workspaceId,
						hourId,
						actionTypes);
				}
				catch (KeyNotFoundException ex)
				{
					// The workspace doesn't exist anymore.  Log the error, but swallow it.
					await this.logger.LogWarningAsync(
						$"Exception thrown when trying to ReadTotalExecutionTime for workspaceId {workspaceId}, serverId {serverId}, hourId {hourId}, details: {ex}");
				}				
			}

			return totalExecutionTime;
			//*/
		}
	}
}
