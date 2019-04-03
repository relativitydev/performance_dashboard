using kCura.PDB.Automated.UI.Testing.Constants;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using Milyli.UIAutomation.Relativity.Utils;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class UptimePage : BasePage
	{
		public UptimePage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
		}
	}
}