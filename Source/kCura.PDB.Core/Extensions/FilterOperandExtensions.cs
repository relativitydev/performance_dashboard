namespace kCura.PDB.Core.Extensions
{
	using kCura.PDB.Core.Models;

	public static class FilterOperandExtensions
	{
		public static string GetSqlOperation(this FilterOperand operand)
		{
			switch (operand)
			{
				case FilterOperand.GreaterThan:
					return ">";
				case FilterOperand.GreaterThanEqual:
					return ">=";
				case FilterOperand.LessThan:
					return "<";
				case FilterOperand.LessThanEqual:
					return "<=";
				case FilterOperand.Equals:
					return "=";
				default:
					return "=";
			}
		}
	}
}
