namespace kCura.PDB.Core.Models
{
	using System;
	using System.Linq;

	public class MetricManagerExecutionStat
	{
		public Guid ExecutionId { get; set; }

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public string Name { get; set; }

		public double TotalTime { get; set; }

		public double MaxTime { get; set; }

		public int Count { get; set; }
	}
}
