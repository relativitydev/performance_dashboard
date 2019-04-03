namespace kCura.PDB.Core.Models
{
	public class CategoryScore
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }

		public int? ServerId { get; set; }

		public decimal? Score { get; set; }

		public Category Category { get; set; }

		public Server Server { get; set; }
	}
}
