using kCura.PDD.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace kCura.PDD.Web.Controllers
{
	using kCura.PDB.Core.Models.BISSummary.Grids;

	[AuthenticateUserAttribute]
	public class BaseApiController : ApiController
	{
		protected HttpSessionState GetSession()
		{
			return HttpContext.Current.Session;
		}

		protected GridConditions PopulateCommonGridConditions(List<KeyValuePair<string, string>> queryParams)
		{
			var conditions = new GridConditions();

			var sEcho = queryParams.FirstOrDefault(k => k.Key == "sEcho");
			var iDisplayStart = queryParams.FirstOrDefault(k => k.Key == "iDisplayStart");
			var iDisplayLength = queryParams.FirstOrDefault(k => k.Key == "iDisplayLength");
			var timezoneOffset = queryParams.FirstOrDefault(k => k.Key == "TimezoneOffset");
			var startDate = queryParams.FirstOrDefault(x => x.Key == "StartDate");
			var endDate = queryParams.FirstOrDefault(x => x.Key == "EndDate");

			conditions.sEcho = sEcho.Value;
			conditions.StartRow = string.IsNullOrEmpty(iDisplayStart.Value)
				? 1
				: int.Parse(iDisplayStart.Value) + 1;
			conditions.EndRow = string.IsNullOrEmpty(iDisplayLength.Value)
				? 25
				: int.Parse(iDisplayLength.Value) + conditions.StartRow - 1;
			conditions.TimezoneOffset = string.IsNullOrEmpty(timezoneOffset.Value)
				? 0
				: int.Parse(timezoneOffset.Value);

			//Page-level date filters
			DateTime sd, ed;
			if (!DateTime.TryParse(startDate.Value, out sd))
				conditions.StartDate = DateTime.UtcNow.AddDays(-90).AddMinutes(conditions.TimezoneOffset);
			else
				conditions.StartDate = sd.AddMinutes(-1 * conditions.TimezoneOffset); //midnight local time this day, converted to UTC

			if (!DateTime.TryParse(endDate.Value, out ed))
				conditions.EndDate = DateTime.UtcNow.AddMinutes(conditions.TimezoneOffset);
			else
				conditions.EndDate = ed.AddDays(1).AddMinutes(-1 * conditions.TimezoneOffset - 1); //23:59 local time the following day, converted to UTC

			return conditions;
		}
	}
}