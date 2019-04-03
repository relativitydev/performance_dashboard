namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Data;

	using kCura.PDB.Core.Models.BISSummary.Grids;

	public interface IRecoverabilityIntegrityReportReader
	{
		DataTableCollection GetBackupDbccHistoryDetails(
			GridConditions gridConditions,
			BackupDbccViewFilterConditions filterConditions,
			BackupDbccViewFilterOperands filterOperands);

		DataTableCollection GetRecoverabilityIntegrityDetails(
			GridConditions gridConditions,
			RecoverabilityIntegrityViewFilterConditions filterConditions,
			RecoverabilityIntegrityViewFilterOperands filterOperands);

		DataTableCollection GetRecoveryObjectivesDetails(
			GridConditions gridConditions,
			RecoveryObjectivesViewFilterConditions filterConditions,
			RecoveryObjectivesViewFilterOperands filterOperands);
	}
}
