namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class RecoverabilityIntegrityViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public FilterOperand RecoverabilityIntegrityScore;
        public FilterOperand BackupFrequencyScore;
        public FilterOperand BackupCoverageScore;
        public FilterOperand DbccFrequencyScore;
        public FilterOperand DbccCoverageScore;
        public FilterOperand RPOScore;
        public FilterOperand RTOScore;
	}
}
