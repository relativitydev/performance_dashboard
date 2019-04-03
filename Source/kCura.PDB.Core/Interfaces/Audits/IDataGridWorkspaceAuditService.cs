namespace kCura.PDB.Core.Interfaces.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;

	public interface IDataGridWorkspaceAuditService : IWorkspaceAuditService
	{
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
		/// Method to read if there are ANY number of audits for the given workspace/timeRange/actionTypes
		/// </summary>
		/// <param name="workspaceId">The ArtifactId of the workspace that contains the audits</param>
		/// <param name="hourId">The start hourId to obtain audits for</param>
		/// <param name="actionTypes">The audit action types which to obtain</param>
		/// <returns>True or False for the hour for the given workspace/timeRange/actionTypes</returns>
		Task<bool> ReadAnyAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes);
	}
}
