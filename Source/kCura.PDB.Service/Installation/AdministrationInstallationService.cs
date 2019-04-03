namespace kCura.PDB.Service.Installation
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.ScriptInstallation;

	/// <summary>
	/// Ideally, we would use RoundhousE for this. We need to get the .exe out of the DatabaseMigrations project references so we can reference
	/// that project from custom pages. For now, we just have stored procedures, so we can drop and create them without worrying about versioning.
	/// </summary>
	public class AdministrationInstallationService : IAdministrationInstallationService
	{
		private readonly IDatabaseDeployer databaseDeployer;
		private readonly IRefreshServerService refreshServerService;
		private readonly IServerRepository serverRepository;
		private readonly IAdministrationInstallationRepository administrationInstallationRepository; // To get rid of sqlServerRepo dependency

		public AdministrationInstallationService(IDatabaseDeployer databaseDeployer, IServerRepository serverRepository, IRefreshServerService refreshServerService, IAdministrationInstallationRepository administrationInstallationRepository)
		{
			this.databaseDeployer = databaseDeployer;
			this.serverRepository = serverRepository;
			this.refreshServerService = refreshServerService;
			this.administrationInstallationRepository = administrationInstallationRepository;
		}

		/// <summary>
		/// Verifies that the credentials are valid
		/// </summary>
		/// <param name="credentialInfo">The credential information</param>
		/// <returns>results</returns>
		public bool CredentialsAreValid(GenericCredentialInfo credentialInfo)
		{
			//Get a list of active SQL servers and check each one
			var servers = this.serverRepository.ReadAllActive()
							.Where(s => s.ServerType == ServerType.Database);
			foreach (var server in servers)
			{
				this.administrationInstallationRepository.HasDbccPermissions(server.ServerName, credentialInfo);
			}

			//This will be true unless we failed the permissions check for a server above or a DB error occurred
			return true;
		}

		/// <summary>
		/// Installs all scripts that require admin privileges
		/// </summary>
		/// <param name="credentialInfo">The credential information</param>
		/// <returns>results</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
		public ScriptInstallationResults InstallScripts(GenericCredentialInfo credentialInfo)
		{
			var results = new ScriptInstallationResults();

			try
			{
				//Context setup with admin credentials
				results.AppendMessage("Setting database context...");
				this.administrationInstallationRepository.InstallPrimaryServerAdminScripts(credentialInfo);

				//Do a server refresh prior to deployment
				var currentServers = refreshServerService.GetServerList();
				if (currentServers != null && currentServers.Any())
					refreshServerService.UpdateServerList(currentServers);

				//Get an array of all SQL servers registered in Relativity
				results.AppendMessage("Retrieving server list...");

				var servers = this.serverRepository.ReadAllActive()
								.Where(s => s.ServerType == ServerType.Database);
				foreach (var server in servers)
				{
					//Install any per-server scripts that need admin privileges
					results.AppendMessage($"Installing scripts on {server.ServerName}...");
					var deploymentResult = databaseDeployer.DeployResource(server, credentialInfo);
					HandleDeploymentResponse(deploymentResult, results);

					results.AppendMessage($"Scripts installed on {server.ServerName}.");
				}

				results.AppendMessage("Finished installing per-server scripts.");

				//Update the configuration to indicate that administrative scripts have been installed
				results.AppendMessage("Updating script installation history...");
				this.administrationInstallationRepository.UpdateAdminScriptsRun();
				results.AppendMessage("Installation complete.");

				results.Success = true;
			}
			catch (Exception e)
			{
				results.AppendMessage($"Installation failed. {e}");
				results.Success = false;
			}

			return results;
		}
		
		public static void HandleDeploymentResponse(MigrationResultSet deploymentResults, ScriptInstallationResults installResults)
		{
			deploymentResults.Messages
				.Where(m => m.Severity != LogSeverity.Debug)
				.Select(m => m.Message)
				.ForEach(m => installResults.AppendMessage(
					"Deploying " + Names.Database.PdbResource + $" script message: {m}"));
		}

	}
}
