namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.Audits;

	public interface IAuditBatchAnalyzer
	{
		/// <summary>
		/// Analyzes a given set of SearchAudits and returns parsed usable aggregate data (SearchAuditBatchResults).
		/// </summary>
		/// <param name="parsedAuditGroups">Pepared SearchAudits to be analyzed</param>
		/// <param name="batchId">The batch id that the given parsedAudits belong to (Sets in return object)</param>
		/// <returns>List of usable aggregate data based on given parsed SearchAudits</returns>
		IList<SearchAuditBatchResult> GetBatchResults(IEnumerable<SearchAuditGroup> parsedAuditGroups, int batchId);
	}
}
