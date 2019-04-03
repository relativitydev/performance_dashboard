namespace kCura.PDB.Service.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Apm;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Reports;

	public class WorkspaceAuditReporter : IWorkspaceAuditReporter
	{
		private readonly IUserExperienceReportRepository userExperienceReportRepository;
		private readonly ISearchAuditReporter searchAuditReporter;
		private readonly IWorkspaceAuditApmReporter workspaceAuditApmReporter;

		public WorkspaceAuditReporter(IUserExperienceReportRepository userExperienceReportRepository, ISearchAuditReporter searchAuditReporter, IWorkspaceAuditApmReporter workspaceAuditApmReporter)
		{
			this.userExperienceReportRepository = userExperienceReportRepository;
			this.searchAuditReporter = searchAuditReporter;
			this.workspaceAuditApmReporter = workspaceAuditApmReporter;
		}

		public Task ReportWorkspaceAudits(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour, int serverId)
		{
			var createTask = GroupSearchAuditsForWorkspaceReport(workspaceSearchAudits, hour, serverId)
				.Pipe(this.userExperienceReportRepository.CreateUserExperienceWorkspaceRecord);

			this.workspaceAuditApmReporter.ReportAuditDataToApm(workspaceSearchAudits, hour);

			return Task.WhenAll(
				this.searchAuditReporter.ReportWorkspaceSearchAudits(workspaceSearchAudits, hour, serverId),
				createTask);
		}

		internal static IList<UserExperienceWorkspace> GroupSearchAuditsForWorkspaceReport(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour, int serverId) =>
			workspaceSearchAudits
				.GroupBy(sa => new { sa.SearchArtifactId, sa.UserId })
				.Select(ag => new UserExperienceWorkspace
				{
					HourId = hour.Id,
					ServerId = serverId,
					IsComplex = ag.First().IsComplex,
					SearchId = ag.Key.SearchArtifactId,
					SearchName = ag.First().Audits.First().Search?.Name ?? "AdHoc Search",
					WorkspaceId = ag.First().WorkspaceId,
					TotalExecutionTime = ag.Sum(a => a.ExecutionTime),
					TotalSearchAudits = ag.Count()
				})
				.ToList();

	}
}
