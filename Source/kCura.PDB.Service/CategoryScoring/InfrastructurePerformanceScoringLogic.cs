namespace kCura.PDB.Service.CategoryScoring
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Models;

	[CategoryType(CategoryType.InfrastructurePerformance)]
	public class InfrastructurePerformanceScoringLogic : ICategoryScoringLogic
	{
		public Task<decimal> ScoreMetrics(CategoryScore categoryScore, IList<MetricData> metricDatas)
		{
			return Task.FromResult(Defaults.Scores.OneHundred);
		}
	}
}
