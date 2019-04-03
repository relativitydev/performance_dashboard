namespace kCura.PDB.Core.Services
{
	using System;
	using System.Data;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class TimeService : ITimeService
	{
		public DateTime GetUtcNow()
		{
			return DateTime.UtcNow;
		}

		public void Sleep(TimeSpan timeSpan)
		{
			Thread.Sleep(timeSpan);
		}

		public Task Delay(TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan);
		}

		public Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
		{
			return Task.Delay(timeSpan, cancellationToken);
		}
	}
}
