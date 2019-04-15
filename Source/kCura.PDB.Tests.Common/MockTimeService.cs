namespace kCura.PDB.Tests.Common
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;

	internal class MockTimeService : ITimeService
	{
		private readonly Lazy<DateTime> initialDateTime = new Lazy<DateTime>(() => DateTime.UtcNow);
		private readonly double multiplier;
		private readonly Lazy<DateTime> startTime;

		public MockTimeService(double multiplier, DateTime? startTime)
		{
			this.multiplier = multiplier;
			this.startTime = new Lazy<DateTime>(() => startTime ?? DateTime.UtcNow);
		}

		public DateTime GetUtcNow() =>
			startTime.Value.AddSeconds((DateTime.UtcNow - initialDateTime.Value).TotalSeconds * multiplier);

		public void Sleep(TimeSpan timeSpan) =>
			Thread.Sleep(TimeSpan.FromSeconds(timeSpan.TotalSeconds / multiplier));

		public Task Delay(TimeSpan timeSpan) =>
			Task.Delay(TimeSpan.FromSeconds(timeSpan.TotalSeconds / multiplier));

		public Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken) =>
			Task.Delay(TimeSpan.FromSeconds(timeSpan.TotalSeconds / multiplier), cancellationToken);
	}
}
