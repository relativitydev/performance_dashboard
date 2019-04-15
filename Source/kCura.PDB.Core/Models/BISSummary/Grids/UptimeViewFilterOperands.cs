namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class UptimeViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public readonly FilterOperand Status = FilterOperand.Equals;
		public FilterOperand Score;
		public FilterOperand Uptime;
	}
}
