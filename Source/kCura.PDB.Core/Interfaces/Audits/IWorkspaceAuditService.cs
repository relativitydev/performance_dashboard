namespace kCura.PDB.Core.Interfaces.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// Interface for interacting with audit repositories
	/// </summary>
	public interface IWorkspaceAuditService
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
		/// Method to read audit data for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <param name="batchSize">The maximum number of audits to obtain per batch</param>
		/// <param name="pageStart">The initial starting point (ID?) to read the audits from</param>
		/// <returns>List of audits for the given workspace/timeRange/actionTypes</returns>
		Task<IList<Audit>> ReadAuditsAsync(
			int workspaceId,
			int hourId,
			IList<AuditActionId> actionTypes,
			int batchSize,
			long pageStart);

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
		/// Method to read total number of audits for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of audits for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalAuditsForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read total number of unique users for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="startHour">The start time to obtain audits for</param>
		/// <param name="endHour">The end time to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of unique users for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes);

		/// <summary>
		/// Method to read total number of unique users for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of unique users for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes);

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

		/// <summary>
		/// Method to read audit unique users for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Unique UserIds that have audits for the given workspace/timeRange/actionTypes</returns>
		Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(
			int workspaceId,
			int hourId,
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
		/// Method to read total long running queries for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Number of audits that classify as long-running for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalLongRunningQueriesForHourAsync(
			int workspaceId,
			int hourId,
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

		/// <summary>
		/// Method to read total audit execution time for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>Execution Time (in MS) for all audits for the given workspace/timeRange/actionTypes</returns>
		Task<long> ReadTotalAuditExecutionTimeForHourAsync(
			int workspaceId,
			int hourId,
			IList<AuditActionId> actionTypes);
	}
}
