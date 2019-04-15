namespace kCura.PDB.Agent
{
	using System.Runtime.InteropServices;
	using System.Threading;
	using kCura.Agent.CustomAttributes;
	using kCura.PDB.Core.Constants;

	[Name(Names.Agent.TrustWorkerAgentName)]
	[Guid(Guids.Agent.TrustWorkerAgentGuidString)]
	public class TrustWorkerAgent : AgentBaseExtended
	{
		protected override void ExecutePdbAgent(CancellationToken cancellationToken)
		{
			RaiseMessage("This agent has been deprecated and should be removed", 10);
		}

		public override string Name => Names.Agent.TrustWorkerAgentName;

		protected override bool ShowBeginEndMessages => true;
	}
}