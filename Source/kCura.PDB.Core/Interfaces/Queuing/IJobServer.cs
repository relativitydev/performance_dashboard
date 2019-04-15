namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IJobServer : IDisposable
	{
		void Start();

		Task WaitTillProcessesAreDone(CancellationToken cancellationToken);
	}
}
