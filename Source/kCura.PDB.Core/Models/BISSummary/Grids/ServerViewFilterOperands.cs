namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class ServerViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public readonly FilterOperand Server = FilterOperand.Equals;
		public readonly FilterOperand Workspace = FilterOperand.Equals;
		public readonly FilterOperand IsActiveWeeklySample = FilterOperand.Equals;
		public FilterOperand TotalUsers;
		public FilterOperand TotalLongRunning;
		public FilterOperand TotalSearchAudits;
		public FilterOperand TotalNonSearchAudits;
		public FilterOperand TotalExecutionTime;
		public FilterOperand TotalAudits;
		public FilterOperand Score;
	}
}
