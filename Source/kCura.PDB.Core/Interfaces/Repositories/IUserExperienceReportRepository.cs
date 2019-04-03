namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Reports;

	public interface IUserExperienceReportRepository
	{
		Task CreateUserExperienceWorkspaceRecord(IList<UserExperienceWorkspace> workspaceSearches);

		Task CreateUserExperienceSearchRecord(IList<UserExperienceSearch> searches);

		Task CreateServerAuditRecord(UserExperienceServer serverAudits);

		Task UpdateSearchAuditRecord(Hour hour, Server server);

		Task CreateVarscatOutput(Hour hour, Server server);

		Task CreateVarscatOutputDetails(Hour hour, Server server);

		Task DeleteTempReportData(Hour hour, Server server);
	}
}
