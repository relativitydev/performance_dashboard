using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class UserExperienceSearchViewModel
	{
		public UserExperienceSearchViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new SearchViewFilterConditions();
			FilterOperands = new SearchViewFilterOperands();
		}

		public GridConditions GridConditions;
		public SearchViewFilterConditions FilterConditions;
		public SearchViewFilterOperands FilterOperands;
	}
}