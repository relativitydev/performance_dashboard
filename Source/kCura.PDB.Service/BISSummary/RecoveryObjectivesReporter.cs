namespace kCura.PDB.Service.BISSummary
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class RecoveryObjectivesReporter : IRecoveryObjectivesReporter
	{
		private readonly IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository;

		public RecoveryObjectivesReporter(IRecoverabilityIntegrityReportWriter recoverabilityIntegrityReportRepository)
		{
			this.recoverabilityIntegrityReportRepository = recoverabilityIntegrityReportRepository;
		}

		public Task UpdateRpoReport(IList<DatabaseRpoScoreData> databaseRpoScoreData)
		{
			return databaseRpoScoreData.Select(d => this.recoverabilityIntegrityReportRepository.UpdateRecoveryObjectivesRpoReport(d)).WhenAllStreamed(1);
		}

		public Task UpdateRtoReport(IList<DatabaseRtoScoreData> databaseRtoScoreData)
		{
			return databaseRtoScoreData.Select(d => this.recoverabilityIntegrityReportRepository.UpdateRecoveryObjectivesRtoReport(d)).WhenAllStreamed(1);
		}
	}
}
