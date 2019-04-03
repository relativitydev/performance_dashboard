namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class RecoveryObjectivesViewFilterOperands
	{
		public readonly FilterOperand DatabaseName = FilterOperand.Equals;
		public readonly FilterOperand Server = FilterOperand.Equals;
		public FilterOperand RPOScore;
		public FilterOperand RTOScore;
		public FilterOperand PotentialDataLossMinutes;
		public FilterOperand EstimatedTimeToRecoverHours;
	}
}
