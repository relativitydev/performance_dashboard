namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Models;

	public interface IMetricDataRepository
	{
		Task<MetricData> CreateAsync(MetricData metricData);

		Task<MetricData> ReadAsync(int metricDataId);

		Task<IList<MetricData>> ReadByCategoryScoreAsync(CategoryScore categoryScore);

		Task<IList<MetricData>> ReadByCategoryTypeAndServerIdAsync(CategoryType categoryType, int serverId, IList<int> hourIds);

		Task<MetricData> ReadByHourAndMetricTypeAsync(Hour hour, Server server, MetricType metricType);

		Task<MetricData> ReadWorstScoreInDateRangeAsync(DateTime startTime, DateTime endTime, Server server, MetricType metricType);

		Task UpdateAsync(MetricData metricData);

		Task DeleteAsync(MetricData metricData);
	}
}
