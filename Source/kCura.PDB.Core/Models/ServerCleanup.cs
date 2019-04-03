namespace kCura.PDB.Core.Models
{
	public class ServerCleanup
	{
		public int Id { get; set; }

		public int HourId { get; set; }

		public int ServerId { get; set; }

		public bool Success { get; set; }
	}
}
