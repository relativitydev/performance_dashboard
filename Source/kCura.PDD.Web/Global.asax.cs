using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Diagnostics;
using kCura.PDD.Web.Handlers;
using kCura.PDD.Web.Services;
using Relativity.CustomPages;

namespace kCura.PDD.Web
{
	using System.Configuration;
	using System.Security.Claims;
	using System.Web.Helpers;
	using System.Web.Mvc;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Data;
	using kCura.PDD.Web.Factories;

	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			AssemblyHelper.InitResolves();
			Application["ApplicationId"] = Guid.NewGuid().ToString();

			RouteTable.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = System.Web.Http.RouteParameter.Optional }
				).RouteHandler = new SessionRouteHandler();

			AreaRegistration.RegisterAllAreas();

			DataSetup.Setup();

			//https://stackoverflow.com/a/20000098
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			Response.ClearHeaders();
			Response.AppendHeader("Cache-Control", "no-store");
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_PostAcquireRequestState(object sender, EventArgs e)
		{
			if (HttpContext.Current.Request.CurrentExecutionFilePathExtension == ".aspx" ||
				HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/mvc/"))
			{
				var authSrv = new AuthenticationServiceFactory().GetService();
				if (authSrv.HasPermissionsToPdbTab() == false)
				{
					authSrv.RedirectPermissionDenied();
				}
			}
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var ex = Server.GetLastError().GetBaseException();
			var errorHandlingSerivce = new ErrorHandlingService();
			errorHandlingSerivce.LogToErrorTable(ex, Request.RawUrl);
			errorHandlingSerivce.LogToErrorLog(ex, Request.RawUrl);
			errorHandlingSerivce.LogToEventViewer(ex, Request.Url.AbsoluteUri, Request.QueryString.ToString());
		}



		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}


	}
}