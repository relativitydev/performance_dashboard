using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;

namespace kCura.PDD.Web.Filters
{
	public class ValidateAjaxAntiForgeryTokenAttribute : System.Web.Http.AuthorizeAttribute
	{
		// c.f. http://stephenwalther.com/archive/2013/03/05/security-issues-with-single-page-apps
		protected override bool IsAuthorized(HttpActionContext actionContext)
		{

			var headerToken = actionContext
				.Request
				.Headers
				.GetValues("X-CSRF-Header")
				.FirstOrDefault();

			var cookieToken = actionContext
				.Request
				.Headers
				.GetCookies()
				.Select(c => c[AntiForgeryConfig.CookieName])
				.FirstOrDefault();

			// check for missing cookie or header
			if (cookieToken == null || headerToken == null)
			{
				return false;
			}

			// ensure that the cookie matches the header
			try
			{
				AntiForgery.Validate(cookieToken.Value, headerToken);
			}
			catch
			{
				return false;
			}

			return base.IsAuthorized(actionContext);
		}
	}
}
