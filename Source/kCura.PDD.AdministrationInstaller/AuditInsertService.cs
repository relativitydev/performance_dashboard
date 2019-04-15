namespace kCura.PDD.AdministrationInstaller
{
	using System;
	using System.Threading.Tasks;
	using Dapper;
	using global::Relativity.API;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data;
	using kCura.PDB.Tests.Common.TestHelpers;

	public class AuditInsertService
	{
		private readonly IConnectionFactory connectionFactory;

		public AuditInsertService(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public async Task SetupWorkspaceAudits(string baseTemplateName, string primaryServerName, string username, string password, string injectAuditDataQuery, IServicesMgr servicesManager)
		{
			// Setup
			var testWorkspaceName = "TestWorkspaceName";
			DataSetup.Setup();

			// Create Workspace
			var workspaceId = await CreateWorkspace.CreateWorkspaceAsync(testWorkspaceName, baseTemplateName, servicesManager, username, password);

			// Inject Audits by running SQL script against the workspace database
			using (var conn = await connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				await conn.ExecuteAsync(
					injectAuditDataQuery,
					new
					{
						workspace = workspaceId,
						startHour = DateTime.UtcNow.AddDays(-7),
						endHour = DateTime.UtcNow,
						suggestedExecutionTime = (int?)null,
						usersPerHour = 3,
						minExecutionTime = 1000,
						maxExecutionTime = 10000,
						auditsPerHour = 1000,
						primaryServerName
					});
			}
		}
	}
}
