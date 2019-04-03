namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public class RamHealth : BaseServerHealth
	{
		[DataMember]
		public decimal PagesPerSec { get; set; }

		[DataMember]
		public decimal PageFaultsPerSec { get; set; }

		[DataMember]
		public decimal Percentage { get; set; }
	}
}
