namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class EnvironmentCheckRecommendation
	{
		public String Scope { get; set; }
		public String Name { get; set; }
		public String Description { get; set; }
		public String Status { get; set; }
		public String Recommendation { get; set; }
		public String Value { get; set; }
		public String Section { get; set; }

		public Int32? Severity { get; set; }
	}
}
