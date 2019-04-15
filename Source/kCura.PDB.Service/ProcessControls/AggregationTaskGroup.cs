namespace kCura.PDB.Service.ProcessControls
{
	using System;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck;

	public class AggregationTaskGroup
	{
		public AggregationTaskGroup(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
		{
			this.logger = logger;
			this.sqlRepo = sqlRepo;
			this.AgentId = agentId;
		}

		private readonly ILogger logger;
		private readonly ISqlServerRepository sqlRepo;
		private readonly int AgentId;

		public void Execute()
		{
			this.logger.WithClassName().LogVerbose("RunAggregations Called");

			try
			{
				// var wmiHelper = new WMIHelper(this.logger);
				var manager = new ProcessControlTaskManager(this.sqlRepo, this.logger);
				manager.Subscribe(
					new RunServerDataRefreshTask(this.logger, this.sqlRepo, this.AgentId),
					new ApplicationPerformanceTask(this.logger, this.sqlRepo, this.AgentId),
					new LoadApplicationSummaryTask(this.sqlRepo, this.AgentId),
					new LoadServerHealthSummaryTask(this.logger, this.sqlRepo, this.AgentId),
					new CollectWaitStatisticsTask(this.sqlRepo, this.AgentId),
					new FileLevelLatencyTask(this.logger, this.sqlRepo, this.AgentId),
					new ReadErrorLogTask(this.logger, this.sqlRepo, this.AgentId),
					new MonitorVirtualLogFilesTask(this.logger, this.sqlRepo, this.AgentId),
					new CycleErrorLogTask(this.logger, this.sqlRepo, this.AgentId),
					new EnvironmentCheckRelativityTask(this.logger, this.sqlRepo, this.AgentId),
					new EnvironmentCheckSqlConfigTask(this.logger, this.sqlRepo, this.AgentId),
					new EnvironmentCheckServerInfoTask(this.logger, this.sqlRepo, this.AgentId),
					new DataTableCleanupTask(this.sqlRepo, this.AgentId));

				// Analytics checks temporarily disabled
				// new AnalyticsCheckProcessorCheck(wmiHelper, this.sqlRepo, dataContext, this.AgentId),
				// new AnalyticsCheckMemoryCheck(wmiHelper, workspaceRepository, this.logger, this.sqlRepo, dataContext, this.AgentId),
				manager.Execute();

				this.logger.WithClassName().LogVerbose("RunAggregations Called - Success");

			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.logger.WithClassName().LogError($"RunAggregations Called - Failure. Details: {message}");
			}

		}
	}
}
