namespace kCura.PDB.Data.Repositories
{
	using System;
	using global::Relativity.API;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.Relativity.Client;

	public class ErrorRepository : IErrorRepository
	{
		public ErrorRepository(IHelper helper)
		{
			_helper = helper;
		}
		private readonly IHelper _helper;
		public Int32 LogError(Exception ex, string url, int workspaceId, string source)
		{
			System.Diagnostics.Trace.TraceError(ex.ToString());

			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				var errorToCreate = new kCura.Relativity.Client.DTOs.Error();
				errorToCreate.FullError = ex.ToString();
				errorToCreate.Message = ex.Message.Truncate(200);
				errorToCreate.SendNotification = false;
				errorToCreate.Server = System.Environment.MachineName;
				errorToCreate.Source = source;
				errorToCreate.URL = url;
				errorToCreate.Workspace = new kCura.Relativity.Client.DTOs.Workspace(workspaceId);
				
				try
				{
					var resultset = client.Repositories.Error.Create(errorToCreate);
					if (true == resultset.Success)
					{
						if (0 < resultset.Results.Count)
						{
							return resultset.Results[0].Artifact.ArtifactID;
						}
					}
				}
				catch (System.Exception ex2)
				{
					System.Diagnostics.Trace.TraceError(ex2.ToString());
				}
				return 0;
			}
		}
	}
}
