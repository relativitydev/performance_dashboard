using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class UserExperienceHoursViewModel
	{
		public UserExperienceHoursViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new HoursViewFilterConditions();
			FilterOperands = new HoursViewFilterOperands();
		}

		public GridConditions GridConditions;
		public HoursViewFilterConditions FilterConditions;
		public HoursViewFilterOperands FilterOperands;
	}
}