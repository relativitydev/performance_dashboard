namespace kCura.PDB.Core.Models
{
	public class MetricData
	{
		public int Id { get; set; }

		public int MetricId { get; set; }

		public int? ServerId { get; set; }

		public string Data { get; set; }

		public decimal? Score { get; set; }

		public Metric Metric { get; set; }

		public Server Server { get; set; }
	}
}
