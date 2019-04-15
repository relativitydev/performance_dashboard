using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class SystemLoadWaitsViewModel
	{
        public SystemLoadWaitsViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new WaitsViewFilterConditions();
			FilterOperands = new WaitsViewFilterOperands();
		}

		public GridConditions GridConditions;
        public WaitsViewFilterConditions FilterConditions;
        public WaitsViewFilterOperands FilterOperands;
	}
}