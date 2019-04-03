namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class HoursViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public readonly FilterOperand Workspace = FilterOperand.Equals;
		public readonly FilterOperand Search = FilterOperand.Equals;
		public readonly FilterOperand IsComplex = FilterOperand.Equals;
		public readonly FilterOperand IsActiveWeeklySample = FilterOperand.Equals;
		public FilterOperand TotalRunTime;
		public FilterOperand AverageRunTime;
		public FilterOperand TotalRuns;
	}
}
