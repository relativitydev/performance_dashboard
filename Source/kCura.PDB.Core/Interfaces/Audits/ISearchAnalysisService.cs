namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Analyses a given search audit
	/// </summary>
	public interface ISearchAnalysisService
	{
		/// <summary>
		/// Returns the complexity status of a given SearchAudit
		/// </summary>
		/// <param name="audit">Search audit to analyze</param>
		/// <returns>Returns an a search audit with botht the audit and search</returns>
		Task<SearchAuditGroup> AnalyzeSearchAudit(SearchAuditGroup audit);
	}
}
