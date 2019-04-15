namespace kCura.PDB.Service.Workspace
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Workspace;

	public class WorkspaceService : IWorkspaceService
	{
		private readonly IServerRepository serverRepository;
		private readonly IArtifactRepository artifactRepository;
		private readonly ILogger logger;

		public WorkspaceService(IServerRepository serverRepository, IArtifactRepository artifactRepository, ILogger logger)
		{
			this.serverRepository = serverRepository;
			this.artifactRepository = artifactRepository;
			this.logger = logger.WithClassName();
		}

		/// <inheritdoc />
		public async Task<IList<int>> ReadAvailableWorkspaceIdsAsync(int serverId)
		{
			// Read all the workspaces for the server
			var workspaceIds = await this.serverRepository.ReadServerWorkspaceIdsAsync(serverId);

			// Check the workspace is available
			var availableWorkspaces = await workspaceIds
				.Select(async ws => new { WorkspaceId = ws, Available = await this.WorkspaceIsAvailableAsync(ws) })
				.WhenAllStreamed();

			// Return workspace ids only that are available
			return availableWorkspaces
				.Where(ws => ws.Available)
				.Select(ws => ws.WorkspaceId)
				.ToList();
		}

		/// <inheritdoc />
		public async Task<bool> WorkspaceIsAvailableAsync(int workspaceId)
		{
			try
			{
				// Checks if we have access to the workspace audit table
				var databaseAccessTask = this.artifactRepository.TestDatabaseAccessAsync(workspaceId);
				var workspaceUpgradeComplete = this.artifactRepository.IsWorkspaceUpgradeComplete(workspaceId);

				return (await Task.WhenAll(databaseAccessTask, workspaceUpgradeComplete)).All(b => b == true);
			}
			catch (Exception ex)
			{
				await this.logger.LogWarningAsync($"Could not connect to workspace database EDDS{workspaceId}", ex);
				return false;
			}
		}
	}
}
