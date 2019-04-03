namespace kCura.PDB.Service.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Models.Audits;

	public class AuditBatchAnalyzer : IAuditBatchAnalyzer
	{
		/// <inheritdoc />
		public IList<SearchAuditBatchResult> GetBatchResults(IEnumerable<SearchAuditGroup> parsedAuditGroups, int batchId)
		{
			return parsedAuditGroups.GroupBy(auditGroup => auditGroup.UserId)
				.Select(userAuditGrouping => new SearchAuditBatchResult
				{
					TotalComplexQueries = userAuditGrouping.Count(audit => audit.IsComplex),
					TotalLongRunningQueries = userAuditGrouping.Count(SearchAnalysisService.IsLongRunning),
					TotalSimpleLongRunningQueries = userAuditGrouping.Count(auditGroup => !auditGroup.IsComplex && SearchAnalysisService.IsLongRunning(auditGroup)),
					TotalQueries = userAuditGrouping.Count(),
					UserId = userAuditGrouping.Key,
					BatchId = batchId,
					TotalExecutionTime = userAuditGrouping.Sum(auditGroup => auditGroup.ExecutionTime)
				}).ToList();
		}
	}
}
