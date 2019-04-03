namespace kCura.PDB.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Threading;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using Ninject;

	[kCura.Agent.CustomAttributes.Name(Names.Agent.MetricManagerAgentName)]
	[Guid(Guids.Agent.MetricManagerAgentGuidString)]
	public class MetricManagerAgent : AgentBaseExtended
	{
		public override string Name => Names.Agent.MetricManagerAgentName;

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
				//can only debug, once the EDDSPerformance database exists... b/c the config key lives there
				sqlService.IfInDebugModeLaunchDebugger();

				using (var kernel = this.GetKernel())
				{

					var managerLogic = kernel.Get<MetricSystemManagerAgentLogic>();
					logger.WithClassName().LogVerbose($"Starting Manager Logic");
					managerLogic.Execute(cancellationToken).GetAwaiter().GetResult();
					logger.WithClassName().LogVerbose($"Finish Manager Logic, Start kernel dispose");
				}

				logger.WithClassName().LogVerbose($"Finish kernel Dispose");
			}
			catch (Exception ex)
			{
				logger.WithClassName().LogError($"Metric Manager Failed. Details: {ex.ToString()}");
			}
		}

		protected override bool ShowBeginEndMessages => true;
	}
}
