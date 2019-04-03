namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using kCura.PDB.Core.Models;

	public class WaitsViewFilterOperands
	{
		public readonly FilterOperand SummaryDayHour = FilterOperand.Equals;
		public readonly FilterOperand Server = FilterOperand.Equals;
		public readonly FilterOperand WaitType = FilterOperand.Equals;
		public readonly FilterOperand IsPoisonWait = FilterOperand.Equals;
		public readonly FilterOperand IsActiveWeeklySample = FilterOperand.Equals;
		public FilterOperand OverallScore;
		public FilterOperand SignalWaitsRatio;
		public FilterOperand SignalWaitTime;
		public FilterOperand TotalWaitTime;
		public FilterOperand PercentOfCPUThreshold = FilterOperand.Equals;
		public FilterOperand DifferentialWaitingTasksCount = FilterOperand.Equals;
	}
}
