namespace kCura.PDB.Core.Models.BISSummary.ViewModels
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class RecoveryObjectivesViewModel
	{
        public RecoveryObjectivesViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new RecoveryObjectivesViewFilterConditions();
			FilterOperands = new RecoveryObjectivesViewFilterOperands();
		}

		public GridConditions GridConditions;
        public RecoveryObjectivesViewFilterConditions FilterConditions;
        public RecoveryObjectivesViewFilterOperands FilterOperands;
	}
}