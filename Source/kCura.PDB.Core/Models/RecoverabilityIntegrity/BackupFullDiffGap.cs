namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	/// <summary>
	/// Represents backup gaps for Full and Differential backups
	/// </summary>
	public class BackupFullDiffGap : Gap
	{
		public override GapActivityType ActivityType { get; set; } = GapActivityType.BackupFullAndDiff;
	}
}
