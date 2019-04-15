namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[Description("Error log management")]
	public class ReadErrorLogTask : BaseProcessControlTask, IProcessControlTask
	{
		public ReadErrorLogTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{

		}

		public bool Execute(ProcessControl processControl)
		{
			if (SqlRepo.AdminScriptsInstalled() == false)
			{
				LogWarning("Installation of Performance Dashboard is incomplete. Please install the latest scripts from PDB's custom pages.");
				return true;
			}
			SqlRepo.SummarizeSqlServerPageouts();
			return true;
		}

		public ProcessControlId ProcessControlID
		{
			get { return ProcessControlId.ReadErrorLog; }
		}
	}
}
