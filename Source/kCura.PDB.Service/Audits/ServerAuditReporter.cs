namespace kCura.PDB.Service.Audits
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Reports;

	public class ServerAuditReporter : IServerAuditReporter
	{
		private readonly IUserExperienceReportRepository userExperienceReportRepository;
		private readonly IWorkspaceService workspaceService;
		private readonly IWorkspaceAuditServiceFactory workspaceAuditServiceFactory;
		private readonly ISearchAuditBatchRepository searchAuditBatchRepository;

		public ServerAuditReporter(
			IUserExperienceReportRepository userExperienceReportRepository,
			IWorkspaceService workspaceService,
			IWorkspaceAuditServiceFactory workspaceAuditServiceFactory,
			ISearchAuditBatchRepository searchAuditBatchRepository)
		{
			this.userExperienceReportRepository = userExperienceReportRepository;
			this.workspaceService = workspaceService;
			this.workspaceAuditServiceFactory = workspaceAuditServiceFactory;
			this.searchAuditBatchRepository = searchAuditBatchRepository;
		}

		// Report Server Audits needs to report all data for that workspace
		public async Task ReportServerAudits(Server server, Hour hour, decimal score)
		{
			var workspaceIds = await this.workspaceService.ReadAvailableWorkspaceIdsAsync(server.ServerId);
			var workspaceRepos = await workspaceIds.Select(async wid => new
			{
				Id = wid,
				AuditRepo = await this.workspaceAuditServiceFactory.GetAuditService(wid, hour.Id)
			}).WhenAllStreamed();

			var batchInfoGroups = this.searchAuditBatchRepository.ReadByHourAndServer(hour.Id, server.ServerId).GroupBy(b => b.WorkspaceId);
			await workspaceRepos.Select(async w => this.userExperienceReportRepository.CreateServerAuditRecord(new UserExperienceServer
			{
				WorkspaceId = w.Id,
				HourId = hour.Id,
				ServerId = server.ServerId,
				Score = score,
				TotalAudits = await w.AuditRepo.ReadTotalAuditsForHourAsync(w.Id, hour.Id, AuditConstants.RelevantAuditActionIds),
				TotalUsers = await w.AuditRepo.ReadTotalUniqueUsersForHourAuditsAsync(w.Id, hour.Id, AuditConstants.RelevantAuditActionIds),
				TotalLongRunning = batchInfoGroups.FirstOrDefault(g => g.Key == w.Id)?.Sum(b => b.BatchResults?.Sum(r => r.TotalLongRunningQueries) ?? 0) ?? 0
			}))
			.WhenAllStreamed();
		}

		public Task FinalizeServerReports(Server server, Hour hour) =>
				Task.WhenAll(
					this.userExperienceReportRepository.CreateVarscatOutput(hour, server),
					this.userExperienceReportRepository.CreateVarscatOutputDetails(hour, server),
					this.userExperienceReportRepository.UpdateSearchAuditRecord(hour, server));

		public Task DeleteServerTempReportData(Server server, Hour hour) =>
			this.userExperienceReportRepository.DeleteTempReportData(hour, server);
	}
}
