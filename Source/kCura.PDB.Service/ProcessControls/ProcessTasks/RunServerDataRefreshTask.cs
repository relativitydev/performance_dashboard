namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;

	[Description("Run Server Refresh")]
	public class RunServerDataRefreshTask : BaseProcessControlTask, IProcessControlTask
	{
		public RunServerDataRefreshTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId, IRefreshServerService refreshServerSvc = null)
			: base(logger.WithClassName(), sqlRepo, agentId)
		{
			_logger = logger.WithClassName();
			_refreshServerSvc = refreshServerSvc ?? new RefreshServerService(logger, sqlRepo.ResourceServerRepository);
		}

		private readonly ILogger _logger;
		private readonly IRefreshServerService _refreshServerSvc;

		public bool Execute(ProcessControl processControl)
		{
			var currentServers = _refreshServerSvc.GetServerList();
			_logger.LogInformation($"Total Server :{currentServers.Count}", "GetServerList");

			if (currentServers != null && currentServers.Any())
			{
				//update server list
				_refreshServerSvc.UpdateServerList(currentServers);
				
				//refresh server list since new list may include new servers added
				var latestServers = this.SqlRepo.PerformanceServerRepository.ReadAllActive();
				_logger.LogInformation($"Total Server count after update: {latestServers.Count}", "GetServerList");
			}
			else
				Log("RunServerTask Called - CurrentServers is null!");

			return true;
		}

		public ProcessControlId ProcessControlID => ProcessControlId.ServerInfoRefresh;
	}
}
