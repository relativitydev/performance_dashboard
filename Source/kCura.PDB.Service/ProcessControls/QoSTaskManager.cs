namespace kCura.PDB.Service.ProcessControls
{
	using System;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.BISSummary;

	public class QoSTaskManager : IQoSTaskManager
	{
		private ILogger _logger;
		private ISqlServerRepository _sqlService;
		private string _agentId;

		public QoSTaskManager(ILogger logger, ISqlServerRepository sqlServerRepo, IAgentService agentService)
		{
			_logger = logger;
			_sqlService = sqlServerRepo;
			_agentId = agentService.AgentID.ToString();
		}

		public void Run()
		{
			try
			{
				if (_sqlService.ClaimLookingGlass(_agentId))
				{
					_logger.LogVerbose("Calling RunQoSTasks", "Quality Task Manager");

					var agentArtifactId = 0;
					int.TryParse(_agentId, out agentArtifactId);
					var manager = new LookingGlassTaskGroup(_sqlService, _logger);
					manager.Execute(agentArtifactId);

					_logger.LogVerbose("Calling RunQoSTasks - Done", "Quality Task Manager");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				_logger.LogError(message, "Quality Task Manager");
			}
		}
	}
}
