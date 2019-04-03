using System.Collections.Generic;
using Milyli.UIAutomation.Relativity.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.StepDefinitions
{
	public class BasePage
	{
		protected RemoteWebDriver driver;
		public BasePage(RemoteWebDriver driver)
		{
			this.driver = driver;
			driver.SwitchToFrame(Constants.PageConstants.InnerFrame);
		}
		public IWebElement Title => driver.FindElementById("ReportHeader");

		public Dictionary<string, string> scoreColor = new Dictionary<string, string>(){
			{ "red", "failText" },
			{ "yellow", "warnText" },
			{ "green", "passText" }
		};

		public Dictionary<string, string> gapSizeColor = new Dictionary<string, string>(){
			{ "red", "grid-item-critical" },
			{ "yellow", "grid-item-warning" },
			{ "green", "grid-item-healthy" }
		};
	}
}