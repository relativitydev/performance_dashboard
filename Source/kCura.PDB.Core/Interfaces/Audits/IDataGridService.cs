namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Interact with the API endpoint given settings and query arguments and return needed data
	/// </summary>
	public interface IDataGridService
	{
		Task<IList<Audit>> ReadAuditsAsync(AuditQueryBatch queryBatch, int auditArtifactTypeId, IList<int> actionChoiceIds);

		Task<long> ReadTotalAuditsForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById);

		Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById);

		Task<long> ReadTotalLongRunningQueriesForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById);

		Task<long> ReadTotalAuditExecutionTimeForHourAsync(AuditQuery query, int auditArtifactTypeId, IList<int> actionChoiceIds, int userGroupById);
	}
}
