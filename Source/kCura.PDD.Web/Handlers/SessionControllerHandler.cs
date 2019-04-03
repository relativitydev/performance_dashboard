using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.SessionState;
using System.Web.Routing;

namespace kCura.PDD.Web.Handlers
{
	public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
	{
		public SessionControllerHandler(RouteData routeData)
			: base(routeData)
		{
		}
	}
}