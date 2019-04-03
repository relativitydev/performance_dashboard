namespace kCura.PDB.Core.Interfaces.CategoryScoring
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface ICategoryScoringTask
	{
		Task<int> ScoreCategory(int categoryScoreId);

		Task<IList<int>> FindNextCategoriesToScore();
	}
}
