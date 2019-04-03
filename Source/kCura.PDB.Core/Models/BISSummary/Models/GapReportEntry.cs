namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class GapReportEntry
	{
		public int Id { get; set; }
		public int DatabaseId { get; set; }
		public int ActivityType { get; set; }
		public DateTime LastActivity { get; set; }
		public DateTime? GapResolutionDate { get; set; }
		public int GapSize { get; set; }
	}
}
