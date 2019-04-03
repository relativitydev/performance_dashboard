namespace kCura.PDB.Core.Interfaces.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricDataService
	{
		T GetData<T>(MetricData metricData);

		void SetData<T>(MetricData metricData, T data);

		Task<MetricData> GetMetricData(int metricDataId);

		Task<IList<MetricData>> ReadMetricDatasByCategoryScoreAsync(CategoryScore categoryScore);

		/// <summary>
		/// Gets Metric data for the given hourIds for the given serverId/categoryType
		/// </summary>
		/// <param name="hourIds">Hours to retrieve metric data for</param>
		/// <param name="serverId">Server to retrieve metric data for</param>
		/// <param name="categoryType">CategoryType to retrieve metric data for</param>
		/// <returns>Metric data</returns>
		Task<IList<MetricData>> GetMetricData(IList<int> hourIds, int serverId, CategoryType categoryType);
	}
}
