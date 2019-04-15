namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class HoursViewGrid
	{
		public HoursViewGrid()
		{
			Data = new List<UserExperienceWorkspaceHourInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<UserExperienceWorkspaceHourInfo> Data;
	}
}
