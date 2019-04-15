namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public abstract class BaseServerHealth : HealthBase
	{
		[DataMember]
		public string ServerType { get; set; }

		[DataMember]
		public string Server { get; set; }
	}
}
