namespace kCura.PDB.Core.Models.BISSummary.ViewColumns
{
	/// <summary>
	/// The names of these must match the strings used for filtering in the backing procedure,
	/// and the order of the columns must match the order in the grid.
	/// </summary>
	public enum BackupDbccViewColumns
	{
		ServerName = 0,
		DatabaseName = 1,
		IsBackup = 2,
		LastActivityDate = 3,
		ResolutionDate = 4,
		GapSize = 5
	}
}