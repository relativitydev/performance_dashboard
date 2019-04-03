namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class RecoveryObjectiveReportEntry
	{
		public int Id { get; set; }
		public int DatabaseId { get; set; }
		public decimal RpoScore { get; set; }
		public int RpoMaxDataLoss { get; set; }
		public decimal RtoScore { get; set; }
		public decimal RtoTimeToRecover { get; set; }
	}
}
