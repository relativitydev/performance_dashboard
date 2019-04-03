namespace kCura.PDB.Core.Interfaces.Audits
{
	using System;

	public interface IDataGridConditionBuilder
	{
		/// <summary>
		/// Condition is just the action choiceIds and timeframe
		/// </summary>
		/// <param name="actionIds">Choice ids of the actions to filter by</param>
		/// <param name="start">Start time</param>
		/// <param name="end">End time</param>
		/// <returns>String condition for the Data Grid ObjectManager/Pivot query</returns>
		string BuildActionTimeframeCondition(int[] actionIds, DateTime start, DateTime end);

		/// <summary>
		/// Condition is the action choiceIds, timeframe, and Execution time threshold
		/// </summary>
		/// <param name="actionIds">Choice ids of the actions to filter by</param>
		/// <param name="start">Start time</param>
		/// <param name="end">End time</param>
		/// <returns>String condition for the Data Grid ObjectManager/Pivot query</returns>
		string BuildActionTimeframeLongRunningCondition(int[] actionIds, DateTime start, DateTime end);
	}
}
