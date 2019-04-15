namespace kCura.PDB.Core.Models.Audits
{
	using kCura.PDB.Core.Enumerations;

	/// <summary>
	/// This model is meant to hold the data obtained after parsing a given Audit with ActionTypeId 28 (Search).
	/// </summary>
	public class SearchAudit
	{
		/// <summary>
		/// Gets or sets the base Audit
		/// </summary>
		public Audit Audit { get; set; }

		/// <summary>
		/// Gets or sets the base search details from a saved search
		/// </summary>
		public Search Search { get; set; }

		public bool IsComplex { get; set; }

		public QueryType? QueryType { get; set; }

		public string QueryId { get; set; }
	}
}
