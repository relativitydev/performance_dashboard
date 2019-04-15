namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// Process data in order to score audits
	/// </summary>
	public interface IAuditBatchProcessor
	{
		/// <summary>
		/// Processes a given batch section into data to be used for User Experience Scoring
		/// </summary>
		/// <param name="batchId">The id for the batch to process</param>
		/// <returns>SearchAuditBatch information processed for the given batchId</returns>
		Task<IList<int>> ProcessBatch(int batchId);
	}
}
