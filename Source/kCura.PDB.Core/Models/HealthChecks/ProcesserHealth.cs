namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public class ProcesserHealth : BaseServerHealth
	{
		[DataMember]
		public decimal ProcesserTime { get; set; }

		[DataMember]
		public decimal CPUProcessorTime { get; set; }

		[DataMember]
		public string ServerCore { get; set; }
	}
}
