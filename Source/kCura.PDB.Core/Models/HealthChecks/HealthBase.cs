namespace kCura.PDB.Core.Models.HealthChecks
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[KnownType(typeof(ApplicationHealth))]
	[KnownType(typeof(HardDiskHealth))]
	[KnownType(typeof(RamHealth))]
	[KnownType(typeof(SqlServerHealth))]
	[KnownType(typeof(ProcesserHealth))]
	[KnownType(typeof(BaseServerHealth))]
	[Serializable]
	public class HealthBase
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DateTime MeasureDate { get; set; }
	}
}
