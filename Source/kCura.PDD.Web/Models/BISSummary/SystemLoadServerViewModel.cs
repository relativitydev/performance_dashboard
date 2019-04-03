using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class SystemLoadServerViewModel
	{
		public SystemLoadServerViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new LoadViewFilterConditions();
			FilterOperands = new LoadViewFilterOperands();
		}

		public GridConditions GridConditions;
		public LoadViewFilterConditions FilterConditions;
		public LoadViewFilterOperands FilterOperands;
	}
}