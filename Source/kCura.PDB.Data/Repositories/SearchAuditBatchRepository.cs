namespace kCura.PDB.Data.Repositories
{
	using System.Linq;
	using System.Collections.Generic;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Core.Extensions;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models.Audits;

	public class SearchAuditBatchRepository : ISearchAuditBatchRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public SearchAuditBatchRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task CreateBatches(IList<SearchAuditBatch> searchAuditBatches)
		{
			if (!searchAuditBatches.Any()) return;

			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(Resources.SearchAuditBatch_Create, searchAuditBatches);
			}
		}

		public IList<int> CreateBatchResults(IList<SearchAuditBatchResult> searchAuditBatchResults)
		{
			var results = new List<int>();
			if (searchAuditBatchResults.Any() == false) return results;
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				foreach (var searchAuditBatchResult in searchAuditBatchResults)
				{
					var resultsUpdated = connection.Execute(Resources.SearchAuditBatchResult_Update, searchAuditBatchResult);
					if (resultsUpdated == 0)
					{
						connection.Execute(Resources.SearchAuditBatchResult_Create, searchAuditBatchResult);
					}
					var result = connection.QueryFirstOrDefault<int>(Resources.SearchAuditBatchResult_ReadByBatchAndUser, searchAuditBatchResult);
					results.Add(result);
				}
			}

			return results;
		}

		public SearchAuditBatch ReadBatch(int batchId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return connection.QueryFirstOrDefault<SearchAuditBatch>(Resources.SearchAuditBatch_Read, new { batchId });
			}
		}

		public IList<SearchAuditBatchResult> ReadBatchResults(int batchId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return connection.Query<SearchAuditBatchResult>(Resources.SearchAuditBatchResult_ReadByBatchId, new { batchId }).ToList();
			}
		}

		public async Task<IList<SearchAuditBatch>> ReadBatchesByHourAndServer(int hourId, int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryAsync<SearchAuditBatch>(Resources.SearchAuditBatch_ReadByHourAndServer, new { hourId, serverId })).ToList();
			}
		}

		public IList<SearchAuditBatch> ReadByHourAndServer(int hourId, int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var batches = connection.Query<SearchAuditBatch>(Resources.SearchAuditBatch_ReadByHourAndServer, new { hourId, serverId });
				var results = connection.Query<SearchAuditBatchResult>(Resources.SearchAuditBatchResult_ReadByHourAndServer, new { hourId, serverId });
				return batches.ForEach(b => b.BatchResults = results.Where(r => r.BatchId == b.Id).ToList()).ToList();
			}
		}

		public async Task<bool> ExistsForHourAndServer(int hourId, int serverId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryFirstOrDefaultAsync<int>(Resources.SearchAuditBatchResult_ExistByHourAndServer,
					new { hourId, serverId, backFillHours = Defaults.BackfillDays })) > 0;
			}
		}

		public async Task<int> CreateHourSearchAuditBatch(int hourId, int serverId, int batchesCreated)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var updateResult = await connection.ExecuteAsync(Resources.HourSearchAuditBatch_UpdateByHourAndServer, new { hourId, serverId, batchesCreated });
				if (updateResult == 0)
				{
					return await connection.QueryFirstOrDefaultAsync<int>(
						Resources.HourSearchAuditBatches_Create,
						new { hourId, serverId, batchesCreated });
				}
				return await connection.QueryFirstOrDefaultAsync<int>(
					Resources.HourSearchAuditBatches_ReadByHourAndServer,
					new { hourId, serverId, });
			}
		}

		public async Task UpdateAsync(SearchAuditBatch batch)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(Resources.SearchAuditBatch_Update, batch);
			}
		}

	    public async Task DeleteAllBatchesAsync(int hourSearchAuditBatchId)
	    {
	        using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
	        {
	            await connection.ExecuteAsync(
	                Resources.SearchAuditBatchResult_DeleteByHourBatchId,
	                new { hourSearchAuditBatchId });

                await connection.ExecuteAsync(
                          Resources.SearchAuditBatch_DeleteByHourBatchId,
                          new { hourSearchAuditBatchId });
	        }
        }
	}
}
