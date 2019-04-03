namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public class SqlServerHealth : BaseServerHealth
	{
		[DataMember]
		public decimal PageLifeExpectancy { get; set; }
	}
}
