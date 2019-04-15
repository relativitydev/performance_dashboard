namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	public class EnvironmentCheckDatabaseFilterOperands
	{
		public FilterOperand ServerName = FilterOperand.Equals;
		public FilterOperand SQLVersion = FilterOperand.Equals;
		public FilterOperand AdHocWorkload = FilterOperand.Equals;
		public FilterOperand MaxServerMemory = FilterOperand.Equals;
		public FilterOperand MaxDegreeOfParallelism = FilterOperand.Equals;
		public FilterOperand TempDBDataFiles = FilterOperand.Equals;
		public FilterOperand LastSQLRestart = FilterOperand.Equals;

	}
}