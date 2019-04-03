namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class EnvironmentCheckServerDetails
	{
		public String ServerName { get; set; }
		public String OSVersion { get; set; }
		public String OSName { get; set; }
		public Int32 LogicalProcessors { get; set; }
		public Boolean Hyperthreaded { get; set; }
		public String ServerIPAddress { get; set; }
	}
}
