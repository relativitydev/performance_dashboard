namespace kCura.PDB.Core.Models.Audits
{
	public class SearchAuditBatchResult
	{
		public int Id { get; set; }

		public int BatchId { get; set; }

		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets Total number of complex queries.
		/// </summary>
		public long TotalComplexQueries { get; set; }

		/// <summary>
		/// Gets or sets Total number of long-running queries
		/// </summary>
		public long TotalLongRunningQueries { get; set; }

		public long TotalSimpleLongRunningQueries { get; set; }

		/// <summary>
		/// Gets or sets Total number of queries.  Used for ArrivalRate -- also used to determine if batch is finished
		/// </summary>
		public long TotalQueries { get; set; }

		public long TotalExecutionTime { get; set; }
	}
}