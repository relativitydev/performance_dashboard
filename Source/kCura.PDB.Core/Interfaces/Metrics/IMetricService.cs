namespace kCura.PDB.Core.Interfaces.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IMetricService
	{
		Task<IList<int>> CreateMetricDatasForCategoryScores(int categoryScoreId);
	}
}
