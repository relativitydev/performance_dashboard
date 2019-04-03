namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Linq;
	using System.Text;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Audits;

	public class DataGridConditionBuilder : IDataGridConditionBuilder
	{
		public string BuildActionTimeframeCondition(int[] actionIds, DateTime start, DateTime end)
		{
			return this.BuildActionTimeframeConditionStringBuilder(actionIds, start, end).ToString();
		}

		public string BuildActionTimeframeLongRunningCondition(int[] actionIds, DateTime start, DateTime end)
		{
			var sb = this.BuildActionTimeframeConditionStringBuilder(actionIds, start, end);

			// Add execution time condition
			sb.Append($" AND ((\'{DataGridResourceConstants.DataGridFieldExecutionTime}\' > {AuditConstants.LongRunningSimpleThreshold}))");

			return sb.ToString();
		}

		private StringBuilder BuildActionTimeframeConditionStringBuilder(int[] actionIds, DateTime start, DateTime end)
		{
			if (!actionIds.Any())
			{
				throw new Exception("Need at least one action ID");
			}

			// Start by appending each action choice id
			var sb = new StringBuilder();
			sb.Append($"(('{DataGridResourceConstants.DataGridFieldAction}' IN CHOICE [");
			foreach (var id in actionIds)
			{
				sb.Append($"{id}, ");
			}

			sb.Remove(sb.Length - 2, 2); // Remove the last comma and space

			// Add the time range condition
			sb.Append($"])) AND ((\'{DataGridResourceConstants.DataGridFieldTimeStamp}\' >= {start:yyyy-MM-ddTHH:mm:ss.ffZ})) AND ((\'{DataGridResourceConstants.DataGridFieldTimeStamp}\' <= {end:yyyy-MM-ddTHH:mm:ss.ffZ}))");
			return sb;
		}
	}
}
