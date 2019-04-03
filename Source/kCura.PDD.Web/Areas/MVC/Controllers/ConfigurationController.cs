namespace kCura.PDD.Web.Areas.MVC.Controllers
{
	using kCura.PDD.Web.Filters;
	using System.Web.Mvc;

	[AuthenticateUser,
		PromptWhenAdminScriptsNotDeployed]
	public class ConfigurationController : Controller
	{
		// GET: MVC/Configuration
		public ActionResult Index()
		{
			return View();
		}
	}
}