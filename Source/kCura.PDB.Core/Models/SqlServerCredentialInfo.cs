namespace kCura.PDB.Core.Models
{
	using System;

	[Serializable]
	public class SqlServerCredentialInfo : GenericCredentialInfo
	{
		public string DatabaseName { get; set; }
		public string SqlServerInstance { get; set; }
	}
}
