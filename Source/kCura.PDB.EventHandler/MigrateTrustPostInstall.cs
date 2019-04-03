namespace kCura.PDB.EventHandler
{
	using kCura.EventHandler;
	using kCura.EventHandler.CustomAttributes;

	[RunOnce(true)]
	[Description("Deprecated PDB Post Installer")]
	[System.Runtime.InteropServices.Guid("77C1C939-E5D3-4DEE-8B6E-0C24ABC4D822")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.InstanceAndWorkspace)]
	public class MigrateTrustPostInstall : PostInstallEventHandler
	{
		public override Response Execute()
		{
			// Return response
			return new Response { Message = "MigrateTrustPostInstall completed", Success = true };
		}
	}
}