namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricManagerStatsRepository
	{
		Task CreateAsync(IList<MetricManagerExecutionStat> stat);
	}
}
