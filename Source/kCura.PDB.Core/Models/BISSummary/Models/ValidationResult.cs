namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class ValidationResult
	{
		public bool Valid { get; set; }
		public string Details { get; set; }

		public ValidationResult()
		{
			Valid = true;
			Details = string.Empty;
		}
	}
}
