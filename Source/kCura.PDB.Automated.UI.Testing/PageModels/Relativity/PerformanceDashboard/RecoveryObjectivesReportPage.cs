using OpenQA.Selenium.Remote;
using kCura.PDB.Automated.UI.Testing.Constants;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using OpenQA.Selenium;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class RecoveryObjectivesReportPage : ScoringBasePage
	{
		public IWebElement MaxDataLossColumnValue => driver.FindElementByCssSelector("#varscat-table > tbody > tr:nth-child(1) > td:nth-child(5)");
		public IWebElement RTOScoreColumn => driver.FindElementByCssSelector("#varscat-table > thead > tr:nth-child(1) > th:nth-child(4)");
		public IWebElement SearchRTOScoreTextField => driver.FindElementByCssSelector("#varscat-table > thead > tr.searchRow > th:nth-child(4) > input");
		public IWebElement RTOScoreColumnValue => driver.FindElementByCssSelector("#varscat-table > tbody > tr:nth-child(1) > td:nth-child(4) > span");

		public RecoveryObjectivesReportPage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
		}

		public override bool VerifyExpectedColor(string colorText)
		{
			var classValue = RTOScoreColumnValue.GetAttribute("class");
			return classValue.Contains(colorText);
		}

		public override bool VerifyExpectedNumberValue(int scoreNumber)
		{
			var scoreNumberValue = RTOScoreColumnValue.Text;
			int actualValue;

			if (int.TryParse(scoreNumberValue, out actualValue))
			{
				return scoreNumber == actualValue;
			}
			return false;
		}

		public bool VerifyRecoveryObjectivesReportDataShowsUpOnThePage()
		{
			var recoveryObjectivesReportTableData = MaxDataLossColumnValue.Text;
			return !string.IsNullOrEmpty(recoveryObjectivesReportTableData);
		}

		public override bool VerifyListIsSortable()
		{
			var className = RTOScoreColumn.GetAttribute("class");
			Assert.That(className, Is.EqualTo("sorting"));
			RTOScoreColumn.Click();
			driver.Wait(1);
			var sortedClassName = RTOScoreColumn.GetAttribute("class");
			return sortedClassName == "sorting_asc";
		}
		public override bool VerifyListIsFiltarable(string searchData)
		{
			var className = RTOScoreColumnValue.Text;
			Assert.That(className.Contains(searchData), Is.False);
			SearchRTOScoreTextField.SendKeys(searchData);
			SearchRTOScoreTextField.SendKeys(Keys.Enter);
			driver.Wait(1);
			var filteredColumnDataValue = RTOScoreColumnValue.Text;
			return filteredColumnDataValue == searchData;
		}
	}
}