namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IServerAuditReporter
	{
		Task ReportServerAudits(Server server, Hour hour, decimal score);

		Task FinalizeServerReports(Server server, Hour hour);

		Task DeleteServerTempReportData(Server server, Hour hour);
	}
}
