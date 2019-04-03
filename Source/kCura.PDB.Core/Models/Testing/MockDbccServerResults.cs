namespace kCura.PDB.Core.Models.Testing
{
	using System;

	public class MockDbccServerResults
	{
		public string Server { get; set; }
		public string Database { get; set; }
		public int? CaseArtifactID { get; set; }
		public DateTime? LastCleanDBCCDate { get; set; }
	}
}
