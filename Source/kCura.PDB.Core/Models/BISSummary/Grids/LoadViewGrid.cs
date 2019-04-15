namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class LoadViewGrid
	{
		public LoadViewGrid()
		{
			Data = new List<SystemLoadServerHourInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<SystemLoadServerHourInfo> Data;
	}
}