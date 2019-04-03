namespace kCura.PDB.Core.Models.Audits
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Enumerations;

	public class AuditQuery
	{
		public int WorkspaceId { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public IList<AuditActionId> ActionTypes { get; set; }
	}
}
