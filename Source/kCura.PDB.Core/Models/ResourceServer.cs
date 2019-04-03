namespace kCura.PDB.Core.Models
{
	public class ResourceServer
	{
		/// <summary>
		/// Gets or sets Server type like Web, Agent, database etc....
		/// </summary>
		public kCura.PDB.Core.Models.ServerType ServerType { get; set; }

		/// <summary>
		/// Gets or sets IP Address of the server
		/// </summary>
		public string IP { get; set; }

		public int ArtifactID { get; set; }

		/// <summary>
		/// Gets or sets Server Name
		/// </summary>
		public string Name { get; set; }

		public string Url { get; set; }

		/// <summary>
		/// Gets or sets Server Instance name
		/// </summary>
		public string ServerInstance { get; set; }
	}
}
