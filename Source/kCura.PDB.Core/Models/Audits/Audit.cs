namespace kCura.PDB.Core.Models.Audits
{
	using System;
	using kCura.PDB.Core.Enumerations;

	public class Audit
	{
		/// <summary>
		/// Gets or sets ID from the AuditRecord Table
		/// </summary>
		public int AuditID { get; set; }

		/// <summary>
		/// Gets or sets User ID who executed the search
		/// </summary>
		public int UserID { get; set; }

		/// <summary>
		/// Gets or sets ArtifactID of the AuditRecord
		/// </summary>
		public int ArtifactID { get; set; }

		/// <summary>
		/// Gets or sets XML Doc (Only used in Search Audits)
		/// </summary>
		public string Details { get; set; }

		/// <summary>
		/// Gets or sets parsed text from QueryText element from XML Doc
		/// </summary>
		public string ParsedDetails { get; set; }

		/// <summary>
		/// Gets or sets when the search was executed
		/// </summary>
		public DateTime TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets Audit Action Type ID
		/// </summary>
		public AuditActionId Action { get; set; }

		/// <summary>
		/// Gets or sets how long the search took to execute
		/// </summary>
		public long? ExecutionTime { get; set; }

		/// <summary>
		/// Gets or sets string used to filter adhoc queries (NOT LIKE/LIKE '%relativitywebapi%')
		/// </summary>
		public string RequestOrigination { get; set; }

		public int WorkspaceId { get; set; }
	}
}
