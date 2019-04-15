namespace kCura.PDB.Core.Models
{
	using System;
	using System.Security.Policy;

	public class Meter
	{
		public TimeSpan ElapsedTime { get; set; }

		public int Count { get; set; }

		public TimeSpan MaxElapsedTime { get; set; }

		public void Increment(TimeSpan additionalTime)
		{
			this.ElapsedTime += additionalTime;
			if (additionalTime > this.MaxElapsedTime)
			{
				this.MaxElapsedTime = additionalTime;
			}
			Increment();
		}

		public void Increment() => this.Count++;
	}
}
