namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class FileLatency
	{
		public String ServerName { get; set; }
		public String DatabaseName { get; set; }
		public decimal? Score { get; set; }
		public long? DataReadLatency { get; set; }
		public long? DataWriteLatency { get; set; }
		public long? LogReadLatency { get; set; }
		public long? LogWriteLatency { get; set; }
		public enum Columns : int
		{
			ServerName = 0,
			DatabaseName = 1,
			Score = 2,
			DataReadLatency = 3,
			DataWriteLatency = 4,
			LogReadLatency = 5,
			LogWriteLatency = 6,
		}
	}
}
