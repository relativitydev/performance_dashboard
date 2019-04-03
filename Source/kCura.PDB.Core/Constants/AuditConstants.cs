namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;

	public class AuditConstants
	{
		public const string ConfigurationSection_Audit = "kCura.Audit";
		public const string ConfigurationKey_PostMigrationPersistencePeriod = "PostMigrationPersistencePeriod";

		public const int BatchSize = 5000;
		public const int LongRunningComplexThreshold = 8000;
		public const int LongRunningSimpleThreshold = 2000;

		public static readonly List<AuditActionId> RelevantAuditActionIdsOtherThanUpdate;
		public static readonly List<AuditActionId> RelevantAuditActionIds;
		public static readonly List<AuditActionId> Audits3456;
		public static readonly List<AuditActionId> UpdateAuditActionIds;

		/// <summary>
		/// Initializes static members of the <see cref="AuditConstants"/> class.
		/// Explicitly setting the order of the public static fields because of dependencies.
		/// </summary>
		static AuditConstants()
		{
			Audits3456 = new List<AuditActionId>
			{
				AuditActionId.Update,
				AuditActionId.UpdateMassEdit,
				AuditActionId.UpdateMassReplace,
				AuditActionId.UpdatePropagation
			};

			UpdateAuditActionIds = new List<AuditActionId>
			{
				AuditActionId.Update,
				AuditActionId.UpdateMassEdit,
				AuditActionId.UpdateMassReplace,
				AuditActionId.UpdatePropagation,
				AuditActionId.UpdateImport
			};

			RelevantAuditActionIds =
				Enum.GetValues(typeof(AuditActionId)).Cast<AuditActionId>().ToList();

			RelevantAuditActionIdsOtherThanUpdate =
				Enum.GetValues(typeof(AuditActionId)).Cast<AuditActionId>().Where(a => !UpdateAuditActionIds.Contains(a)).ToList();
		}
	}
}
