namespace kCura.PDB.Core.Models
{
	using System;

	public class Event
	{
		public long Id { get; set; }

		public int SourceTypeId { get; set; }

		public int? SourceId { get; set; }

		public int StatusId { get; set; }

		public DateTime TimeStamp { get; set; }

		public DateTime LastUpdated { get; set; }

		public string EventHash { get; set; }

		public int? HourId { get; set; }

		public int? Delay { get; set; }

		public long? PreviousEventId { get; set; }

		public int? ExecutionTime { get; set; }

		public int? Retries { get; set; }

		public EventStatus Status
		{
			get { return (EventStatus)this.StatusId; }
			set { this.StatusId = (int)value; }
		}

		public EventSourceType SourceType
		{
			get { return (EventSourceType)this.SourceTypeId; }
			set { this.SourceTypeId = (int)value; }
		}
	}
}
