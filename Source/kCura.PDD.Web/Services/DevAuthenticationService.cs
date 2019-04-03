namespace kCura.PDD.Web.Services
{
    using System;

    public class DevAuthenticationService : IAuthenticationService
    {
		public bool IsSystemAdministrator()
		{
			return true;
		}

		public int GetUserId()
        {
            return 9;
        }

        public bool HasPermissionsToPdbTab()
        {
            return true;
        }

        public void RedirectPermissionDenied()
        {
            throw new NotImplementedException();
        }
    }
}