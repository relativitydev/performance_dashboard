namespace kCura.PDD.Web.Services
{
	using System;
	using System.Data.SqlClient;
	using System.Web;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Constants;

	public class AuthenticationService : IAuthenticationService
	{
		public int GetUserId()
		{
			var result = -1;

			try
			{
				var connectionHelper = ConnectionHelper.Helper();
				var authenticationManager = connectionHelper.GetAuthenticationManager();
				result = authenticationManager.UserInfo.ArtifactID;
			}
			catch
			{
				// The user may not be logged in or...
				//		... older versions of Relativity may not implement the necessary web method.
				// Eat exceptions and fall back on session stored EDDSIdentity.
			}

			if (result == -1)
			{
				try
				{
					var userIdString = System.Web.HttpContext.Current.Session["UserID"];
					if (userIdString != null)
						result = Int32.Parse(userIdString.ToString());
				}
				catch
				{
					// Again, the user may not be logged in.
					// Unlikely, but eat the exception anyway.
					// The security attribute will force unauthenticated and unauthorized requests to the access denied page.
				}
			}

			return result;
		}

		public bool HasPermissionsToPdbTab()
		{
			try
			{
				var userId = GetUserId();
				var connectionHelper = ConnectionHelper.Helper();

				var context = connectionHelper.GetDBContext(-1);
				var resultCount = context.ExecuteSqlStatementAsScalar<int>(@"
					SELECT COUNT(*) 
					FROM eddsdbo.[Tab] AS T 
					JOIN eddsdbo.GroupTab AS GT ON T.ArtifactID = GT.TabArtifactID 
					JOIN eddsdbo.[GroupUser] AS GU ON GT.GroupArtifactID = GU.GroupArtifactID 
					WHERE T.Name = @tabName AND GU.UserArtifactID = @userId
				", new SqlParameter[] { new SqlParameter("@tabName", Names.Tab.PerformanceDashboard), new SqlParameter("@userId", userId) });
                context.ReleaseConnection();
				if (resultCount >= 1)
					return true;
				else
					return false;
			}
			catch
			{
				//if the relativity doesn't have admin permissions this will fail 
				//and we can fail back on checking the workspace ID is in the session since
				//instances that aren't upgraded to 9.3 will still have the id in the session
			}


			if (System.Web.HttpContext.Current.Session["WorkspaceID"] != null && (int)System.Web.HttpContext.Current.Session["WorkspaceID"] == -1)
				return true;
			else
				return false;

		}

		public bool IsSystemAdministrator()
		{
			var userId = GetUserId();
			var connectionHelper = ConnectionHelper.Helper();

			var context = connectionHelper.GetDBContext(-1);
			var result = context.ExecuteSqlStatementAsScalar<bool>(@"
				SELECT CASE WHEN EXISTS (
					SELECT * FROM [eddsdbo].[GroupUser]
					WHERE [GroupArtifactID] = 20 AND [UserArtifactID] = @userId
				) THEN 1 ELSE 0 END
				",
				new SqlParameter("@userId", userId) );
			context.ReleaseConnection();
			return result;
		}

		public void RedirectPermissionDenied()
		{
			var context = HttpContext.Current;
			var redirectHtml = "<html><head><meta http-equiv='refresh' content='0;url={0}://{1}/Relativity/Error/PermissionDenied.htm'></head></html>";
			var permissionDeniedPage = string.Format(redirectHtml, context.Request.Url.Scheme, context.Request.Url.Authority);
			context.Response.Write(permissionDeniedPage);
			context.Response.Flush();
			context.Response.End();

		}
	}
}