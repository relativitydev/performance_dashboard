using kCura.PDD.Web.Factories;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;

namespace kCura.PDD.Web.Filters
{
	public class AuthenticateSystemAdministratorAttribute : System.Web.Http.AuthorizeAttribute
	{
		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			var authSrv = new AuthenticationServiceFactory().GetService();
			return authSrv.IsSystemAdministrator();
		}
	}
}
