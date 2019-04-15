using kCura.PDB.Automated.UI.Testing.Constants;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class RecoverabilityIntegrityPage : ScoringBasePage
	{
		public IWebElement ReInstallScriptsSubTab => driver.FindElementByCssSelector("body > div.action-bar > div.right > a:nth-child(5)");
		public IWebElement BackupDBCCReportSubTab => driver.FindElementByCssSelector("body > div.action-bar > div.right > a:nth-child(3)");
		public IWebElement HourColumn => driver.FindElementByCssSelector("#varscat-table > thead > tr:nth-child(1) > th");
		public IWebElement SearchOverallScoreTextField => Configuration.Driver.FindElementByCssSelector("#varscat-table > thead > tr.searchRow > th:nth-child(2) > input");
		public IWebElement OveralScoreValue => driver.FindElementByCssSelector("#varscat-table > tbody > tr:nth-child(1) > td:nth-child(2) > span");

		public RecoverabilityIntegrityPage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
		}

		public IWebElement RecoveryObjectivesReportSubTab => driver.FindElementByCssSelector("body > div.action-bar > div.right > a:nth-child(4)");
		public override bool VerifyExpectedColor(string colorText)
		{
			var classValue = OveralScoreValue.GetAttribute("class");
			return classValue.Contains(colorText);
		}

		public override bool VerifyExpectedNumberValue(int scoreNumber)
		{
			var scoreNumberValue = OveralScoreValue.Text;
			int actualValue;

			if (int.TryParse(scoreNumberValue, out actualValue))
			{
				return scoreNumber == actualValue;
			}
			return false;
		}
		public bool VerifyRIScoreShowsUpOnThePage()
		{
			var tableScoreValue = OveralScoreValue.Text;
			return !string.IsNullOrEmpty(tableScoreValue);
		}

		public override bool VerifyListIsSortable()
		{
			var className = HourColumn.GetAttribute("class");
			Assert.That(className, Is.EqualTo("sorting"));
			HourColumn.Click();
			driver.Wait(1);
			var sortedClassName = HourColumn.GetAttribute("class");
			return sortedClassName == "sorting_asc";
		}

		public override bool VerifyListIsFiltarable(string searchData)
		{
			var className = OveralScoreValue.Text;
			Assert.That(className.Contains(searchData), Is.False);
			SearchOverallScoreTextField.SendKeys(searchData);
			SearchOverallScoreTextField.SendKeys(Keys.Enter);
			driver.Wait(1);
			var filteredColumnDataValue = OveralScoreValue.Text;
			return filteredColumnDataValue == searchData;
		}
	}
}