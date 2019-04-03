namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class DatabaseRpoScoreData
	{
		public int DatabaseId { get; set; }
		public int? PotentialDataLoss { get; set; }
		public decimal RpoScore { get; set; }
	}
}
