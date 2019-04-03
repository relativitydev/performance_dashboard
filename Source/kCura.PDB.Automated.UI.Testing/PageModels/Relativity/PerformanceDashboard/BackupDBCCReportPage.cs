using kCura.PDB.Automated.UI.Testing.Constants;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class BackupDBCCReportPage : ScoringBasePage
	{
		public IWebElement SearchGapSizeTextField => driver.FindElementByCssSelector("#varscat-table > thead > tr.searchRow > th:nth-child(6) > input");
		public IWebElement GapSizeColumnValue => driver.FindElementByCssSelector("#varscat-table > tbody > tr:nth-child(1) > td:nth-child(6) > b");
		public IWebElement GapResolutionDateColumn => driver.FindElementByCssSelector("#varscat-table > thead > tr:nth-child(1) > th:nth-child(5)");
		public IWebElement GapResolutionDateColumnValue => driver.FindElementByCssSelector("#varscat-table > tbody > tr:nth-child(1) > td:nth-child(5)");
		public BackupDBCCReportPage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame(PageConstants.InnerFrame);
		}

		public override bool VerifyExpectedColor(string colorText)
		{
			var classValue = GapSizeColumnValue.GetAttribute("class");
			return classValue.Contains(colorText);
		}

		public override bool VerifyExpectedNumberValue(int gapSize)
		{
			var gapSizeValue = GapSizeColumnValue.Text;
			int actualValue;

			if (int.TryParse(gapSizeValue, out actualValue))
			{
				return gapSize == actualValue;
			}
			return false;
		}

		public bool VerifyGapResolutionDate(string gapResolutionDate)
		{
			var gapResolutionDateValue = GapResolutionDateColumnValue.Text;
			return gapResolutionDateValue.Contains(gapResolutionDate);
		}

		public bool VerifyBackupDBCCDataShowsUpOnThePage()
		{
			var backupDBCCTableData = GapSizeColumnValue.Text;
			return !string.IsNullOrEmpty(backupDBCCTableData);
		}

		public override bool VerifyListIsSortable()
		{
			var className = GapResolutionDateColumn.GetAttribute("class");
			Assert.That(className, Is.EqualTo("sorting"));
			GapResolutionDateColumn.Click();
			driver.Wait(1);
			var sortedClassName = GapResolutionDateColumn.GetAttribute("class");
			return sortedClassName == "sorting_asc";
		}

		public override bool VerifyListIsFiltarable(string searchData)
		{
			var className = GapSizeColumnValue.Text;
			Assert.That(className.Contains(searchData), Is.False);
			SearchGapSizeTextField.SendKeys(searchData);
			SearchGapSizeTextField.SendKeys(Keys.Enter);
			driver.Wait(1);
			var filteredColumnDataValue = GapSizeColumnValue.Text;
			return filteredColumnDataValue == searchData;
		}
	}
}