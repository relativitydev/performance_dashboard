namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class BackupDbccGapInfo
	{
		public int Index;
		public int ServerId;
		public string Server;
		public string Database;
		public string Workspace;
		public DateTime? LastActivityDate;
		public string FriendlyLastActivityDate => LastActivityDate.HasValue
			? $"{LastActivityDate.Value.ToShortDateString()} {LastActivityDate.Value:t}"
			: "N/A";

		public bool IsBackup;
		public string FriendlyIsBackup => IsBackup
			? "Backup"
			: "DBCC";

		public DateTime? GapResolutionDate;
		public string FriendlyGapResolutionDate => GapResolutionDate.HasValue
			? $"{GapResolutionDate.Value.ToShortDateString()} {GapResolutionDate.Value.ToString("t")}"
			: "N/A";

		public int GapSize;
	}
}
