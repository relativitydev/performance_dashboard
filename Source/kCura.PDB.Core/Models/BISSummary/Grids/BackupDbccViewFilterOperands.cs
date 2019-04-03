namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class BackupDbccViewFilterOperands
	{
		public readonly FilterOperand Server = FilterOperand.Equals;
		public readonly FilterOperand Database = FilterOperand.Equals;
		public readonly FilterOperand LastActivityDate = FilterOperand.Equals;
		public readonly FilterOperand IsBackup = FilterOperand.Equals;
		public readonly FilterOperand GapResolutionDate = FilterOperand.Equals;
		public FilterOperand GapSize;
	}
}
