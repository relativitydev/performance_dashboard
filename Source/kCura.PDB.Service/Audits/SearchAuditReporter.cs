namespace kCura.PDB.Service.Audits
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.Reports;

	public class SearchAuditReporter : ISearchAuditReporter
	{
		private readonly IUserExperienceReportRepository userExperienceReportRepository;

		public SearchAuditReporter(IUserExperienceReportRepository userExperienceReportRepository)
		{
			this.userExperienceReportRepository = userExperienceReportRepository;
		}

		public Task ReportWorkspaceSearchAudits(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour, int serverId) =>
			GroupSearchAuditsForReport(workspaceSearchAudits, hour, serverId)
				.Pipe(this.userExperienceReportRepository.CreateUserExperienceSearchRecord);

		internal static IList<UserExperienceSearch> GroupSearchAuditsForReport(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour, int serverId) =>
			workspaceSearchAudits
				.GroupBy(a => new { SearchId = a.SearchArtifactId, a.UserId })
				.Select(ag => new UserExperienceSearch
				{
					HourId = hour.Id,
					ServerId = serverId,
					SearchId = ag.Key.SearchId,
					MinAuditId = ag.Min(sag => sag.Audits.Min(a => a.Audit.AuditID)),
					UserId = ag.Key.UserId,
					WorkspaceId = ag.First().WorkspaceId,
					IsComplex = ag.First().IsComplex,
					TotalSearchAudits = ag.Count(),
					PercentLongRunning = (decimal)ag.Count(SearchAnalysisService.IsLongRunning) / (decimal)ag.Count(),
					TotalExecutionTime = (int)ag.Sum(a => a.ExecutionTime)
				})
			.ToList();
	}
}
