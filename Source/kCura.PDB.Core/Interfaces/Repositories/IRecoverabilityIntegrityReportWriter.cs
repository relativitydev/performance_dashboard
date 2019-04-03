namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IRecoverabilityIntegrityReportWriter
	{
		Task UpdateRecoveryObjectivesRpoReport(DatabaseRpoScoreData rpoScoreData);

		Task UpdateRecoveryObjectivesRtoReport(DatabaseRtoScoreData rtoScoreData);

		Task CreateGapReportData(GapReportEntry gapReportEntry);

		Task ClearUnresolvedGapReportData(int serverId, GapActivityType gapActivityType);

		Task CreateRecoverabilityIntegrityReportData(RecoverabilityIntegrityReportEntry recoverabilityIntegrityReportEntry);

		Task CreateRecoverabilityIntegrityRpoReport(WorstRpoReportEntry worstRpoEntry);

		Task CreateRecoverabilityIntegrityRtoReport(WorstRtoReportEntry worstRtoEntry);
	}
}
