namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Threading;

	public interface ICancellationTokenSourceFactory
	{
		CancellationTokenSource GetCancellationTokenSource();

		CancellationTokenSource GetTimeoutCancellationTokenSource(TimeSpan timeout);

		CancellationTokenSource CreateLinkedTokenSource(CancellationToken token, CancellationToken otherToken);
	}
}
