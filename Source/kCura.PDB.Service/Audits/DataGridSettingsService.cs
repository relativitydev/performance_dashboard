namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class DataGridSettingsService : IDataGridSettingsService
	{
		private readonly IArtifactRepository artifactRepository;
		private readonly ILogger logger;

		public DataGridSettingsService(IArtifactRepository artifactRepository, ILogger logger)
		{
			this.artifactRepository = artifactRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.DataGrid);
		}

		public async Task<IList<int>> ReadActionChoiceIds(int workspaceId, IList<AuditActionId> actions)
		{
			// Query the field for the choice type id
			var fieldChoiceId = await this.artifactRepository.ReadFieldChoiceIdByGuid(workspaceId, DataGridResourceConstants.ActionFieldGuid);
			if (!fieldChoiceId.HasValue)
			{
				throw new Exception(
					$"Field not found in workspace {workspaceId} for action field guild {DataGridResourceConstants.ActionFieldGuid}");
			}

			await this.logger.LogVerboseAsync(
				$"Workspace {workspaceId} - choiceTypeId {fieldChoiceId} for action field guid {DataGridResourceConstants.ActionFieldGuid}");

			// Query for all of the choices
			var choices = await this.artifactRepository.ReadChoiceByChoiceType(workspaceId, fieldChoiceId.Value);

			await this.logger.LogVerboseAsync(
				$"Workspace {workspaceId} Retrieved {choices.Count} choiceIds - {choices.Select(c => $"{c.Name}-{c.ArtifactId}").Join(", ")}");

			// Now we need to map the correct actions to the artifact Ids to make sure we only return those ones.
			var responseActions =
				choices.Where(c => actions.Any(a => a.GetDisplayName() == c.Name)).ToList();

			if (responseActions.Count != actions.Count)
			{
				var notFoundActions = actions.Where(a => responseActions.All(r => r.Name != a.GetDisplayName())).ToList();
				throw new Exception(
					$"WorkspaceId {workspaceId} - Mismatch of action/reponse count: Actions - {actions.Count}, FilteredResponse Actions - {responseActions.Count}, ResponseTotal - {choices.Count}, MissingActions - {string.Join(" | ", notFoundActions.Select(r => $"{r.GetDisplayName()}"))}, {{ {string.Join(" , ", choices.Select(r => $"{r.Name}, {r.ArtifactId}"))} }}");
			}

			return responseActions.Select(r => r.ArtifactId).ToList();
		}
	}
}
