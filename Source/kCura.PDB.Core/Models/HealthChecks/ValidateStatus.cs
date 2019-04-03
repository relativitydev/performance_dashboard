namespace kCura.PDB.Core.Models.HealthChecks
{
	using System.Runtime.Serialization;

	[DataContract]
	public enum ValidateStatus
	{
		[EnumMember]
		InvalidUserName,
		[EnumMember]
		InvalidPassword,
		[EnumMember]
		UserIsBlocked,
		[EnumMember]
		Success,
		[EnumMember]
		InvalidArguments
	}
}
