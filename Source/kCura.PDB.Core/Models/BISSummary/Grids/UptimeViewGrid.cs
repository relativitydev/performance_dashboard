namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class UptimeViewGrid
	{
        public UptimeViewGrid()
		{
            Data = new List<UptimeReportHourInfo>().AsQueryable();
		}

		public int Count;
        public IQueryable<UptimeReportHourInfo> Data;
	}
}
