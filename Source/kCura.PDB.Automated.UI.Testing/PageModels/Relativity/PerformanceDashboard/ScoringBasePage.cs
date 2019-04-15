using System.IO;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public abstract class ScoringBasePage : BasePage
	{
		public ScoringBasePage(RemoteWebDriver driver) : base(driver)
		{
			driver.SwitchToFrame("Viewer_Splitter_Viewer_ContentFrame");
		}
		public IWebElement Chart => driver.FindElementById("scoreChart");
		public IWebElement QuaterlyScore => driver.FindElementById("BestInServiceScoreContainer");
		public IWebElement ArrowNext => driver.FindElementByCssSelector("#varscat-table_next");
		public IWebElement TableInfo => driver.FindElementById("varscat-table_info");
		public IWebElement ExportListButton => driver.FindElementByCssSelector("#varscat-table_wrapper > div.table-controls > div.export-excel");
		public abstract bool VerifyExpectedColor(string color);
		public abstract bool VerifyExpectedNumberValue(int number);
		public abstract bool VerifyListIsSortable();
		public abstract bool VerifyListIsFiltarable(string searchData);
		public bool VerifyListIsPagable()
		{
			var tableFirstPage = TableInfo.Text;
			ArrowNext.Click();
			var tableSecondPage = TableInfo.Text;
			return tableFirstPage != tableSecondPage;
		}

		public void VerifyListIsExportable(string fileName, string filePath)
		{
			ExportListButton.Click();
			driver.Wait(2);
			var directory = new DirectoryInfo(filePath);
			var files = directory.GetFiles($"{fileName}*");
			Assert.That(files.Length, Is.EqualTo(1));
			files[0].Delete();
		}

		public bool VerifyQuaterlyScoreIsShownInTheTopLeft()
		{
			return !string.IsNullOrEmpty(QuaterlyScore.Text);
		}
	}
}