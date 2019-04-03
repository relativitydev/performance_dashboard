namespace kCura.PDB.Core.Models
{
	public class LoopEventResult : EventResult
	{
		public LoopEventResult(bool shouldContinue)
			: base(true)
		{
			this.ShouldContinue = shouldContinue;
		}

	}
}
