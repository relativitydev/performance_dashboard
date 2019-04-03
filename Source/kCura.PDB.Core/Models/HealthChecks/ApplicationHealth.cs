namespace kCura.PDB.Core.Models.HealthChecks
{
	public class ApplicationHealth : HealthBase
	{
		public int CaseArtifactId { get; set; }

		public int ErrorCount { get; set; }

		public int LRQCount { get; set; } // Long running queries

		public int UserCount { get; set; }

		public string WorkspaceName { get; set; }

		public string DatabaseLocation { get; set; }
	}
}
