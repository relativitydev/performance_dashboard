namespace kCura.PDB.Core.Models.BISSummary.ViewModels
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class RecoverabilityIntegrityViewModel
	{
        public RecoverabilityIntegrityViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new RecoverabilityIntegrityViewFilterConditions();
			FilterOperands = new RecoverabilityIntegrityViewFilterOperands();
		}

		public GridConditions GridConditions;
        public RecoverabilityIntegrityViewFilterConditions FilterConditions;
        public RecoverabilityIntegrityViewFilterOperands FilterOperands;
	}
}