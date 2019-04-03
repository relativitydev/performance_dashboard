namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class BackupDbccViewFilterConditions
	{
		public string Server { get; set; }

		public string Database { get; set; }

		public DateTime? LastActivityDate { get; set; }

		public bool? IsBackup { get; set; }

		public DateTime? ResolutionDate { get; set; }

		public int? GapSize { get; set; }

		public string FriendlyLastActivityDate => LastActivityDate.HasValue
			? $"{LastActivityDate.Value.ToShortDateString()} {LastActivityDate.Value.ToShortTimeString()}"
			: string.Empty;

		public string FriendlyResolutionDate => ResolutionDate.HasValue
			? $"{ResolutionDate.Value.ToShortDateString()} {ResolutionDate.Value.ToShortTimeString()}"
			: string.Empty;

		public string FriendlyIsBackup => IsBackup.HasValue
			? IsBackup.Value
				? "Backup"
				: "DBCC"
			: string.Empty;
	}
}
