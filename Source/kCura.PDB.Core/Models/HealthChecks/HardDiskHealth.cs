namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public class HardDiskHealth : BaseServerHealth
	{
		[DataMember]
		public decimal DiskRead { get; set; }

		[DataMember]
		public decimal DiskWrite { get; set; }

		[DataMember]
		public int DiskNumber { get; set; }

		[DataMember]
		public string ServerDisk { get; set; }

		[DataMember]
		public string DriveLetter { get; set; }
	}
}
