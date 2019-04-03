namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class WaitsViewGrid
	{
        public WaitsViewGrid()
		{
            Data = new List<SystemLoadWaitsInfo>().AsQueryable();
		}

		public int Count;
        public IQueryable<SystemLoadWaitsInfo> Data;
	}
}