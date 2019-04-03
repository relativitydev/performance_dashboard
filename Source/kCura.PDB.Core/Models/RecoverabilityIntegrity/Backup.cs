namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	using System;

	public class Backup
	{
		public string DatabaseName { get; set; }

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public char BackupType { get; set; }

		public BackupType Type
		{
			get { return (BackupType)this.BackupType; }
			set { this.BackupType = (char)value; }
		}

		private int? duration;
		public int Duration
		{
			get { return duration ?? (int)(End - Start).TotalMinutes; }
			set { this.duration = value; }
		}
	}
}
