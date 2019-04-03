namespace kCura.PDD.Web.Areas.MVC.Controllers
{
	using kCura.PDD.Web.Filters;
	using System.Web.Mvc;

	[AuthenticateUser,
		PromptWhenAdminScriptsNotDeployed]
	public class ServiceQualityController : Controller
	{
		// GET: MVC/ServiceQuality
		public ActionResult Index()
		{
			return View();
		}
	}
}