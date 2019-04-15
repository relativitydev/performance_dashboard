namespace kCura.PDB.Core.Models.Audits
{
	public class UserExperienceRating
	{
		public int Id { get; set; }

		public int ServerArtifactId { get; set; }

		public decimal ArrivalRateUXScore { get; set; }

		public decimal ConcurrencyUXScore { get; set; }

		public int HourId { get; set; }
	}
}