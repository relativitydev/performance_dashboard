using kCura.PDD.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Hangfire.Dashboard;

namespace kCura.PDD.Web.Filters
{
	using kCura.PDD.Web.Factories;

	public class AuthenticateUserAttribute : AuthorizeAttribute, Hangfire.Dashboard.IDashboardAuthorizationFilter
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var authSrv = new AuthenticationServiceFactory().GetService();
			if (authSrv.HasPermissionsToPdbTab() == false)
			{
				authSrv.RedirectPermissionDenied();
			}
		}

		public bool Authorize(DashboardContext context)
		{
			var enableHangfireDashboard = System.Configuration.ConfigurationManager.AppSettings["enablehangfiredashboard"];
			if (enableHangfireDashboard == null) return false;
			if (Convert.ToBoolean(enableHangfireDashboard) == true) return true;
			return false;
		}
	}
}