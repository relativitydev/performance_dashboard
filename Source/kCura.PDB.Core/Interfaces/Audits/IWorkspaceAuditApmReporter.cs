namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;

	public interface IWorkspaceAuditApmReporter
	{
		void ReportAuditDataToApm(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour);
	}
}
