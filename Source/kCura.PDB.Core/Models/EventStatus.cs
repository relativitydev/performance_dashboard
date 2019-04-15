namespace kCura.PDB.Core.Models
{
	public enum EventStatus
	{
		Pending = 1,
		InProgress = 2,
		Completed = 3,
		Error = 4,

		PendingHangfire = 5,
		Duplicate = 100,
		Cancelled = 110,
		Expired = 120,
	}
}
