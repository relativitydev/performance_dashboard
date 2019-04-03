namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class SearchViewGrid
	{
		public SearchViewGrid()
		{
			Data = new List<UserExperienceSearchUserInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<UserExperienceSearchUserInfo> Data;
	}
}
