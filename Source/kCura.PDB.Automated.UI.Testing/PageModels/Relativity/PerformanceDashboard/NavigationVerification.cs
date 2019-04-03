using kCura.PDB.Automated.UI.Testing.Constants;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using Milyli.UIAutomation.Relativity.Utils;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public static class NavigationVerification
	{
		public static bool VerifyPageNavigation(string scoringPage, BasePage basePage)
		{
			return basePage.Title.Text.Contains(scoringPage);
		}
	}
}