namespace kCura.PDB.Core.Models.Audits
{
	using System;
	using System.Collections.Generic;

	// Contains information needed to score the audits for the user experience
	public class SearchAuditBatch
	{
		/// <summary>
		/// Gets or sets the batch id
		/// </summary>
		public int Id { get; set; }

		public int HourId { get; set; }

		public int WorkspaceId { get; set; }

		public int ServerId { get; set; }

		public long BatchStart { get; set; }

		public int BatchSize { get; set; }

		public bool Completed { get; set; }

		public int HourSearchAuditBatchId { get; set; }

		/// <summary>
		/// Gets or sets collection of SearchAuditBatchResults
		/// </summary>
		public IList<SearchAuditBatchResult> BatchResults { get; set; }
	}
}