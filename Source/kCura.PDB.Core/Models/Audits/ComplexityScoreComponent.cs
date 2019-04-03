namespace kCura.PDB.Core.Models.Audits
{
	public class ComplexityScoreComponent
	{
		public int TotalLikes { get; set; }

		public int TotalValueWords { get; set; }

		public int TotalCharacters { get; set; }

		public int TotalDTSearchCharacters { get; set; }

		public int TotalSubsearches { get; set; }

		public int TotalNonLikeOperators { get; set; } // Excludes 'like', 'in', 'contains'

		public int TotalSearchFolders { get; set; }

		public bool HasDTSearch { get; set; }

		public bool HasFullTextSearch { get; set; }

		public bool HasInOrContainsOperator { get; set; }

		public int TotalScore => (this.TotalLikes * 10) + this.TotalValueWords + this.TotalCharacters + this.TotalDTSearchCharacters + (this.HasDTSearch ? 1 : 0) + (this.HasFullTextSearch ? 1 : 0) + (this.HasInOrContainsOperator ? 1 : 0) + (this.TotalSearchFolders * this.TotalNonLikeOperators);

		public bool IsComplexSearch => this.TotalScore > 9;
	}
}
