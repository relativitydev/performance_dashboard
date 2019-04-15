namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class RecoveryObjectivesViewGrid
	{
        public RecoveryObjectivesViewGrid()
		{
            Data = new List<RecoveryObjectivesInfo>().AsQueryable();
		}

		public int Count;
        public IQueryable<RecoveryObjectivesInfo> Data;
	}
}
