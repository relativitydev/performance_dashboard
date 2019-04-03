namespace kCura.PDD.Web.Filters
{
	using System.Web.Mvc;
	using global::Relativity.CustomPages;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;

	public class PromptWhenAdminScriptsNotDeployedAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//Redirect if elevated permissions are needed for script updates
			var sqlRepo = new SqlServerRepository(new HelperConnectionFactory(ConnectionHelper.Helper()));
			if (!sqlRepo.AdminScriptsInstalled())
				filterContext.Result = new RedirectResult("~/AdministrationInstall.aspx");
			else
				base.OnActionExecuting(filterContext);
		}
	}
}