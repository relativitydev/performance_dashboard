namespace kCura.PDD.Web.Areas.MVC.Controllers
{
	using kCura.PDD.Web.Filters;
	using System.Web.Mvc;

	[AuthenticateUser,
		PromptWhenAdminScriptsNotDeployed]
	public class NotificationsController : Controller
	{
		// GET: MVC/Notifications
		public ActionResult Index()
		{
			return View();
		}
	}
}