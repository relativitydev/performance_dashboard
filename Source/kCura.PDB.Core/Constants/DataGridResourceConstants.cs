namespace kCura.PDB.Core.Constants
{
	using System;

	public class DataGridResourceConstants
	{
		public const string ChoiceTypeIdString = "Choice Type ID";
		public const string DataGridFieldAuditId = "Audit ID";
		public const string DataGridFieldUserId = "UserID";
		public const string DataGridFieldAuditArtifactId = "Object ArtifactID";
		public const string DataGridFieldDetails = "Details";
		public const string DataGridFieldTimeStamp = "Timestamp";
		public const string DataGridFieldAction = "Action";
		public const string DataGridFieldExecutionTime = "Execution Time (ms)";

		public const string DataGridKeplerApplicationRequestContentType = "application/json";

		// This is what was given by Prabhath
		public const string ObjectManagerFormatRequestPath =
			"/API/Relativity.Objects.Audits/workspaces/{0}/audits/query/withdetails";

		public const string PivotRequestPath =
			"/API/kCura.AuditUI2.Services.ExternalData.ExternalAuditLogModule/External%20Audit%20Log%20Manager/ExecuteAsync";

		public const string AuditArtifactTypeString = "B25B3669-79BC-414E-9F3F-BDA607B676CA";
		public const string ActionFieldString = "3C417469-CB89-4C22-8B5B-4F526BA7ECAA";
		public const string TimeStampFieldString = "CC7DBA18-4F84-45E5-A06E-EB5C61AADD72";
		public const string UserNameFieldString = "8C669259-810C-495D-90BE-3B1C162B367C";
		public const string ObjectFieldString = "DCE0D08A-D03F-48C5-B782-08F8FAEF50FF";

		public const string DataGridAuditMinVersion = "12.9.0.14";
		public const string DataGridGrandTotal = "Grand Total";
		public const string DataGridUserName = "UserName";

		/// <summary>
		/// Returns Guid for DataGridAudit ArtifactTypeId
		/// </summary>
		public static readonly Guid AuditArtifactTypeGuid = new Guid(AuditArtifactTypeString);

		/// <summary>
		/// Returns Guid for ActionField Pivot
		///  </summary>
		public static readonly Guid ActionFieldGuid = new Guid(ActionFieldString);

		/// <summary>
		/// Returns Guid for TimeStamp Pivot
		/// </summary>
		public static readonly Guid TimeStampFieldGuid = new Guid(TimeStampFieldString);

		/// <summary>
		/// Returns Guid for UserName Pivot
		/// </summary>
		public static readonly Guid UserNameFieldGuid = new Guid(UserNameFieldString);

		/// <summary>
		/// Returns Guid for ObjectField Pivot
		/// </summary>
		public static readonly Guid ObjectFieldGuid = new Guid(ObjectFieldString);
	}
}
