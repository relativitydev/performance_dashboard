namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;

	public interface IServerAuditService
	{
		/// <summary>
		/// Returns the total number of audits in a given workspace for a given hour
		/// </summary>
		/// <param name="serverId">The ArtifactId for the server</param>
		/// <param name="hourId">The id of the hour to check audits for</param>
		/// <param name="actionTypes">The audit action types to check for</param>
		/// <returns>Number of total audits for the given server/hour</returns>
		Task<long> ReadTotalAuditsForHourAsync(int serverId, int hourId, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Returns the total number of unique users who created audits for the given hour
		/// </summary>
		/// <param name="serverId">The ArtifactId for the server</param>
		/// <param name="hourId">The id for the hour to check audits for</param>
		/// <param name="actionTypes">The audit action types to check for</param>
		/// <returns>Number of unique users who created audits for the given server/hour</returns>
		Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int serverId, int hourId, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Do not use until Data Grid API has implementation
		/// </summary>
		/// <param name="serverId">Do not use serverId</param>
		/// <param name="hourId">Do not use hourId</param>
		/// <param name="actionTypes">Do not use actionTypes</param>
		/// <returns>Do not use</returns>
		Task<long> ReadTotalAuditExecutionTimeForHourAsync(int serverId, int hourId, IList<AuditActionId> actionTypes);
	}
}