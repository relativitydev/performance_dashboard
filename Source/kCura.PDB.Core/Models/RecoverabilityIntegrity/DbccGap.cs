namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	public class DbccGap : Gap
	{
		public override GapActivityType ActivityType { get; set; } = GapActivityType.Dbcc;
	}
}
