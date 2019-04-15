namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class LoadViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public readonly FilterOperand Server = FilterOperand.Equals;
		public readonly FilterOperand ServerType = FilterOperand.Equals;
		public readonly FilterOperand IsActiveWeeklySample = FilterOperand.Equals;
		public FilterOperand OverallScore;
		public FilterOperand CPUUtilizationScore;
		public FilterOperand RAMUtilizationScore;
        public FilterOperand MemorySignalScore;
        public FilterOperand WaitsScore;
        public FilterOperand VirtualLogFilesScore;
        public FilterOperand LatencyScore;
	}
}
