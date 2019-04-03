namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.Audits;

	public interface ISearchAuditBatchRepository
	{
		Task CreateBatches(IList<SearchAuditBatch> searchAuditBatches);

		IList<int> CreateBatchResults(IList<SearchAuditBatchResult> searchAuditBatchResults);

		SearchAuditBatch ReadBatch(int batchId);

		IList<SearchAuditBatchResult> ReadBatchResults(int batchId);

		Task<IList<SearchAuditBatch>> ReadBatchesByHourAndServer(int hourId, int serverId);

		IList<SearchAuditBatch> ReadByHourAndServer(int hourId, int serverId);

		Task<bool> ExistsForHourAndServer(int hourId, int serverId);

		Task<int> CreateHourSearchAuditBatch(int hourId, int serverId, int batchesCreated);

		Task UpdateAsync(SearchAuditBatch batch);

	    Task DeleteAllBatchesAsync(int hourSearchAuditBatchId);
	}
}
