namespace kCura.PDB.EventHandler
{
	using System;
	using kCura.EventHandler;
	using kCura.EventHandler.CustomAttributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using Relativity.API;
	using Relativity.Services.Agent;

	[kCura.EventHandler.CustomAttributes.RunOnce(false)]
	[kCura.EventHandler.CustomAttributes.Description("Triggers forced execution of Install Server Scripts and Install Workspace Scripts processes")]
	[System.Runtime.InteropServices.Guid("11511440-116A-4F0D-B98B-FB35E82045E7")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.InstanceAndWorkspace)]
	public class RunEveryTimePostInstallEventHandler : PostInstallEventHandler
	{
		private IPostInstallService postInstallService;

		public RunEveryTimePostInstallEventHandler()
		{
		}

		public RunEveryTimePostInstallEventHandler(IPostInstallService postInstallService)
		{
			this.postInstallService = postInstallService;
		}

		public override Response Execute()
		{
			// If we're already initialized (Test path)
			if (this.postInstallService != null)
			{
				return this.ExecutePdb();
			}

			var logger = new TextLogger();
			var connectionFactory = new HelperConnectionFactory(this.Helper);
			var agentRepository = new AgentRepository(connectionFactory);
			var agentManager = this.Helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System);
			var agentManagerService = new AgentManagerService(agentManager, agentRepository, logger);

			var textLogger = new TextLogger();
			var resourceServerRepository = new ResourceServerRepository(connectionFactory);
			var refreshServerService = new RefreshServerService(textLogger, resourceServerRepository);
			var serverRepository = new ServerRepository(connectionFactory);
			var configurationRepository = new ConfigurationRepository(connectionFactory);
			this.postInstallService = new PostInstallService(agentManagerService, refreshServerService, serverRepository, configurationRepository, logger);
			
			var response = this.ExecutePdb();
			agentManager?.Dispose();
			return response;
		}

		private Response ExecutePdb()
		{
			var response = new Response
			{
				Success = false, //Assume failure until life proves rosy and beautiful
				Message = string.Empty
			};

			// Do work 
			try
			{
				// Enable all agents and create new ones
				var serviceResult = this.postInstallService.RunEveryTimeAsync().GetAwaiter().GetResult();
				if (serviceResult.Success == false)
				{
					response.Message = serviceResult.Message;
					return response;
				}
			}
			catch (Exception ex)
			{
				response.Message = string.Format(Messages.Exception.PostInstallFailure, ex);
				return response;
			}

			//Life is beautiful and everything worked
			response.Success = true;
			return response;
		}
	}
}
