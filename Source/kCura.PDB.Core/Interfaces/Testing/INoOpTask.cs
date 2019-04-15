namespace kCura.PDB.Core.Interfaces.Testing
{
	using System.Threading.Tasks;

	public interface INoOpTask
	{
		Task FailsThenSucceeds(long eventId);
	}
}
