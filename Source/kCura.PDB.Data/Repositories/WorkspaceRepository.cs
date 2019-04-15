namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::Relativity.API;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	public class WorkspaceRepository : IWorkspaceRepository
	{
		public WorkspaceRepository(IHelper helper)
		{
			_helper = helper;
		}

		private readonly IHelper _helper = null;
		public List<Workspace> ReadAll(List<String> fieldNames = null)
		{
			var fields = null != fieldNames
				? fieldNames.Select(f => new FieldValue(f)).ToList()
				: FieldValue.AllFields;

			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				var result = client.Repositories.Workspace.Query(new Query<Workspace>(fields, null, new List<Sort>()));

				if (result.Success)
				{
					return result.Results.Select(r => r.Artifact).ToList();
				}
				else if (1 <= result.Results.Count)
				{
					throw new Exception(result.Results.First().Message);
				}
				else
				{
					throw new Exception(result.Message);
				}
			}
		}
	}
}
