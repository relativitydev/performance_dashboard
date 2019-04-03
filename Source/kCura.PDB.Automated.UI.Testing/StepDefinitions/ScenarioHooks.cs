using Milyli.UIAutomation.Relativity;
using Milyli.UIAutomation.Relativity.PageModels;
using Milyli.UIAutomation.Relativity.Utils;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace kCura.PDB.Automated.UI.Testing.StepDefinitions
{
	using kCura.PDB.Tests.Common;
	using OpenQA.Selenium;
	using System.Globalization;
	using System.IO;
	using OpenQA.Selenium.Chrome;

	[Binding]
	public class ScenarioHooks
	{
		[BeforeScenario]
		public void ScenarioSetup()
		{
			var chromeOptions = new ChromeOptions();
			chromeOptions.AddArguments("window-size=1366,768",
				"no-sandbox",
				"incognito",
				"enable-automation",
				"test-type=browser",
				"disable-plugins",
				"disable-extensions",
				"disable-gpu",
				"disable-infobars");
			Configuration.Driver = DriverFactory.Initialize(Browser.Chrome, 100, chromeOptions);
			Configuration.Username = Config.RSAPIUsername;
			Configuration.Password = Config.RSAPIPassword;
			Configuration.Server = $"https://{Config.RSAPIServer}/Relativity/";
			Configuration.Driver.Setup();

			//This creates a means to add the driver to any Page Model
			ScenarioContext.Current.Add("driver", Configuration.Driver);

			var loginPage = new RelativityLoginPageModel(Configuration.Driver);
			loginPage.Login();
		}

		[AfterScenario]
		public void ScenarioTearDown()
		{
			//http://gasparnagy.com/2016/04/specflow-tips-collect-more-information-on-error-part-1/
			if (ScenarioContext.Current.TestError != null)
			{
				var screenshot = ((ITakesScreenshot)Configuration.Driver).GetScreenshot();

				var title = ToTitleCase(ScenarioContext.Current.ScenarioInfo.Title);

				string screenshotFileName = string.Format("{0}_{1}_{2:yyyyMMdd-HHmmss}.png",
					Environment.MachineName,
					title.Substring(0, Math.Min(20, title.Length)),
					DateTime.Now);

				screenshot.SaveAsFile(Path.Combine(Config.UITestScreenshotLocation, screenshotFileName));
				Console.WriteLine("Screenshot saved to: {0}", screenshotFileName);
			}

			Configuration.Driver.Quit();
			Configuration.Driver.Dispose();
		}

		private static string ToTitleCase(string s)
		{
			s = new string(s.Where(c => c == ' ' || char.IsLetterOrDigit(c)).ToArray());

			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s).Replace(" ", "");
		}
	}
}