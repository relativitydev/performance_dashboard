using System;
using kCura.PDB.Automated.UI.Testing.Constants;
using OpenQA.Selenium;
using Milyli.UIAutomation.Relativity.Utils;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class BackfillConsolePage : BasePage
	{
		public BackfillConsolePage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
		}
		//Placeholder Class for when we start checking Backfill Console Page.
		public IWebElement RetryErroredEventsButton => driver.FindElementById("btnRetryErroredEvents");

		public IWebElement ErroredEventsCount => driver.FindElementById("numberOfErrorEvents");

		public bool VerifyRetryErroredEventsButtonIsNotPresent()
		{
			{
				try
				{
					var retryErroredEventsButtonElement = RetryErroredEventsButton;
					return (retryErroredEventsButtonElement == null);
				}
				catch (NoSuchElementException)
				{
					return true;
				}
			}
		}
	}
}