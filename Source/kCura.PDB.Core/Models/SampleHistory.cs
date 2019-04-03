namespace kCura.PDB.Core.Models
{
	using System;

	public class SampleHistory
	{
		public int Id { get; set; }

		public bool IsActiveArrivalRateSample { get; set; }

		public bool IsActiveConcurrencySample { get; set; }

		public int ServerId { get; set; }

		public int HourId { get; set; }
	}
}
