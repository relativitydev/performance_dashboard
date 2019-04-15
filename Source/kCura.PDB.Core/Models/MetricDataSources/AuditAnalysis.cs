namespace kCura.PDB.Core.Models.MetricDataSources
{
	public class AuditAnalysis
	{
		public int Id { get; set; }

		public int MetricDataId { get; set; }

		public int UserId { get; set; }

		public long TotalComplexQueries { get; set; }

		public long TotalLongRunningQueries { get; set; }

		public long TotalSimpleLongRunningQueries { get; set; }

		public long TotalQueries { get; set; }

		public long TotalExecutionTime { get; set; }
	}
}
