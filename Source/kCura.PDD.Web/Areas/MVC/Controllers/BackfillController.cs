namespace kCura.PDD.Web.Areas.MVC.Controllers
{
	using kCura.PDD.Web.Filters;
	using System.Web.Mvc;

	[AuthenticateUser,
		PromptWhenAdminScriptsNotDeployed]
	public class BackfillController : Controller
	{
		// GET: MVC/Backfill
		public ActionResult Index()
		{
			return View();
		}
	}
}
