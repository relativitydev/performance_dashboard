namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Repository that returns audit data
	/// </summary>
	public interface IAuditRepository
	{
		/// <summary>
		/// Method to read audit data for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <param name="batchSize">The maximum number of audits to obtain per batch</param>
		/// <param name="pageStart">The initial starting point (ID?) to read the audits from</param>
		/// <returns>List of audits for the given workspace/timeRange/actionTypes</returns>
		Task<IList<Audit>> ReadAuditsAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes,
			int batchSize,
			long pageStart);

		/// <summary>
		/// Method to read if there are ANY number of audits for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>True or False for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<bool> ReadAnyAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read total number of audits for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of audits for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalAuditsForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read audit unique users for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Unique UserIds that have audits for the given workspace/timeRange/actionTypes</returns>
		Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes);

		Task<int> ReadTotalUniqueUsersForHourAuditsAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read total long running queries for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of audits that classify as long-running for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalLongRunningQueriesForHourAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read total audit execution time for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Execution Time (in MS) for all audits for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalAuditExecutionTimeForHourAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes);
	}
}