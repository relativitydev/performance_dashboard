using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	public class EnvironmentCheckViewModel
	{
		public EnvironmentCheckViewModel()
		{
			GridConditions = new GridConditions();

			RecommendationFilterConditions = new EnvironmentCheckRecommendationFilterConditions();

			ServerFilterConditions = new EnvironmentCheckServerFilterConditions();
			ServerFilterOperands = new EnvironmentCheckServerFilterOperands();

			DatabaseFilterConditions = new EnvironmentCheckDatabaseFilterConditions();
			DatabaseFilterOperands = new EnvironmentCheckDatabaseFilterOperands();
		}

		public GridConditions GridConditions;
		public EnvironmentCheckRecommendationFilterConditions RecommendationFilterConditions;

		public EnvironmentCheckServerFilterConditions ServerFilterConditions;
		public EnvironmentCheckServerFilterOperands ServerFilterOperands;

		public EnvironmentCheckDatabaseFilterConditions DatabaseFilterConditions;
		public EnvironmentCheckDatabaseFilterOperands DatabaseFilterOperands;

		public EnvironmentCheckReportType ReportType;
	}
	public enum EnvironmentCheckReportType
	{
		Recommendations = 1,
		Server = 3,
		Database = 4,
	}
}