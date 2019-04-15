namespace kCura.PDD.Web.Areas.MVC.Controllers
{
	using kCura.PDD.Web.Filters;
	using System.Web.Mvc;

	[AuthenticateUser,
		PromptWhenAdminScriptsNotDeployed]
	public class MaintenanceWindowSchedulerController : Controller
	{
		// GET: MVC/MaintenanceWindowScheduler
		public ActionResult Index()
		{
			return View();
		}
	}
}