using OpenQA.Selenium;
using System;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using kCura.PDB.Automated.UI.Testing.Constants;
using OpenQA.Selenium.Remote;

namespace kCura.PageModels.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class QualityOfServicePage : BasePage
	{
		public QualityOfServicePage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
			driver.SwitchToFrame("Viewer_Splitter_Viewer_ContentFrame");
		}
		public IWebElement RIQuaterlyScoreSummaryPeport => driver.FindElementById("qRecoveryChart");
		public IWebElement RIWeeklyScoreSummaryPeport => driver.FindElementById("wRecoveryChart");

		public IWebElement QosReportTable => driver.FindElementByClassName("row");

		public void VerifyQoSPageNavigation()
		{
			var qosPageCard = driver.FindElementById("mainReport").Displayed;
			Assert.That(qosPageCard, Is.True);
		}

		public bool VerifyExpectedScoreNumberValue(int table, int cssTrSelector, int cssTdSelector)
		{
			var scoreNumber = driver.FindElementByCssSelector($"#mainReport > div:nth-child(2) > div:nth-child(3) > div:nth-child({table}) > div > table > tbody > tr:nth-child({cssTrSelector}) > td:nth-child({cssTdSelector})").Text;
			int actualValue;
			return int.TryParse(scoreNumber, out actualValue);
		}

		public bool VerifyExpectedTimeValue(int cssTrSelector)
		{
			var timeValue = driver.FindElementByCssSelector($"#mainReport > div:nth-child(2) > div:nth-child(3) > div:nth-child(3) > div > table > tbody > tr:nth-child({cssTrSelector}) > td:nth-child(2)").Text;
			return timeValue != null;
		}
	}
}