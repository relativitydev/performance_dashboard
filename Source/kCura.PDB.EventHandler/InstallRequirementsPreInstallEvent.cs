namespace kCura.PDB.EventHandler
{
	using System;
	using kCura.EventHandler;
	using kCura.EventHandler.CustomAttributes;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Logging;
	using Ninject;
	using Relativity.API;
	using Relativity.Services.Agent;

	[kCura.EventHandler.CustomAttributes.RunOnce(false)]
	[kCura.EventHandler.CustomAttributes.Description("Checks required conditions for installation and upgrades to Performance Dashboard")]
	[System.Runtime.InteropServices.Guid("0002AB93-832B-4FF0-B2F9-7A445A557496")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.InstanceAndWorkspace)]
	public class InstallRequirementsPreInstallEvent : PreInstallEventHandler
	{
		private IPreInstallService preInstallService;

		public InstallRequirementsPreInstallEvent()
		{
		}

		public InstallRequirementsPreInstallEvent(IPreInstallService preInstallService)
		{
			this.preInstallService = preInstallService;
		}

		public override Response Execute()
		{
			// Initialize
			if (this.preInstallService != null)
			{
				return this.ExecutePDB();
			}

			var logger = new TextLogger();
			var connectionFactory = new HelperConnectionFactory(this.Helper);
			var agentRepository = new AgentRepository(connectionFactory);
			var agentManagerProxy = this.Helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System);
			var agentManagerService = new AgentManagerService(agentManagerProxy, agentRepository, logger);
			this.preInstallService = new PreInstallService(agentManagerService);


			var response = this.ExecutePDB();
			agentManagerProxy?.Dispose();
			return response;
		}

		private Response ExecutePDB()
		{
			var response = new Response
			{
				Success = false, //Assume failure until life proves rosy and beautiful
				Message = string.Empty
			};

			// TODO -- Separate this logic to a separate service
			try
			{
				var serviceResponse = this.preInstallService.RunEveryTimeAsync().GetAwaiter().GetResult();
				if (!serviceResponse.Success)
				{
					response.Message = serviceResponse.Message;
					return response;
				}
			}
			catch (Exception ex)
			{
				response.Message = $"Checking requirements for installation failed: {ex.Message}";
				return response;
			}

			//Checks passed
			response.Success = true;
			return response;
		}
	}
}
