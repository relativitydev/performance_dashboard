namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IAuditServerBatcher
	{
		/// <summary>
		/// Creates (search) audit batch collection for the given serverId/hourId
		/// </summary>
		/// <param name="metricDataId">The metric data id to create (search) audit batches for.</param>
		/// <returns>List of SearchAuditBatch ids</returns>
		Task<IList<int>> CreateServerBatches(int metricDataId);
	}
}
