namespace kCura.PDB.Core.Models.Audits
{
	public class ViewCriteria
	{
		public string Operator { get; set; }

		public string Value { get; set; }

		public bool IsSubSearch { get; set; }

		public Search SubSearch { get; set; }
	}
}
