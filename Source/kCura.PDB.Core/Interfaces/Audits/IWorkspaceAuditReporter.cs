namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;

	public interface IWorkspaceAuditReporter
	{
		Task ReportWorkspaceAudits(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour, int serverId);
	}
}
