namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Threading.Tasks;

	public interface IRecoverabilityIntegritySummaryReporter
	{
		Task CreateRecoverabilityIntegritySummaryReport(
			int hourId,
			decimal overallScore,
			decimal rpoScore,
			decimal rtoScore,
			decimal backupFrequencyScore,
			decimal backupCoverageScore,
			decimal dbccFrequencyScore,
			decimal dbccCoverageScore);

		Task UpdateWorstRto(int hourId, int worstRtoDatabaseId, decimal rtoTimeToRecover);

		Task UpdateWorstRpo(int hourId, int worstRpoDatabaseId, int rpoMaxDataLoss);
	}
}
