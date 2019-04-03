namespace kCura.PDB.Core.Models
{
	public class EventLock
	{
		public long Id { get; set; }

		public long EventId { get; set; }

		public int EventTypeId { get; set; }

		public long? SourceId { get; set; }

		public int WorkerId { get; set; }
	}
}
