namespace kCura.PDB.Core.Models
{
	using System;

	public class Hour
	{
		public int Id { get; set; }

		public DateTime HourTimeStamp { get; set; }

		public decimal? Score { get; set; }

		public bool InSample { get; set; }

		public DateTime? StartedOn { get; set; }

		public DateTime? CompletedOn { get; set; }

		public HourStatus Status { get; set; }
	}

	public enum HourStatus
	{
		Pending = 1,
		Started = 2,
		Completed = 3,
		Cancelled = 4,
	}
}
