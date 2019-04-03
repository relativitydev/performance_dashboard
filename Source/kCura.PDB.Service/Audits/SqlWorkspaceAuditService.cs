namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.Audits;

	/// <summary>
	/// This may seem like a passthrough class because it is, but is neccesarry because of the
	/// DataGrid Audit implementation needing specific data before it can access the repository
	/// </summary>
	public class SqlWorkspaceAuditService : ISqlWorkspaceAuditService
	{
		private readonly ISqlAuditRepository auditRepository;
		private readonly IHourRepository hourRepository;

		public SqlWorkspaceAuditService(ISqlAuditRepository sqlAuditRepository, IHourRepository hourRepository)
		{
			this.auditRepository = sqlAuditRepository;
			this.hourRepository = hourRepository;
		}

		public Task<IList<Audit>> ReadAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes, int batchSize, long pageStart)
		{
			return this.auditRepository.ReadAuditsAsync(workspaceId, startHour, endHour, actionTypes, batchSize, pageStart);
		}

		public async Task<IList<Audit>> ReadAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes, int batchSize, long pageStart)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadAuditsAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes,
				batchSize,
				pageStart);
		}

		public Task<long> ReadTotalAuditExecutionTimeForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return this.auditRepository.ReadTotalAuditExecutionTimeForHourAsync(workspaceId, startHour, endHour, actionTypes);
		}

		public async Task<long> ReadTotalAuditExecutionTimeForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadTotalAuditExecutionTimeForHourAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes);
		}

		public Task<long> ReadTotalAuditsForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return this.auditRepository.ReadTotalAuditsForHourAsync(workspaceId, startHour, endHour, actionTypes);
		}

		public async Task<long> ReadTotalAuditsForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadTotalAuditsForHourAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes);
		}

		public Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return this.auditRepository.ReadTotalUniqueUsersForHourAuditsAsync(workspaceId, startHour, endHour, actionTypes);
		}

		public async Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadTotalUniqueUsersForHourAuditsAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes);
		}

		public Task<long> ReadTotalLongRunningQueriesForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return this.auditRepository.ReadTotalLongRunningQueriesForHourAsync(workspaceId, startHour, endHour, actionTypes);
		}

		public async Task<long> ReadTotalLongRunningQueriesForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadTotalLongRunningQueriesForHourAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes);
		}

		public Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return this.auditRepository.ReadUniqueUsersForHourAuditsAsync(workspaceId, startHour, endHour, actionTypes);
		}

		public async Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();
			return await this.auditRepository.ReadUniqueUsersForHourAuditsAsync(
				workspaceId,
				startHour.HourTimeStamp,
				endHour,
				actionTypes);
		}
	}
}
