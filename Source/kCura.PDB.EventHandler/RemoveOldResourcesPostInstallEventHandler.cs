namespace kCura.PDB.EventHandler
{
	using System;
	using System.Runtime.InteropServices;
	using kCura.EventHandler;
	using kCura.EventHandler.CustomAttributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Logging;

	[RunOnce(false)]
	[Guid("940ED540-DA03-41B9-AC23-DFE13FB4FF50")]
	[Description("PostInstall handler for removing old resource files after install.")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.InstanceAndWorkspace)]
	public class RemoveOldResourcesPostInstallEventHandler : PostInstallEventHandler
	{
		private ISqlServerRepository sqlServerRepository;
		private readonly TextLogger logger = new TextLogger();

		public RemoveOldResourcesPostInstallEventHandler()
		{
		}

		public RemoveOldResourcesPostInstallEventHandler(ISqlServerRepository sqlServerRepository)
		{
			this.sqlServerRepository = sqlServerRepository;
		}

		public override Response Execute()
		{
			// Initialize
			this.logger.Clear();
			var response = new Response { Success = false, Message = "" };
			if (this.sqlServerRepository == null)
			{
				var connectionFactory = new HelperConnectionFactory(this.Helper);
				this.sqlServerRepository = new SqlServerRepository(connectionFactory);
			}

			// TODO -- Separate this logic to a separate service
			try
			{
				this.logger.LogVerbose($"Removing old application references");
				sqlServerRepository.DeploymentRepository.RemoveOldApplicationReferences();
				var activeWorkspaceId = this.Helper.GetActiveCaseID();
				if (activeWorkspaceId > 0)
				{
					this.logger.LogVerbose($"Removing old agent guids from workspace {activeWorkspaceId}");
					Guids.Agent.AgentGuidsToRemove.ForEach(guid =>
					{
						this.logger.LogVerbose($"Removing old agent guid {guid}");
						sqlServerRepository.DeploymentRepository.RemoveOldApplicationReferencesFromWorkspace(guid, activeWorkspaceId);
					});
				}

				this.logger.LogVerbose($"Removing old resource files");
				sqlServerRepository.DeploymentRepository.RemoveOldResourceFiles(); // Clean out the dummy files if they still exist
			}
			catch (Exception ex)
			{
				this.logger.LogError($"Failed to execute {nameof(RemoveOldResourcesPostInstallEventHandler)}", ex);
				response.Message = this.logger.Text;
				return response;
			}

			response.Success = true;
			response.Message = "Removed old resource files.";
			return response;
		}
	}
}
