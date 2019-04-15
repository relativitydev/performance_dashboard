namespace kCura.PDB.Service.DataGrid
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Audits.DataGrid;

	public class DataGridWorkspaceAuditService : IDataGridWorkspaceAuditService
	{
		private readonly IDataGridSettingsService dataGridSettingsService;
		private readonly IArtifactRepository artifactRepository;
		private readonly IDataGridService dataGridService;
		private readonly IHourRepository hourRepository;

		public DataGridWorkspaceAuditService(
			IDataGridSettingsService dataGridSettingsRepository,
			IArtifactRepository artifactRepository,
			IDataGridService dataGridService,
			IHourRepository hourRepository)
		{
			this.dataGridSettingsService = dataGridSettingsRepository;
			this.dataGridService = dataGridService;
			this.artifactRepository = artifactRepository;
			this.hourRepository = hourRepository;
		}

		public async Task<IList<Audit>> ReadAuditsAsync(
			int workspaceId,
			DateTime startHour,
			DateTime endHour,
			IList<AuditActionId> actionTypes,
			int batchSize,
			long pageStart)
		{
			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);

			await Task.WhenAll(auditArtifactTypeId, choiceIds);

			// Pass the settings to the API, along with the batch args
			var queryBatch = new AuditQueryBatch
			{
				Query = new AuditQuery
				{
					WorkspaceId = workspaceId,
					StartTime = startHour,
					EndTime = endHour,
					ActionTypes = actionTypes
				},
				Start = pageStart,
				Size = batchSize
			};

			return await this.dataGridService.ReadAuditsAsync(queryBatch, auditArtifactTypeId.Result, choiceIds.Result);
		}

		public async Task<IList<Audit>> ReadAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes, int batchSize, long pageStart)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadAuditsAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes, batchSize, pageStart);
		}

		public async Task<long> ReadTotalAuditsForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);
			var userGroupById = this.artifactRepository.ReadGroupByArtifactId(workspaceId, DataGridGroupByEnum.User);

			await Task.WhenAll(auditArtifactTypeId, choiceIds, userGroupById);

			var query = new AuditQuery
			{
				WorkspaceId = workspaceId,
				ActionTypes = actionTypes,
				StartTime = startHour,
				EndTime = endHour
			};

			return await this.dataGridService.ReadTotalAuditsForHourAsync(query, auditArtifactTypeId.Result, choiceIds.Result, userGroupById.Result);
		}

		public async Task<long> ReadTotalAuditsForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadTotalAuditsForHourAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes);
		}

		public async Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);
			var userGroupById = this.artifactRepository.ReadGroupByArtifactId(workspaceId, DataGridGroupByEnum.User);

			await Task.WhenAll(auditArtifactTypeId, choiceIds, userGroupById);

			var query = new AuditQuery
			{
				WorkspaceId = workspaceId,
				ActionTypes = actionTypes,
				StartTime = startHour,
				EndTime = endHour
			};

			return (await this.dataGridService.ReadUniqueUsersForHourAuditsAsync(query, auditArtifactTypeId.Result, choiceIds.Result, userGroupById.Result)).Count;
		}

		public async Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadTotalUniqueUsersForHourAuditsAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes);
		}

		public async Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);
			var userGroupById = this.artifactRepository.ReadGroupByArtifactId(workspaceId, DataGridGroupByEnum.User);

			await Task.WhenAll(auditArtifactTypeId, choiceIds, userGroupById);

			var query = new AuditQuery
			{
				WorkspaceId = workspaceId,
				ActionTypes = actionTypes,
				StartTime = startHour,
				EndTime = endHour
			};

			return await this.dataGridService.ReadUniqueUsersForHourAuditsAsync(query, auditArtifactTypeId.Result, choiceIds.Result, userGroupById.Result);
		}

		public async Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadUniqueUsersForHourAuditsAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes);
		}

		public async Task<long> ReadTotalLongRunningQueriesForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);
			var userGroupById = this.artifactRepository.ReadGroupByArtifactId(workspaceId, DataGridGroupByEnum.User);

			await Task.WhenAll(auditArtifactTypeId, choiceIds, userGroupById);

			var query = new AuditQuery
			{
				WorkspaceId = workspaceId,
				ActionTypes = actionTypes,
				StartTime = startHour,
				EndTime = endHour
			};

			return await this.dataGridService.ReadTotalLongRunningQueriesForHourAsync(query, auditArtifactTypeId.Result, choiceIds.Result, userGroupById.Result);
		}

		public async Task<long> ReadTotalLongRunningQueriesForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadTotalLongRunningQueriesForHourAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes);
		}

		public Task<long> ReadTotalAuditExecutionTimeForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			throw new NotSupportedException("Not currently supported by Data Grid API");

			// Read Data grid settings specifically needed for audits
			// -- This workspace's DataGridAudit ArtifactTypeId, action choiceIds
			/*var auditArtifactTypeId = this.artifactRepository.ReadAuditArtifactTypeId(workspaceId);
			var choiceIds = this.dataGridSettingsService.ReadActionChoiceIds(workspaceId, actionTypes);
			var userGroupById = this.artifactRepository.ReadGroupByArtifactId(workspaceId, DataGridGroupByEnum.User);

			await Task.WhenAll(auditArtifactTypeId, choiceIds, userGroupById);

			var query = new AuditQuery
			{
				WorkspaceId = workspaceId,
				ActionTypes = actionTypes,
				StartTime = startHour,
				EndTime = endHour
			};

			return await this.dataGridService.ReadTotalAuditExecutionTimeForHourAsync(query, auditArtifactTypeId.Result, choiceIds.Result, userGroupById.Result);*/
		}

		public Task<long> ReadTotalAuditExecutionTimeForHourAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			throw new NotSupportedException("Not currently supported by Data Grid API");
			/*
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadTotalLongRunningQueriesForHourAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes);
			//*/
		}

		public async Task<bool> ReadAnyAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return await this.ReadTotalAuditsForHourAsync(workspaceId, startHour, endHour, actionTypes) > 0;
		}

		public async Task<bool> ReadAnyAuditsAsync(int workspaceId, int hourId, IList<AuditActionId> actionTypes)
		{
			var startHour = await this.hourRepository.ReadAsync(hourId);
			var endHour = startHour.GetHourEnd();

			return await this.ReadTotalAuditsForHourAsync(workspaceId, startHour.HourTimeStamp, endHour, actionTypes) > 0;
		}
	}
}
