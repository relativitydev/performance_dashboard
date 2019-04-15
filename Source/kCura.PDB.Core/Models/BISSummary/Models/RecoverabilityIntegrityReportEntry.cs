namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class RecoverabilityIntegrityReportEntry
	{
		public int Id { get; set; }
		public int HourId { get; set; }
		public decimal OverallScore { get; set; }
		public decimal RpoScore { get; set; }
		public decimal RtoScore { get; set; }
		public decimal BackupFrequencyScore { get; set; }
		public decimal BackupCoverageScore { get; set; }
		public decimal DbccFrequencyScore { get; set; }
		public decimal DbccCoverageScore { get; set; }
	}
}
