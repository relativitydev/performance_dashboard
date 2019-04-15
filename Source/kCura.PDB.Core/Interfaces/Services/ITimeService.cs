namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ITimeService
	{
		DateTime GetUtcNow();

		void Sleep(TimeSpan timeSpan);

		Task Delay(TimeSpan timeSpan);

	    Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken);
	}
}
