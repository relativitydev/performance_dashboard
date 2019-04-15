namespace kCura.PDB.Core.Models.Audits
{
	using System.Collections.Generic;

	public class Search
	{
		/// <summary>
		/// Gets or sets ArtifactId of the Search
		/// </summary>
		public int ArtifactId { get; set; }

		public int WorkspaceId { get; set; }

		/// <summary>
		/// Gets or sets criterias obtained from ViewCriteria table (SQL)
		/// </summary>
		public IList<ViewCriteria> Criterias { get; set; }

		/// <summary>
		/// Gets or sets SearchText obtained from View table (SQL), unparsed XML
		/// </summary>
		public string SearchText { get; set; }

		/// <summary>
		/// Gets or sets the name of the search if it's a save search
		/// </summary>
		public string Name { get; set; }

		public bool IsAdhoc => this.Name == null;
	}
}
