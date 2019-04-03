namespace kCura.PDB.Tests.Common.TestHelpers
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using global::Relativity.API;
	using kCura.PDB.Tests.Common.Extensions;
	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	public class CreateWorkspace
	{
		public static async Task<int> CreateWorkspaceAsync(string workspaceName, string templateName, IServicesMgr svcMgr, string userName, string password)
		{
			Relativity.Client.IRSAPIClient client = svcMgr.GetProxy<Relativity.Client.IRSAPIClient>(userName, password);
			int num;
			try
			{
				client.APIOptions.WorkspaceID = -1;
				num = await Task.Run<int>((Func<int>)(() => CreateWorkspace.Create(client, workspaceName, templateName)));
			}
			finally
			{
				if (client != null)
					client.Dispose();
			}
			return num;
		}

		public static int Create(Relativity.Client.IRSAPIClient proxy, string workspaceName, string templateName, int? serverId = null)
		{
			try
			{
				int num1 = 0;
				proxy.APIOptions.WorkspaceID = -1;
				if (templateName == string.Empty)
					throw new SystemException("Template name is blank in your configuration setting. Please add a template name to create a workspace");
				QueryResultSet<Workspace> artifactIdOfTemplate = CreateWorkspace.GetArtifactIdOfTemplate(proxy, templateName);
				if (!artifactIdOfTemplate.Success)
					return num1;
				int artifactId = artifactIdOfTemplate.Results.FirstOrDefault<Result<Workspace>>().Artifact.ArtifactID;
				Workspace createDTO = new Workspace();
				createDTO.Name = workspaceName;
				int? nullable1 = serverId ?? artifactIdOfTemplate.Results.FirstOrDefault<Result<Workspace>>().Artifact.ServerID;
				createDTO.ServerID = new int?(nullable1.Value);
				Relativity.Client.ProcessOperationResult processOperationResult = new Relativity.Client.ProcessOperationResult();
				try
				{
					Relativity.Client.ProcessOperationResult async = proxy.Repositories.Workspace.CreateAsync(artifactId, createDTO);
					if (!async.Success)
						throw new Exception(string.Format("workspace creation failed: {0}", (object)async.Message));
					ProcessInformation processState = proxy.GetProcessState(proxy.APIOptions, async.ProcessID);
					int num2 = 0;
					while (processState.State != ProcessStateValue.Completed)
					{
						Thread.Sleep(10000);
						processState = proxy.GetProcessState(proxy.APIOptions, async.ProcessID);
						if (num2 > 6)
							Console.WriteLine("Workspace creation timed out");
						++num2;
					}
					int num3 = processState.OperationArtifactIDs.FirstOrDefault<int?>().Value;
					Console.WriteLine("Workspace Created with Artiafact ID :" + (object)num3);
					return num3;
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Unhandled Exception : {0}", (object)ex));
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Create Workspace failed", ex);
			}
		}

		private static QueryResultSet<Workspace> GetArtifactIdOfTemplate(IRSAPIClient proxy, string templateName)
		{
			try
			{
				return proxy.Repositories.Workspace.Query(new Query<Workspace>()
				{
					Condition = (Relativity.Client.Condition)new TextCondition("Name", TextConditionEnum.EqualTo, templateName),
					Fields = FieldValue.AllFields
				}, 0);
			}
			catch (Exception ex)
			{
				Console.WriteLine((object)ex);
				throw;
			}
		}
	}
}
