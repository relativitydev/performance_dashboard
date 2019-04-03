namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Dapper;
	using Properties;

	public class MetricDataRepository : IMetricDataRepository
	{
		public MetricDataRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task<MetricData> CreateAsync(MetricData metricData)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var readResult = await conn.QueryFirstOrDefaultAsync<MetricData>(Resources.MetricData_ReadByMetricAndServer, metricData);
				if (readResult == null)
					await conn.ExecuteAsync(Resources.MetricData_Create, metricData);
				else
					return readResult;
				return await conn.QueryFirstOrDefaultAsync<MetricData>(Resources.MetricData_ReadByMetricAndServer, metricData);
			}
		}

		public async Task<MetricData> ReadAsync(int metricDataId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<MetricData>(Resources.MetricData_ReadByID, new { id = metricDataId });
			}
		}

		public async Task<IList<MetricData>> ReadByCategoryScoreAsync(CategoryScore categoryScore)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<MetricData>(Resources.MetricData_ReadByCategoryScore,
					new {categoryScore.ServerId, categoryScore.CategoryId})).ToList();
			}
		}

		public async Task<IList<MetricData>> ReadByCategoryTypeAndServerIdAsync(CategoryType categoryType, int serverId, IList<int> hourIds)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<MetricData>(Resources.MetricData_ReadByCategoryTypeHour,
					new { serverId, categoryTypeId = (int)categoryType, hourIds })).ToList();
			}
		}

		public async Task<MetricData> ReadByHourAndMetricTypeAsync(Hour hour, Server server, MetricType metricType)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<MetricData>(
					Resources.MetricData_ReadByHourAndMetricTypeAndServer,
					new {hourId = hour.Id, serverId = server?.ServerId, metricType });
			}
		}

		public async Task<MetricData> ReadWorstScoreInDateRangeAsync(DateTime startTime, DateTime endTime, Server server, MetricType metricType)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<MetricData>(
					Resources.MetricData_ReadWorstScoreInDateRange,
					new { startTime, endTime, serverId = server?.ServerId, metricTypeId = (int)metricType });
			}
		}

		public async Task UpdateAsync(MetricData metricData)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MetricData_Update, metricData);
			}
		}

		public async Task DeleteAsync(MetricData metricData)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MetricData_Delete, new { metricData.Id });
			}
		}
	}
}
