namespace kCura.PDB.Core.Models
{
	using System;
	using System.Linq;

	public class Database
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int ServerId { get; set; }

		public int? WorkspaceId { get; set; }

		public DatabaseType Type { get; set; }

		public DateTime? DeletedOn { get; set; }

		public bool Ignore { get; set; }

		public DateTime? LastDbccDate { get; set; }
		public DateTime? LastBackupLogDate { get; set; }
		public DateTime? LastBackupDiffDate { get; set; }
		public DateTime? LastBackupFullDate { get; set; }
		public int? LastBackupFullDuration { get; set; }
		public int? LastBackupDiffDuration { get; set; }
		public int? LogBackupsDuration { get; set; }
		public DateTime? MostRecentBackupAnyType =>
			new[] { this.LastBackupFullDate, this.LastBackupDiffDate, this.LastBackupLogDate }.Where(d => d.HasValue).Max();
	}

	public enum DatabaseType
	{
		Primary = 0,
		Workspace = 1,
		Invariant = 2,
		Other = 99
	}
}
