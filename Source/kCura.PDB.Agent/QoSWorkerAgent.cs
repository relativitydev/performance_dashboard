namespace kCura.PDB.Agent
{
	using System;
	using System.Runtime.InteropServices;
	using kCura.PDB.Core.Interfaces.Services;
	using System.Collections.Generic;
	using System.Threading;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Services;
	using Ninject;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Logging;

	[kCura.Agent.CustomAttributes.Name(Names.Agent.QosWorkerAgentName)]
	[Guid(Guids.Agent.QosWorkerAgentGuidString)]
	public class QoSWorkerAgent : AgentBaseExtended
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
					logger.WithClassName()
						.LogWarning("EDDSPerformance does not exist. Waiting for a QoS Manager to create the database.");
					return;
				}

				//can only debug, once the EDDSPerformance database exists... b/c the config key lives there
				sqlService.IfInDebugModeLaunchDebugger();

				// start metric event system
				using (var kernel = this.GetKernel())
				{
					var managerLogic = kernel.Get<MetricSystemWorkerAgentLogic>();
					logger.WithClassName().LogVerbose($"Start Worker Logic");

					managerLogic.Execute(cancellationToken).GetAwaiter().GetResult();

					logger.WithClassName().LogVerbose($"Finish Worker Logic, Start kernel dispose");
				}

				logger.WithClassName().LogVerbose($"Finish kernel dispose");
			}
			catch (Exception ex)
			{
				logger.WithClassName().LogError($"QoS Worker Failed. Details: {ex.ToString()}");
			}
		}

		protected override bool ShowBeginEndMessages => true;

		public override string Name => Names.Agent.QosWorkerAgentName;
	}
}
