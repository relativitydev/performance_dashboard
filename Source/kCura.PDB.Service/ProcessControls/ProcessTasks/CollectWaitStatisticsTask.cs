namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	[Description("Collect Wait Statistics")]
	public class CollectWaitStatisticsTask : BaseProcessControlTask, IProcessControlTask
	{
		public CollectWaitStatisticsTask(ISqlServerRepository sqlRepo, int agentId)
			: base(sqlRepo, agentId)
		{
			initialDatabase = Names.Database.EddsPerformance;
		}

		public bool Execute(ProcessControl processControl)
		{
			//Run wait monitor
			SqlRepo.ExecuteWaitMonitor(AgentId);
			return true;
		}

		public ProcessControlId ProcessControlID
		{
			get { return ProcessControlId.CollectWaitStatistics; }
		}
	}
}
