namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class BackupDbccViewGrid
	{
		public BackupDbccViewGrid()
		{
			Data = new List<BackupDbccGapInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<BackupDbccGapInfo> Data;
	}
}
