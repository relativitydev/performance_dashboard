namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	/// <summary>
	/// Represents backup gaps for Full, Differential, and Log backups
	/// </summary>
	public class BackupAllGap : Gap
	{
		public override GapActivityType ActivityType { get; set; } = GapActivityType.Backup;
	}
}
