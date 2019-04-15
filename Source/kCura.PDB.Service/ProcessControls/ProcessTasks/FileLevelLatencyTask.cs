namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[Description(FileLevelLatencyTask.FileLevelLatencyTaskDescription)]
	public class FileLevelLatencyTask : BaseProcessControlTask, IProcessControlTask
	{
		public FileLevelLatencyTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{

		}

		private const String FileLevelLatencyTaskDescription = "Database File Level Latency Details";

		public ProcessControlId ProcessControlID { get { return ProcessControlId.CollectWaitStatistics; } }

		public bool Execute(ProcessControl processControl)
		{
			/*
			TODO -- Refactor this mess in the future
			-- Get EDDSPerformance server instead of PrimarySqlServer (currently tied together)
			//*/
			var registeredSqlServers = SqlRepo.GetRegisteredSQLServers();
			var primary = this.SqlRepo.PrimarySqlServerRepository.GetPrimarySqlServer();
			ExecuteForServers(server =>
			{
				if (server.ServerTypeId == (int)ServerType.Database
				    && registeredSqlServers.Select(s => s.Name).Any(s => s.ToLower() == server.ServerName.ToLower()))
				{
					SqlRepo.FileLatencyRepository.ExecuteSaveFileLevelLatencyDetails(server.ServerName, primary.Name);
				}
			});
			return true;
		}
	}
}
