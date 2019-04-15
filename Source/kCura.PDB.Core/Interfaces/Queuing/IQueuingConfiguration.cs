namespace kCura.PDB.Core.Interfaces.Queuing
{
	using Hangfire;

	public interface IQueuingConfiguration
	{
		void ConfigureSystem();
	}
}
