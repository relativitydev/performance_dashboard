namespace kCura.PDB.Core.Models
{
	using System;

	public class MaintenanceWindow
	{
		public int Id { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public MaintenanceWindowReason Reason { get; set; }

		public string Comments { get; set; }

		public int DurationHours => (int)this.EndTime.Subtract(this.StartTime).TotalHours;

		public bool IsDeleted { get; set; }
	}
}
