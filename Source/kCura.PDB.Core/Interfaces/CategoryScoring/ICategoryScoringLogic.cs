namespace kCura.PDB.Core.Interfaces.CategoryScoring
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface ICategoryScoringLogic
	{
		Task<decimal> ScoreMetrics(CategoryScore categoryScore, IList<MetricData> metricDatas);
	}
}
