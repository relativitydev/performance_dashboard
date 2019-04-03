namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	[Description("Load Application Health Summary")]
	public class LoadApplicationSummaryTask : BaseProcessControlTask, IProcessControlTask
	{
		public LoadApplicationSummaryTask(ISqlServerRepository sqlRepo, int agentId)
			: base(sqlRepo, agentId)
		{
			this.sqlRepo = sqlRepo;
		}

		private readonly ISqlServerRepository sqlRepo;

		public bool Execute(ProcessControl processControl)
		{
			sqlRepo.PerformanceSummaryRepository.LoadApplicationHealthSummary(processControl.LastProcessExecDateTime);
			return true;
		}

		public ProcessControlId ProcessControlID {
			get { return ProcessControlId.ApplicationMetricsDWLoad; }
		}
	}
}
