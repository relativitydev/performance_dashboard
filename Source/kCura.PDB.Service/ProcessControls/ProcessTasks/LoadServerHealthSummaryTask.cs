namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System.ComponentModel;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	[Description("Load Server Health Summary")]
	public class LoadServerHealthSummaryTask : BaseProcessControlTask, IProcessControlTask
	{
		public LoadServerHealthSummaryTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{

		}

		public bool Execute(ProcessControl processControl)
		{
			Log("Calling RunPopulateFactData");
			var populateResult = this.SqlRepo.PopulateFactTable();
			if (populateResult == null)
				return true;
			Log("Calling RunPopulateFactData - Success");
			if (populateResult.IsRunsuccessfully.HasValue && populateResult.IsRunsuccessfully.Value)
			{
				Log("Loading Server Performance health completed");
			}
			else
			{
				LogError(populateResult.ErrorMessage);
			}

			return true;
		}

		public ProcessControlId ProcessControlID
		{
			get { return ProcessControlId.ServerHealthSummary; }
		}
	}
}
