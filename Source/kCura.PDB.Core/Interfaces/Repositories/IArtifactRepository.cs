namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits.DataGrid;

	public interface IArtifactRepository
	{
		Task<int> ReadAuditArtifactTypeId(int workspaceId);

		Task<int> ReadGroupByArtifactId(int workspaceId, DataGridGroupByEnum groupByType);

		Task<IList<Choice>> ReadChoiceByChoiceType(int workspaceId, int choiceTypeId);

		Task<int?> ReadFieldChoiceIdByGuid(int workspaceId, Guid guid);

		Task<bool> TestDatabaseAccessAsync(int workspaceId);

		Task<bool> IsWorkspaceUpgradeComplete(int workspaceId);

	}
}
