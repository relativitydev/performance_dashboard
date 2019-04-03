namespace kCura.PDB.Core.Models
{
	using System;

	public class Server
	{
		public int ServerId { get; set; }

		public string ServerName { get; set; }

		public DateTime CreatedOn { get; set; }

		public DateTime? DeletedOn { get; set; }

		public int ServerTypeId { get; set; }

		public string ServerIpAddress { get; set; }

		public bool? IgnoreServer { get; set; }

		public string ResponsibleAgent { get; set; }

		public int? ArtifactId { get; set; }

		public DateTime? LastChecked { get; set; }

		public string UptimeMonitoringResourceHost { get; set; }

		public bool? UptimeMonitoringResourceUseHttps { get; set; }

		public DateTime? LastServerBackup { get; set; }

		public string AdminScriptsVersion { get; set; }

		public bool IsQoSDeployed { get; set; }

		public ServerType ServerType
		{
			get { return (ServerType)this.ServerTypeId; }
			set { this.ServerTypeId = (int)value; }
		}
	}
}
