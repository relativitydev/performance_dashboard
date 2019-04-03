namespace kCura.PDB.Core.Models.Testing
{
	using System;

	public class MockServer
	{
		public string ServerName { get; set; }
		public int ServerTypeID { get; set; }
		public int ArtifactID { get; set; }
		public DateTime? LastServerBackup { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? DeletedOn { get; set; }
		public bool IgnoreServer { get; set; }
	}
}
