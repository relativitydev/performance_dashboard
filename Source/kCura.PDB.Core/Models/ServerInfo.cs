namespace kCura.PDB.Core.Models
{
	using System;

	[Obsolete("Should be replaced with ResourceServer")]
	public class ServerInfo
	{
		public int ArtifactId { get; set; }

		public string Name { get; set; }

		public int Status { get; set; }

		public string Version { get; set; }

		public string ServerType { get; set; }
	}
}
