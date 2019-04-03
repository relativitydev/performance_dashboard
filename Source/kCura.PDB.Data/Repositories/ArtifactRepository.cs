namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits.DataGrid;
	using kCura.PDB.Data.Properties;

	public class ArtifactRepository : IArtifactRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ArtifactRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task<int> ReadAuditArtifactTypeId(int workspaceId)
		{
			var auditArtifactTypeId = await this.ReadByGuid(workspaceId, DataGridResourceConstants.AuditArtifactTypeGuid); // ArtifactTypeID for this object is (25 - ObjectType), Grab the ArtifactID instead
			if (!auditArtifactTypeId.HasValue)
				throw new Exception("Cannot query Data Grid Audit artifact type id.");

			return auditArtifactTypeId.Value;
		}

		public async Task<int> ReadGroupByArtifactId(int workspaceId, DataGridGroupByEnum groupByType)
		{
			Guid guidToUse;
			switch (groupByType)
			{
				case DataGridGroupByEnum.Action:
					guidToUse = DataGridResourceConstants.ActionFieldGuid;
					break;
				case DataGridGroupByEnum.Date:
					guidToUse = DataGridResourceConstants.TimeStampFieldGuid;
					break;
				case DataGridGroupByEnum.ObjectType:
					guidToUse = DataGridResourceConstants.ObjectFieldGuid;
					break;
				case DataGridGroupByEnum.User:
					guidToUse = DataGridResourceConstants.UserNameFieldGuid;
					break;
				default:
					throw new Exception("Not a valid DataGridGroupByEnum");
			}

			var groupByArtifactId = await this.ReadByGuid(workspaceId, guidToUse);
			if (!groupByArtifactId.HasValue)
				throw new Exception("Cannot query Data Grid Audit user group by id.");

			return groupByArtifactId.Value;
		}

		internal async Task<int?> ReadByGuid(int workspaceId, Guid guid)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.QueryFirstOrDefaultAsync<int>(Resources.Artifact_ReadByGuid, new { artifactGuid = guid });
			}
		}

		public async Task<int?> ReadFieldChoiceIdByGuid(int workspaceId, Guid guid)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.QueryFirstOrDefaultAsync<int?>(Resources.Field_ReadChoiceTypeByGuid, new { artifactGuid = guid });
			}
		}

		public async Task<IList<Choice>> ReadChoiceByChoiceType(int workspaceId, int choiceTypeId)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return (await connection.QueryAsync<Choice>(Resources.Choice_ReadByChoiceTypeId, new { choiceTypeId })).ToList();
			}
		}

		public async Task<bool> TestDatabaseAccessAsync(int workspaceId)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return (await connection.QueryFirstOrDefaultAsync<int>(Resources.Artifact_TestDatabaseAccess)) > 0;
			}
		}

		public async Task<bool> IsWorkspaceUpgradeComplete(int workspaceId)
		{
			using (var connection = this.connectionFactory.GetEddsConnection())
			{
				var status = await connection.QueryFirstOrDefaultAsync<int?>(Resources.Workspace_ReadStatus, new { workspaceId });
				return status.HasValue && status.Value == (int)WorkspaceUpgradeStatus.Completed;
			}
		}
	}
}
