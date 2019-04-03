namespace kCura.PDB.Core.Models.MetricDataSources
{
	using System;

	public class UserExperience
	{
		public int ServerId { get; set; }

		public int HourId { get; set; }

		public decimal ArrivalRate { get; set; }

		public decimal Concurrency { get; set; }

		public int ActiveUsers { get; set; }

		public bool HasPoisonWaits { get; set; }
	}
}
