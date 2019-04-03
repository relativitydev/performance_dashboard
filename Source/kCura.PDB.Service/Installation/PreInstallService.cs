namespace kCura.PDB.Service.Installation
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class PreInstallService : IPreInstallService
	{
		private readonly IAgentManagerService agentManagerService;

		public PreInstallService(IAgentManagerService agentManagerService)
		{
			this.agentManagerService = agentManagerService;
		}

		public ApplicationInstallResponse RunOnce()
		{
			throw new System.NotImplementedException();
		}

		public async Task<ApplicationInstallResponse> RunEveryTimeAsync()
		{
			var response = new ApplicationInstallResponse() { Success = false };
			var exceptionMessage = string.Empty;
			try
			{
				// Disable Agents
				exceptionMessage = Messages.Exception.PreInstallEventHandlerFailure;
				await this.agentManagerService.StopPerformanceDashboardAgentsAsync();
			}
			catch (Exception ex)
			{
				response.Message = string.Format(exceptionMessage, ex.ToString());
				return response;
			}

			response.Success = true;
			return response;
		}
	}
}
