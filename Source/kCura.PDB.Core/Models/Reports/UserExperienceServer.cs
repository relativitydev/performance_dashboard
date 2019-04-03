namespace kCura.PDB.Core.Models.Reports
{
	public class UserExperienceServer
	{
		public int HourId { get; set; }

		public int ServerId { get; set; }

		public int WorkspaceId { get; set; }

		public decimal Score { get; set; }

		public int TotalUsers { get; set; }

		public long TotalAudits { get; set; }

		public long TotalLongRunning { get; set; }
	}
}
