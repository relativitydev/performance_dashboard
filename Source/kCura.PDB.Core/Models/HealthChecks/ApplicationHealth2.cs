namespace kCura.PDB.Core.Models.HealthChecks
{
	using System;

	public class ApplicationHealth2
	{
        public int Id { get; set; }

		public int ArtifactID { get; set; }

		public int Errors { get; set; }

		public int LRQ { get; set; }

		public double Users { get; set; }

		public string WorkspaceName { get; set; }

		public string SQLInstanceName { get; set; }

		public DateTime MeasureDate { get; set; }
	}
}
