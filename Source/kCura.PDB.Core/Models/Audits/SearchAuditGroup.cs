namespace kCura.PDB.Core.Models.Audits
{
	using System.Collections.Generic;
	using System.Linq;

	public class SearchAuditGroup
	{
		public IList<SearchAudit> Audits { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the audit search group was complex
		/// </summary>
		public bool IsComplex
		{
			get { return this.Audits.All(a => a.IsComplex); }
		}

		public long ExecutionTime
		{
			get { return this.Audits.Sum(a => a.Audit.ExecutionTime ?? 0); }
		}

		public int UserId => this.Audits.First().Audit.UserID;

		/// <summary>
		/// Gets the QueryId
		/// </summary>
		public string QueryId => this.Audits.First().QueryId;

		public int SearchArtifactId => this.Audits.First().Audit.ArtifactID;

		public int WorkspaceId => this.Audits.First().Audit.WorkspaceId;
	}
}
