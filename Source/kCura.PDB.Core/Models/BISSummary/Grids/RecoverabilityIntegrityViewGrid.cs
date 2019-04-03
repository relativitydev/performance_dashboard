namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class RecoverabilityIntegrityViewGrid
	{
        public RecoverabilityIntegrityViewGrid()
		{
            Data = new List<RecoverabilityIntegrityScoreInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<RecoverabilityIntegrityScoreInfo> Data;
	}
}
