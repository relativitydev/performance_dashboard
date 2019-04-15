namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public class KeyValueDesc : KeyValue
	{
		[DataMember]
		public string Description { get; set; }
	}
}