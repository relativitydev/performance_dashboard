namespace kCura.PDB.Service.DataGrid.Helpers
{
	using System.Collections.Generic;
	using global::Relativity.Services.Objects.DataContracts;

	public class KeplerSerializationHelperService
	{
		public QueryRequest ForceLoadQueryObject()
		{
			return new QueryRequest
			{
				Condition = $"(('Audit ID' > {1}))",
				RowCondition = string.Empty,
				Sorts = null,
				Fields = new List<FieldRef>()
			};
		}

		public QueryResult ForceLoadQueryResultObject()
		{
			return new QueryResult();
		}
	}
}
