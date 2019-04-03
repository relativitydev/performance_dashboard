namespace kCura.PDB.Core.Models
{
	using System;

	public class AgentHistory
	{
		public int Id { get; set; }

		public int AgentArtifactId { get; set; }

		public DateTime TimeStamp { get; set; }

		public bool Successful { get; set; }
	}
}
