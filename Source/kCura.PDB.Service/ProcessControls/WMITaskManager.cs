namespace kCura.PDB.Service.ProcessControls
{
	using System;
	using global::Relativity.API;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.ProcessControls.HealthPerformance;

	public class WMITaskManager : IWMITaskManager
	{
		private readonly ILogger _logger;
		private readonly ISqlServerRepository _sqlService;
		private readonly IAgentService agentService;
		private readonly IHelper _helper;

		public WMITaskManager(ILogger logger, ISqlServerRepository sqlServerRepo, IAgentService agentService, IHelper helper)
		{
			_logger = logger;
			_sqlService = sqlServerRepo;
			this.agentService = agentService;
			_helper = helper;
		}

		public void Run()
		{
			try
			{
				CollectPerformanceMetrics();
				RunAggregations();
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				_logger.LogError(message, "WMI Task Manager");
			}
		}

		public void CollectPerformanceMetrics()
		{
			_logger.LogVerbose("Getting WMI Task workers", "WMI Task Manager");

			var performanceTask = new HealthPerformanceFactory()
			{
				Logger = _logger,
				SqlService = _sqlService,
				AgentService = agentService
			};

			performanceTask.GetPerformanceMetrics();

			_logger.LogVerbose("Getting WMI Task workers - Success", "WMI Task Manager");
		}

		public void RunAggregations()
		{
			if (_sqlService.ClaimRollup(agentService.AgentID.ToString()))
			{
				//This agent has claimed the aggregation tasks
				_logger.LogVerbose("Calling RunAggregations", "WMI Task Manager");

				var aggregator = new AggregationTaskGroup(_logger, _sqlService, agentService.AgentID);
				aggregator.Execute();

				_logger.LogVerbose("Calling RunAggregations - Done", "WMI Task Manager");
			}
		}
	}
}
