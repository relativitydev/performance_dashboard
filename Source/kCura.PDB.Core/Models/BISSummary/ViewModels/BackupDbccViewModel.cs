namespace kCura.PDB.Core.Models.BISSummary.ViewModels
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class BackupDbccViewModel
	{
		public BackupDbccViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new BackupDbccViewFilterConditions();
			FilterOperands = new BackupDbccViewFilterOperands();
		}

		public GridConditions GridConditions;
		public BackupDbccViewFilterConditions FilterConditions;
		public BackupDbccViewFilterOperands FilterOperands;
	}
}