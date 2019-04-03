namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	[KnownType(typeof(KeyValueDesc))]
	public class KeyValue
	{
		[DataMember]
		public int Key { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}