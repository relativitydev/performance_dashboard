namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	[Description("Run cleanup on data tables")]
	public class DataTableCleanupTask : BaseProcessControlTask, IProcessControlTask
	{
		public DataTableCleanupTask(ISqlServerRepository sqlRepo, int agentId)
			: base(sqlRepo, agentId)
		{

		}
		public bool Execute(ProcessControl processControl)
		{
			SqlRepo.CleanupDataTables();
			return true;
		}

		public ProcessControlId ProcessControlID
		{
			get { return ProcessControlId.DataTableCleanup; }
		}
	}
}
