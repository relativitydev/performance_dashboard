namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class EnvironmentCheckServerFilterOperands
	{
		public FilterOperand ServerName = FilterOperand.Equals;
		public FilterOperand OSName = FilterOperand.Equals;
		public FilterOperand OSversion = FilterOperand.Equals;
		public FilterOperand LogicalProcessors = FilterOperand.Equals;
		public FilterOperand Hyperthreaded = FilterOperand.Equals;
	}
}