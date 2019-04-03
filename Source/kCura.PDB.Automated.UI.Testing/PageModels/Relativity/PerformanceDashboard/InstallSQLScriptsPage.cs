using OpenQA.Selenium;
using Milyli.UIAutomation.Relativity.Utils;

namespace kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard
{
	public class InstallSQLScriptsPage
	{
		public IWebElement UsernameInput => Configuration.Driver.FindElementById("databaseUsername");
		public IWebElement UsernamePassword => Configuration.Driver.FindElementById("databasePassword");
		public IWebElement RunButton => Configuration.Driver.FindElementById("scriptInstallationSubmitButton");
		public IWebElement CancelButton => Configuration.Driver.FindElementById("scriptInstallationSubmitButton");
		public IWebElement WindowsAuthenticationToggle => Configuration.Driver.FindElementById("col - lg - 5 control - label containsWinAuthToggle");
		public IWebElement SuccessMessageWindow => Configuration.Driver.FindElementById("displaySuccessMessage");
	}
}