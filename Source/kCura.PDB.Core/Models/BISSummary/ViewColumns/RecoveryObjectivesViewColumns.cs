namespace kCura.PDB.Core.Models.BISSummary.ViewColumns
{
	/// <summary>
	/// The names of these must match the strings used for filtering in the backing procedure,
	/// and the order of the columns must match the order in the grid.
	/// </summary>
	public enum RecoveryObjectivesViewColumns
	{
        ServerName = 0,
		DBName = 1,
        RPOScore = 2,
        RTOScore = 3,
        PotentialDataLossMinutes = 4,
        EstimatedTimeToRecoverHours = 5
	}
}