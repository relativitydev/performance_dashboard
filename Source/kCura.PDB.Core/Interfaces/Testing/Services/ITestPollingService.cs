namespace kCura.PDB.Core.Interfaces.Testing.Services
{
	using System.Threading;
	using System.Threading.Tasks;

	public interface ITestPollingService
	{
		Task WaitUntilEventCompletionAsync(CancellationToken cancellationToken);
	}
}
