namespace kCura.PDB.Core.Models
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class MigrationResultSet
	{
		public MigrationResultSet()
		{
			this.Messages = new LogMessage[0];
		}

		public MigrationResultSet(bool success, IList<LogMessage> messages)
		{
			this.Success = success;
			this.Messages = messages;
		}

		public bool Success { get; set; }

		public IList<LogMessage> Messages { get; set; }
	}
}