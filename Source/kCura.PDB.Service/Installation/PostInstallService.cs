namespace kCura.PDB.Service.Installation
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class PostInstallService : IPostInstallService
	{
		private readonly IAgentManagerService agentManagerService;
		private readonly IRefreshServerService refreshServerService;
		private readonly IServerRepository serverRepository;
		private readonly IConfigurationRepository configurationRepository;
		private readonly ILogger logger;

		public PostInstallService(
			IAgentManagerService agentManagerService,
			IRefreshServerService refreshServerService,
			IServerRepository serverRepository,
			IConfigurationRepository configurationRepository,
			ILogger logger)
		{
			this.agentManagerService = agentManagerService;
			this.refreshServerService = refreshServerService;
			this.serverRepository = serverRepository;
			this.configurationRepository = configurationRepository;
			this.logger = logger;
		}

		public ApplicationInstallResponse RunOnce()
		{
			throw new System.NotImplementedException();
		}

		public async Task<ApplicationInstallResponse> RunEveryTimeAsync()
		{
			var response = new ApplicationInstallResponse { Success = false, Message = string.Empty };
			var exceptionError = string.Empty;
			try
			{
				// Do a server refresh to make sure that server table is populated
				exceptionError = Messages.Exception.PostInstallServerRefreshFailure;
				var currentServers = this.refreshServerService.GetServerList();
				if (currentServers.Any())
				{
					this.refreshServerService.UpdateServerList(currentServers);
				}

				// Flag Deploy EDDSQoS to all database servers
				exceptionError = Messages.Exception.PostInstallUpdateServerQoSFailure;
				await this.serverRepository.UpdateActiveServersPendingQosDeploymentAsync();

				// Re-enable all agents
				exceptionError = Messages.Exception.PostInstallAgentEnableFailure;
				await this.agentManagerService.StartPerformanceDashboardAgentsAsync();

				// Read instance setting configuration and Create agents as needed
				exceptionError = Messages.Exception.PostInstallAgentCreateFailure;
				var configs = await this.configurationRepository.ReadEddsConfigurationInfoAsync(ConfigurationKeys.Section,
					ConfigurationKeys.CreateAgentsOnInstall);
				if (configs.Any())
				{
					var machineName = configs.FirstOrDefault(c => string.IsNullOrWhiteSpace(c.MachineName) == false)?.MachineName;
					if (string.IsNullOrEmpty(machineName))
					{
						this.logger.LogWarning(Messages.Warning.PostInstallAgentCreateWarning);
					}
					else
					{
						var createdAgentIds = await this.agentManagerService.CreatePerformanceDashboardAgents(machineName);
					}
				}
			}
			catch (Exception ex)
			{
				response.Message = string.Format(exceptionError, ex.ToString());
				return response;
			}

			response.Success = true;
			return response;
		}
	}
}
