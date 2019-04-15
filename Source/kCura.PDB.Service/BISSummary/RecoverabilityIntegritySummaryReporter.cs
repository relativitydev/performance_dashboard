namespace kCura.PDB.Service.BISSummary
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class RecoverabilityIntegritySummaryReporter : IRecoverabilityIntegritySummaryReporter
	{
		private readonly IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository;
		private readonly IDatabaseRepository databaseRepository;

		public RecoverabilityIntegritySummaryReporter(
			IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository, IDatabaseRepository databaseRepository)
		{
			this.recoverabilityIntegrityReportRepository = recoverabilityIntegrityReportRepository;
			this.databaseRepository = databaseRepository;
		}

		public Task CreateRecoverabilityIntegritySummaryReport(
			int hourId,
			decimal overallScore,
			decimal rpoScore,
			decimal rtoScore,
			decimal backupFrequencyScore,
			decimal backupCoverageScore,
			decimal dbccFrequencyScore,
			decimal dbccCoverageScore)
		{
			var reportEntry = new RecoverabilityIntegrityReportEntry
			{
				HourId = hourId,
				OverallScore = overallScore,
				RpoScore = rpoScore,
				RtoScore = rtoScore,
				BackupFrequencyScore = backupFrequencyScore,
				BackupCoverageScore = backupCoverageScore,
				DbccFrequencyScore = dbccFrequencyScore,
				DbccCoverageScore = dbccCoverageScore
			};
			return this.recoverabilityIntegrityReportRepository.CreateRecoverabilityIntegrityReportData(reportEntry);
		}

		public async Task UpdateWorstRto(int hourId, int worstRtoDatabaseId, decimal rtoTimeToRecover)
		{
			var worstRtoDatabase = await this.databaseRepository.ReadAsync(worstRtoDatabaseId);
			var reportEntry = new WorstRtoReportEntry
			{
				HourId = hourId,
				WorstRtoDatabase = worstRtoDatabase.Name,
				RtoTimeToRecover = rtoTimeToRecover
			};
			await this.recoverabilityIntegrityReportRepository.CreateRecoverabilityIntegrityRtoReport(reportEntry);
		}

		public async Task UpdateWorstRpo(int hourId, int worstRpoDatabaseId, int rpoMaxDataLoss)
		{
			var worstRpoDatabase = await this.databaseRepository.ReadAsync(worstRpoDatabaseId);
			var reportEntry = new WorstRpoReportEntry
			{
				HourId = hourId,
				WorstRpoDatabase = worstRpoDatabase.Name,
				RpoMaxDataLoss = rpoMaxDataLoss
			};
			await this.recoverabilityIntegrityReportRepository.CreateRecoverabilityIntegrityRpoReport(reportEntry);
		}
	}
}