namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class SampleHistoryRepository : BaseDbRepository, ISampleHistoryRepository
	{
		public SampleHistoryRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task AddToCurrentSampleAsync(SampleHistory sampleHistory)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var recordsUpdated = await connection.ExecuteAsync(Resources.SampleHistory_Update, sampleHistory);
				if (recordsUpdated == 0)
				{
					await connection.ExecuteAsync(Resources.SampleHistory_AddCurrentSample, sampleHistory);
				}
			}
		}

		public async Task AddToCurrentSampleAsync(IList<SampleHistory> sampleHistories)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var result = await connection.ExecuteAsync(Resources.SampleHistory_AddCurrentSample, sampleHistories);
			}
		}

		public async Task ResetCurrentSampleAsync(int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var result = await connection.ExecuteAsync(Resources.SampleHistory_ResetCurrentSample, new { serverId });
			}
		}

		public async Task<IList<SampleHistory>> ReadCurrentArrivalRateSampleAsync(int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryAsync<SampleHistory>(Resources.SampleHistory_ReadCurrentArrivalRateSample, new { serverId })).ToList();
			}
		}

		public async Task<IList<SampleHistory>> ReadCurrentConcurrencySampleAsync(int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryAsync<SampleHistory>(Resources.SampleHistory_ReadCurrentConcurrencySample, new { serverId })).ToList();
			}
		}

		public async Task RemoveHourFromSampleAsync(int hourId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(Resources.SampleHistory_RemoveHourFromSample, new { hourId });
			}
		}

		public SampleHistoryRange ReadSampleRange()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return connection.QueryFirstOrDefault<SampleHistoryRange>(Resources.SampleHistory_ReadSampleRange);
			}
		}
	}
}
