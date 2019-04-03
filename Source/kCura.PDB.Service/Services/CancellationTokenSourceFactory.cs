namespace kCura.PDB.Service.Services
{
	using System;
	using System.Threading;
	using kCura.PDB.Core.Interfaces.Services;

	public class CancellationTokenSourceFactory : ICancellationTokenSourceFactory
	{
		public CancellationTokenSource GetCancellationTokenSource()
		{
			return new CancellationTokenSource();
		}

		public CancellationTokenSource GetTimeoutCancellationTokenSource(TimeSpan timeout)
		{
			return new CancellationTokenSource(timeout);
		}

		public CancellationTokenSource CreateLinkedTokenSource(CancellationToken token, CancellationToken otherToken)
		{
			return CancellationTokenSource.CreateLinkedTokenSource(token, otherToken);
		}
	}
}
