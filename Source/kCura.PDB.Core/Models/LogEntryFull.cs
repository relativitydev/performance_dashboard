namespace kCura.PDB.Core.Models
{
	using System;

	public class LogEntryFull : LogEntry
	{
		public long? EventId { get; set; }

		public int? EventTypeId { get; set; }

		public int? EventStatusId { get; set; }

		public DateTime? EventTimeStamp { get; set; }

		public int? EventDelay { get; set; }

		public long? PreviousEventId { get; set; }

		public DateTime? EventLastUpdated { get; set; }

		public int? EventRetries { get; set; }

		public int? EventExecutionTime { get; set; }

		public int? EventHourId { get; set; }

		public EventStatus? EventStatus
		{
			get { return (EventStatus?)this.EventStatusId; }
			set { this.EventStatusId = (int?)value; }
		}

		public EventSourceType? EventType
		{
			get { return (EventSourceType?)this.EventTypeId; }
			set { this.EventTypeId = (int?)value; }
		}
	}
}
