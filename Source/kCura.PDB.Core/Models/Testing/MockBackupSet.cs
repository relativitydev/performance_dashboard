namespace kCura.PDB.Core.Models.Testing
{
	using System;

	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class MockBackupSet
	{
		public string Server { get; set; }
		public string Database { get; set; }
		public DateTime BackupStartDate { get; set; }
		public DateTime BackupEndDate { get; set; }

		public string BackupType
		{
			get
			{
				return ((char)this._backupType).ToString();
			}
			set
			{
				this._backupType = (BackupType)Enum.ToObject(typeof(BackupType), value[0]);
			}
		}

		private BackupType _backupType;
	}
}
