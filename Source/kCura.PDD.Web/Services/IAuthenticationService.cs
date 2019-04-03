using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Services
{
	public interface IAuthenticationService
	{
		
		int GetUserId();
		bool HasPermissionsToPdbTab();
		bool IsSystemAdministrator();
		void RedirectPermissionDenied();
	}
}