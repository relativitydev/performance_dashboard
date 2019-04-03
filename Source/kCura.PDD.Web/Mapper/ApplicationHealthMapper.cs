using System;
using System.Collections.Generic;
using System.Linq;

namespace kCura.PDD.Web.Mapper
{
	using kCura.PDB.Core.Models.HealthChecks;

	public static class ApplicationHealthMapper
	{
		public static List<ApplicationHealth2> ToApplicationHealth(IList<HealthBase> healthData)
		{
			const double defaultUserCount = 0;
			List<ApplicationHealth2> applicationHealthSource =
				(from item in healthData
				 let appHealth = item as ApplicationHealth
				 group appHealth by appHealth.CaseArtifactId into g
				 select new ApplicationHealth2()
				 {
					 Id = g.Select(a => a.CaseArtifactId).FirstOrDefault(),
					 ArtifactID = g.Select(a => a.CaseArtifactId).FirstOrDefault(),
					 Errors = g.Sum(a => a.ErrorCount < 0 ? 0 : a.ErrorCount),
					 LRQ = g.Sum(a => a.LRQCount < 0 ? 0 : a.LRQCount),
					 Users = (g.Where(a => a.UserCount > 0).Any()
											 ? Convert.ToDouble(Math.Ceiling((decimal)
													 g.GroupBy(a => a.MeasureDate.Date)          // Grouping by the date,
														.Average(a =>                              // get the average of the
															 a.Max(b => Math.Max(b.UserCount, 0))))) // peak user count (treating negatives as 0)
										 : defaultUserCount
					 ),
					 WorkspaceName = g.Select(a => a.WorkspaceName).FirstOrDefault(),
					 SQLInstanceName = g.Select(a => a.DatabaseLocation).FirstOrDefault(),
					 MeasureDate = g.Select(a => a.MeasureDate.Date).FirstOrDefault(),
				 }).ToList();

			return applicationHealthSource;
		}
	}
}