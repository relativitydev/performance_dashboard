using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class UptimeReportViewModel
	{
		public UptimeReportViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new UptimeViewFilterConditions();
			FilterOperands = new UptimeViewFilterOperands();
		}

		public GridConditions GridConditions;
		public UptimeViewFilterConditions FilterConditions;
		public UptimeViewFilterOperands FilterOperands;
	}
}