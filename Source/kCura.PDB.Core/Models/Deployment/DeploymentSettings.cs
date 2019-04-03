namespace kCura.PDB.Core.Models.Deployment
{
	using System;

	public class DeploymentSettings
	{
		public GenericCredentialInfo CredentialInfo { get; set; }

		public string Server { get; set; }

		public string CreateScriptName { get; set; }

		public byte[] MigrationResource { get; set; }

		public string DatabaseName { get; set; }
	}
}
