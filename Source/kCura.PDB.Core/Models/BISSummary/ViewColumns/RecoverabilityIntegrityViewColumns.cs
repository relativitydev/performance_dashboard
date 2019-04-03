namespace kCura.PDB.Core.Models.BISSummary.ViewColumns
{
	/// <summary>
	/// The names of these must match the strings used for filtering in the backing procedure,
	/// and the order of the columns must match the order in the grid.
	/// </summary>
	public enum RecoverabilityIntegrityViewColumns
	{
		SummaryDayHour = 0,
		RecoverabilityIntegrityScore = 1,
        BackupFrequencyScore = 2,
        BackupCoverageScore = 3,
        DbccFrequencyScore = 4,
        DbccCoverageScore = 5,
        RPOScore = 6,
        RTOScore = 7
	}
}