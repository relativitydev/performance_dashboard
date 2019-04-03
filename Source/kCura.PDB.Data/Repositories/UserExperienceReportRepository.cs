namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Reports;
	using kCura.PDB.Data.Properties;

	public class UserExperienceReportRepository : IUserExperienceReportRepository
	{
		public UserExperienceReportRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;
		private readonly int batchSize = 50;

		public async Task CreateUserExperienceWorkspaceRecord(IList<UserExperienceWorkspace> workspaceSearches)
		{
			// breaks searches into batches of 100 do reduce how many records need to be created by a single connection
			await Enumerable
				.Range(0, (int)Math.Ceiling((decimal)workspaceSearches.Count / (decimal)batchSize))
				.Select(i => Task.Run(() =>
				{
					using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
					{
						conn.Execute(Resources.Reports_WorkspaceSearchAudits_Create, workspaceSearches.Skip(i * batchSize).Take(batchSize));
					}
				}))
				.WhenAllStreamed();
		}

		public async Task CreateUserExperienceSearchRecord(IList<UserExperienceSearch> searches)
		{
			// breaks searches into batches of 100 do reduce how many records need to be created by a single connection
			await Enumerable
				.Range(0, (int)Math.Ceiling((decimal)searches.Count / (decimal)batchSize))
				.Select(i => Task.Run(() =>
				{
					using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
					{
						conn.Execute(Resources.Reports_SearchAudit_Create, searches.Skip(i * batchSize).Take(batchSize));
					}
				}))
				.WhenAllStreamed();
		}

		public async Task CreateServerAuditRecord(UserExperienceServer serverAudits)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Reports_ServerAudits_Create, serverAudits);
			}
		}

		public Task UpdateSearchAuditRecord(Hour hour, Server server)
		{
			// wrapping entire operation in task so connection and query are executed in single task due to expected long execution and help prevent sql timeout
			return Task.Run(() =>
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					conn.Execute(Resources.Reports_SearchAudit_CreateSummaryRecord, new { hourId = hour.Id, server.ServerId, });
				}
			});
		}

		public Task CreateVarscatOutput(Hour hour, Server server)
		{
			// wrapping entire operation in task so connection and query are executed in single task due to expected long execution and help prevent sql timeout
			return Task.Run(() =>
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					conn.Execute(Resources.Reports_WorkspaceSearchAudits_CreateVarscatOutput, new { hourId = hour.Id, server.ServerId, });
				}
			});
		}

		public Task CreateVarscatOutputDetails(Hour hour, Server server)
		{
			// wrapping entire operation in task so connection and query are executed in single task due to expected long execution and help prevent sql timeout
			return Task.Run(() =>
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					conn.Execute(Resources.Reports_WorkspaceSearchAudits_CreateVarscatOutputDetails, new { hourId = hour.Id, server.ServerId, });
				}
			});
		}

		public Task DeleteTempReportData(Hour hour, Server server)
		{
			// wrapping entire operation in task so connection and query are executed in single task due to expected long execution and help prevent sql timeout
			return Task.Run(() =>
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					conn.Execute(Resources.Reports_SearchAndWorkspace_Delete, new { hourId = hour.Id, server.ServerId, });
				}
			});
		}
	}
}
