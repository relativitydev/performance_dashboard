namespace kCura.PDB.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.ProcessControls;
	using kCura.PDB.Service.Services;

	[kCura.Agent.CustomAttributes.Name(Names.Agent.WmiWorkerAgentName)]
	[Guid(Guids.Agent.WmiWorkerAgentGuidString)]
	public class WMIWorkerAgent : AgentBaseExtended
	{
		protected override void ExecutePdbAgent(CancellationToken cancellationToken)
		{
			var connectionFactory = new HelperConnectionFactory(this.Helper);
			var sqlService = new SqlServerRepository(connectionFactory);
			var logger = new CompositeLogger(new List<ILogger>()
				{
					new AgentLogger(this),
					new DatabaseLogger(sqlService.LogRepository, new LogService(sqlService.ConfigurationRepository, sqlService.LogRepository), this)
				});
			try
			{
				if (!sqlService.PerformanceExists())
				{
					logger.WithClassName().LogWarning("EDDSPerformance does not exist. Waiting for a QoS Manager to create the database.");
					return;
				}

				//can only debug once the EDDSPerformance database exists... b/c the config key lives there
				sqlService.IfInDebugModeLaunchDebugger();

				var eventSytemState = sqlService.EventRepository.ReadEventSystemStateAsync().GetAwaiter().GetResult();
				var serversPendingQosDeployment = sqlService.PerformanceServerRepository.ReadServerPendingQosDeploymentAsync().GetAwaiter().GetResult();

				//If the event system is running normal and there are no servers pending qos deployment
				if (eventSytemState == EventSystemState.Normal && !serversPendingQosDeployment.Any())
				{
					// run wmi tasks
					var wmiTaskManager = new WMITaskManager(logger, sqlService, this, this.Helper);
					wmiTaskManager.Run();
				}
			}
			catch (Exception ex)
			{
				logger.WithClassName().LogError($"WMI Worker Failed. Details: {ex.ToString()}");
			}
		}

		protected override bool ShowBeginEndMessages => true;

		public override string Name => Names.Agent.WmiWorkerAgentName;
	}
}
