using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class UserExperienceServerViewModel
	{
		public UserExperienceServerViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = new ServerViewFilterConditions();
			FilterOperands = new ServerViewFilterOperands();
		}

		public GridConditions GridConditions;
		public ServerViewFilterConditions FilterConditions;
		public ServerViewFilterOperands FilterOperands;
	}
}