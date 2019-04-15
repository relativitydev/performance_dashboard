namespace kCura.PDB.Core.Models
{
	using System;

	public class PDBNotification
	{
		public String Message { get; set; }
		public NotificationType Type { get; set; }
	}

	public enum NotificationType
	{
		Critical,
		Warning,
		Info
	}
}
