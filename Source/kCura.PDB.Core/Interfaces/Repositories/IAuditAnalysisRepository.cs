namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	public interface IAuditAnalysisRepository
	{
		Task CreateAsync(IList<AuditAnalysis> auditAnalyses);

		Task<IList<AuditAnalysis>> ReadByMetricData(MetricData metricData);
	}
}
