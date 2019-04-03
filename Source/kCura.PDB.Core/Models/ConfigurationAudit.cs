namespace kCura.PDB.Core.Models
{
	using System;

	public class ConfigurationAudit
	{
		public int Id { get; set; }

		public string FieldName { get; set; }

		public string OldValue { get; set; }
		
		public string NewValue { get; set; }

		public int UserId { get; set; }

		public DateTime CreatedOn { get; set; }

		public string ServerName { get; set; }
	}
}
