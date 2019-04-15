namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	public enum GapActivityType
	{
		/// <summary>
		/// Represents backup gaps for Full, Differential, and Log backups
		/// </summary>
		Backup = 1,

		/// <summary>
		/// Represents dbcc gaps
		/// </summary>
		Dbcc = 2,

		/// <summary>
		/// Represents backup gaps for Full and Differential backups
		/// </summary>
		BackupFullAndDiff = 3
	}
}
