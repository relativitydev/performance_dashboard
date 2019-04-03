namespace kCura.PDB.Service.Services
{
	using System;
	using kCura.PDB.Core.Interfaces.Services;

	public class Timeout : ITimeout
	{
		private readonly TimeSpan timeoutTimeSpan;
		private DateTime timeout;

		public Timeout(TimeSpan timeoutTimeSpan)
		{
			this.timeoutTimeSpan = timeoutTimeSpan;
			this.Reset();
		}

		public void Reset()
		{
			this.timeout = DateTime.UtcNow.Add(this.timeoutTimeSpan);
		}

		public bool IsAfterTimedOut =>
			DateTime.UtcNow >= this.timeout;

		public TimeSpan TimeRemaining
		{
			get
			{
				// Timeout is ahead of current time when created. So the time remaining should b timeout - now.
				// If sufficient time has elapsed and the current time is now after the timeout then there is no time remaining.
				var value = this.timeout - DateTime.UtcNow;
				return value.TotalMilliseconds > (double)0
					? value
					: TimeSpan.FromSeconds(0);
			}
		}
	}
}
