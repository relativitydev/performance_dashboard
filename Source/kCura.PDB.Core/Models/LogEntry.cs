namespace kCura.PDB.Core.Models
{
	using System;
	using System.Collections.Generic;

	public class LogEntry
	{
		public int GrLogId { get; set; }
		public DateTime LogTimestampUtc { get; set; }

		public int? AgentId { get; set; }

		public string Module { get; set; }

		public string TaskCompleted { get; set; }

		public string OtherVars { get; set; }

		public string NextTask { get; set; }

		public int? LogLevel { get; set; }
	}
}
